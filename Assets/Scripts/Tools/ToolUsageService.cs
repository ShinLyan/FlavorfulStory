using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.PlacementSystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlavorfulStory.Tools
{
    /// <summary> Сервис для использования инструментов игроком. </summary>
    public class ToolUsageService
    {
        /// <summary> Презентер инструментов. </summary>
        private readonly ToolPresenter _toolPresenter;

        /// <summary> Слой, по которому осуществляется попадание при использовании инструмента. </summary>
        private readonly LayerMask _hitableLayers;

        /// <summary> Игрок, использующий инструмент. </summary>
        private readonly PlayerController _player;

        /// <summary> Контроллер размещение объектов. </summary>
        private readonly PlacementController _placementController;

        /// <summary> Сервис выброса предметов в игровой мир. </summary>
        private readonly IItemDropService _itemDropService;

        /// <summary> Буфер коллайдеров для поиска объектов, которые можно ударить. </summary>
        private readonly Collider[] _hitsBuffer = new Collider[10];

        /// <summary> Находится ли инструмент на перезарядке? </summary>
        private bool _isOnCooldown;

        /// <summary> Сигнальная шина Zenject. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Максимальная дистанция взаимодействия инструментом (в клетках). </summary>
        public const int MaxDistanceInCells = 2;

        /// <summary> Задержка между использованиями инструмента (в секундах). </summary>
        private const float CooldownSeconds = 1f;

        /// <summary> Событие, вызываемое в момент начала использования инструмента. </summary>
        public event Action<Vector3, GridIndicatorState> ToolUseStarted;

        /// <summary> Событие, вызываемое после завершения использования инструмента. </summary>
        public event Action ToolUseFinished;

        /// <summary> Конструктор сервиса использования инструментов. </summary>
        /// <param name="toolMappings"> Массив структур, сопоставляющих <see cref="ToolType"/> с его префабом. </param>
        /// <param name="hitableLayers"> Слой, по которому осуществляется попадание при использовании. </param>
        /// <param name="player"> Игрок, использующий инструмент. </param>
        /// <param name="placementController"> Контроллер размещение объектов. </param>
        /// <param name="itemDropService"> Сервис выброса предметов в игровой мир. </param>
        /// <param name="signalBus"> Сигнальная шина Zenject. </param>
        public ToolUsageService(ToolPrefabMapping[] toolMappings, LayerMask hitableLayers, PlayerController player,
            PlacementController placementController, IItemDropService itemDropService, SignalBus signalBus)
        {
            _toolPresenter = new ToolPresenter(toolMappings);
            _hitableLayers = hitableLayers;
            _player = player;
            _placementController = placementController;
            _itemDropService = itemDropService;
            _signalBus = signalBus;
        }

        /// <summary> Попробовать использовать инструмент в указанной клетке. </summary>
        /// <param name="tool"> Инструмент, который используется. </param>
        /// <param name="cellCenter"> Центр клетки, в которую целимся. </param>
        /// <param name="isDismantle"></param>
        /// <returns> Успешно ли произошло попадание. </returns>
        public bool TryUseTool(Tool tool, Vector3 cellCenter, out bool isDismantle)
        {
            isDismantle = false;
            if (_isOnCooldown) return false;

            var target = ClampTargetToChebyshevRange(_player.transform.position, cellCenter, MaxDistanceInCells);

            bool hitSuccessful = TryGetValidHitableAt(tool, target, out var hitable);
            if (hitSuccessful)
            {
                hitable.TakeHit();
                SfxPlayer.Play(hitable.SfxType);
            }
            else if (tool.CanDismantlePlaceables && TryDismantleAt(target))
            {
                hitSuccessful = true;
                isDismantle = true;
                SfxPlayer.Play(SfxType.RemoveObject);
            }

            var state = hitSuccessful ? GridIndicatorState.ValidTarget : GridIndicatorState.InvalidTarget;
            ToolUseStarted?.Invoke(target, state);

            BeginUse(tool, target);
            StartCooldownAsync().Forget();

            return hitSuccessful;
        }

        /// <summary> Ограничивает целевую точку квадратом Чебышёва с радиусом maxCells (включая диагонали). </summary>
        /// <param name="playerPosition"> Позиция игрока в мировых координатах. </param>
        /// <param name="target"> Исходная целевая позиция (куда игрок хочет применить инструмент). </param>
        /// <param name="maxCells"> Максимальная дистанция в клетках, на которую можно применять инструмент. </param>
        /// <returns> Целевая позиция, ограниченная радиусом действия инструмента. </returns>
        private static Vector3 ClampTargetToChebyshevRange(Vector3 playerPosition, Vector3 target, int maxCells)
        {
            float cell = GridPositionProvider.CellsToWorldDistance(1);
            float max = cell * maxCells;

            float dx = Mathf.Clamp(target.x - playerPosition.x, -max, max);
            float dz = Mathf.Clamp(target.z - playerPosition.z, -max, max);

            return new Vector3(playerPosition.x + dx, target.y, playerPosition.z + dz);
        }

        /// <summary> Попробовать получить подходящий для удара объект в указанной клетке. </summary>
        /// <param name="tool"> Инструмент в руках игрока. </param>
        /// <param name="cellCenter"> Центр клетки. </param>
        /// <param name="hitable"> Найденный IHitable (если есть). </param>
        /// <returns> <c>true</c>, если найден подходящий IHitable. </returns>
        public bool TryGetValidHitableAt(Tool tool, Vector3 cellCenter, out IHitable hitable)
        {
            hitable = null;

            int count = Physics.OverlapBoxNonAlloc(cellCenter, GridPositionProvider.CellHalfExtents, _hitsBuffer,
                Quaternion.identity, _hitableLayers);
            for (int i = 0; i < count; i++)
            {
                var candidate = _hitsBuffer[i].GetComponentInParent<IHitable>();
                if (candidate == null || !candidate.CanBeHitBy(tool.ToolType, tool.ToolLevel)) continue;

                hitable = candidate;
                return true;
            }

            return false;
        }

        /// <summary> Попробовать демонтировать размещённый объект в указанной клетке. </summary>
        /// <param name="cellCenter"> Центр клетки, в которой производится демонтаж. </param>
        /// <returns> <c>true</c>, если объект был успешно демонтирован. </returns>
        private bool TryDismantleAt(Vector3 cellCenter)
        {
            if (!_placementController.TryRemoveAt(cellCenter, out var removedPlaceable)) return false;

            if (removedPlaceable.TryGetComponent(out ICanBeDismantled restriction) && !restriction.CanBeDismantled)
            {
                _placementController.RegisterPlacedObject(removedPlaceable.transform.position, removedPlaceable);
                _signalBus.Fire(new DismantleDeniedSignal(restriction.DismantleDeniedReason));
                return false;
            }

            _itemDropService.Drop(new ItemStack(removedPlaceable.PlaceableItem, 1), cellCenter,
                ItemDropService.ResourceDropForce);

            Object.Destroy(removedPlaceable.gameObject);
            return true;
        }

        /// <summary> Выполняет подготовительные действия при начале использования инструмента. </summary>
        /// <param name="tool"> Инструмент, который используется. </param>
        /// <param name="target"> Мировая позиция цели удара. </param>
        private void BeginUse(Tool tool, Vector3 target)
        {
            _player.RotateTowards(target);
            _player.TriggerAnimation($"Use{tool.ToolType}");
            _player.SetBusyState(true);
            SfxPlayer.Play(tool.SfxType);
            _toolPresenter.EquipTool(tool);

            InputWrapper.BlockInput(InputButton.MouseScroll);
        }

        /// <summary> Завершает процесс использования инструмента. </summary>
        private void EndUse()
        {
            _toolPresenter.UnequipTool();
            _player.SetBusyState(false);
            ToolUseFinished?.Invoke();

            InputWrapper.UnblockPlayerInput();
        }

        /// <summary> Запускает перезарядку между ударами инструмента и завершает использование. </summary>
        private async UniTaskVoid StartCooldownAsync()
        {
            _isOnCooldown = true;

            await UniTask.Delay(TimeSpan.FromSeconds(CooldownSeconds));

            _isOnCooldown = false;
            EndUse();
        }
    }
}