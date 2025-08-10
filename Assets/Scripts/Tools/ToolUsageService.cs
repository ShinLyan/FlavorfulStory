using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using UnityEngine;

namespace FlavorfulStory.Tools
{
    /// <summary> Сервис для использования инструментов игроком. </summary>
    public class ToolUsageService
    {
        /// <summary> Словарь, сопоставляющий тип инструмента с его префабом. </summary>
        private readonly Dictionary<ToolType, GameObject> _toolPrefabs;

        /// <summary> Слой, по которому осуществляется попадание при использовании инструмента. </summary>
        private readonly LayerMask _hitableLayers;

        /// <summary> Игрок, использующий инструмент. </summary>
        private readonly PlayerController _player;

        /// <summary> Буфер коллайдеров для поиска объектов, которые можно ударить. </summary>
        private readonly Collider[] _hitsBuffer = new Collider[10];

        /// <summary> Находится ли инструмент на перезарядке? </summary>
        private bool _isOnCooldown;

        /// <summary> Текущий отображаемый инструмент, прикреплённый к руке игрока. </summary>
        private GameObject _activeTool;

        /// <summary> Максимальная дистанция взаимодействия инструментом (в клетках). </summary>
        public const int MaxDistanceInCells = 2;

        /// <summary> Задержка между использованиями инструмента (в секундах). </summary>
        private const float CooldownSeconds = 1f;

        /// <summary> Событие, вызываемое в момент начала использования инструмента. </summary>
        public event Action<Vector3, GridIndicatorState> ToolUseStarted;

        /// <summary> Событие, вызываемое после завершения использования инструмента. </summary>
        public event Action ToolUseFinished;

        /// <summary> Конструктор сервиса использования инструментов. </summary>
        /// <param name="toolMappings"> Маппинг типов инструментов на соответствующие префабы для отображения. </param>
        /// <param name="hitableLayers"> Слой, по которому осуществляется попадание при использовании. </param>
        /// <param name="player"> Игрок, использующий инструмент. </param>
        public ToolUsageService(ToolPrefabMapping[] toolMappings, LayerMask hitableLayers, PlayerController player)
        {
            _hitableLayers = hitableLayers;
            _player = player;

            _toolPrefabs = new Dictionary<ToolType, GameObject>();
            foreach (var toolPrefabMapping in toolMappings)
            {
                if (toolPrefabMapping == null) continue;

                _toolPrefabs[toolPrefabMapping.ToolType] = toolPrefabMapping.ToolPrefab;
            }
        }

        /// <summary> Попробовать использовать инструмент в указанной клетке. </summary>
        /// <param name="tool"> Инструмент, который используется. </param>
        /// <param name="cellCenter"> Центр клетки, в которую целимся. </param>
        /// <returns> Успешно ли произошло попадание. </returns>
        public bool TryUseTool(Tool tool, Vector3 cellCenter)
        {
            if (_isOnCooldown) return false;

            var target = ClampTargetToChebyshevRange(_player.transform.position, cellCenter, MaxDistanceInCells);

            bool hitSuccessful = TryGetValidHitableAt(tool, target, out var hitable);
            if (hitSuccessful)
            {
                hitable.TakeHit();
                SfxPlayer.Play(hitable.SfxType);
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
        /// <returns> True, если найден подходящий IHitable. </returns>
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

        /// <summary> Выполняет подготовительные действия при начале использования инструмента. </summary>
        /// <param name="tool"> Инструмент, который используется. </param>
        /// <param name="target"> Мировая позиция цели удара. </param>
        private void BeginUse(Tool tool, Vector3 target)
        {
            _player.RotateTowards(target);
            _player.TriggerAnimation($"Use{tool.ToolType}");
            _player.SetBusyState(true);
            SfxPlayer.Play(tool.SfxType);
            EquipTool(tool);
        }

        /// <summary> Завершает процесс использования инструмента. </summary>
        private void EndUse()
        {
            UnequipTool();
            _player.SetBusyState(false);
            ToolUseFinished?.Invoke();
        }

        /// <summary> Запускает перезарядку между ударами инструмента и завершает использование. </summary>
        private async UniTaskVoid StartCooldownAsync()
        {
            _isOnCooldown = true;

            await UniTask.Delay(TimeSpan.FromSeconds(CooldownSeconds));

            _isOnCooldown = false;
            EndUse();
        }

        /// <summary> Активирует отображение инструмента в руке игрока на основе его типа. </summary>
        /// <param name="tool"> Инструмент, который нужно отобразить. </param>
        /// <remarks> Также блокирует прокрутку мыши, чтобы исключить случайную смену предмета. </remarks>
        private void EquipTool(Tool tool)
        {
            if (_activeTool) return;

            if (!_toolPrefabs.TryGetValue(tool.ToolType, out var prefab) || !prefab) return;

            _activeTool = prefab;
            _activeTool.SetActive(true);

            InputWrapper.BlockInput(InputButton.MouseScroll);
        }

        /// <summary> Отключает отображение текущего инструмента и разблокирует управление игроком. </summary>
        /// <remarks> Также разблокирует ввод (прокрутку мыши и движение). </remarks>
        private void UnequipTool()
        {
            if (!_activeTool) return;

            _activeTool.SetActive(false);
            _activeTool = null;

            InputWrapper.UnblockPlayerInput();
        }
    }
}