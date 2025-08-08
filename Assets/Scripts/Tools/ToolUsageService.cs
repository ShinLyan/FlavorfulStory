using System;
using System.Linq;
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
        /// <summary> Маппинг типов инструментов на соответствующие префабы для отображения. </summary>
        private readonly ToolPrefabMapping[] _toolMappings;

        /// <summary> Слой, по которому осуществляется попадание при использовании инструмента. </summary>
        private readonly LayerMask _hitableLayers;

        /// <summary> Игрок, использующий инструмент. </summary>
        private readonly PlayerController _player;

        /// <summary> Находится ли инструмент на перезарядке? </summary>
        private bool _isOnCooldown;

        /// <summary> Текущий отображаемый инструмент, прикреплённый к руке игрока. </summary>
        private GameObject _activeTool;

        /// <summary> Максимальная дистанция взаимодействия инструментом (в клетках). </summary>
        private const int MaxDistanceInCells = 2;

        /// <summary> Задержка между использованиями инструмента (в секундах). </summary>
        private const float CooldownSeconds = 1f;

        /// <summary> Конструктор сервиса использования инструментов. </summary>
        /// <param name="toolMappings"> Маппинг типов инструментов на соответствующие префабы для отображения. </param>
        /// <param name="hitableLayers"> Слой, по которому осуществляется попадание при использовании. </param>
        /// <param name="player"> Игрок, использующий инструмент. </param>
        public ToolUsageService(ToolPrefabMapping[] toolMappings, LayerMask hitableLayers, PlayerController player)
        {
            _toolMappings = toolMappings;
            _hitableLayers = hitableLayers;
            _player = player;
        }

        /// <summary> Попробовать использовать инструмент в указанной клетке. </summary>
        /// <param name="tool"> Инструмент, который используется. </param>
        /// <param name="cellCenter"> Центр клетки, в которую целимся. </param>
        /// <returns> Успешно ли произошло попадание. </returns>
        public bool TryUseTool(Tool tool, Vector3 cellCenter)
        {
            if (_isOnCooldown) return false;

            var clampedTarget = ClampTargetToRange(_player.transform.position, cellCenter, MaxDistanceInCells);
            DrawDebugHit(clampedTarget);

            bool hitSuccessful = TryGetValidHitableAt(tool, clampedTarget, out var hitable);
            if (hitSuccessful)
            {
                hitable.TakeHit();
                SfxPlayer.Play(hitable.SfxType);
            }

            _player.RotateTowards(clampedTarget);
            _player.TriggerAnimation($"Use{tool.ToolType}");
            _player.SetBusyState(true);
            SfxPlayer.Play(tool.SfxType);

            EquipTool(tool);
            StartCooldownAsync().Forget();

            return hitSuccessful;
        }

        /// <summary> Ограничивает целевую точку до радиуса в клетках от игрока. </summary>
        /// <param name="playerPos"> Позиция игрока в мировых координатах. </param>
        /// <param name="target"> Исходная целевая позиция (куда игрок хочет применить инструмент). </param>
        /// <param name="maxCells"> Максимальная дистанция в клетках, на которую можно применять инструмент. </param>
        /// <returns> Целевая позиция, ограниченная радиусом действия инструмента. </returns>
        private static Vector3 ClampTargetToRange(Vector3 playerPos, Vector3 target, int maxCells)
        {
            var direction = target - playerPos;
            float maxDistance = GridPositionProvider.CellsToWorldDistance(maxCells);
            return direction.magnitude <= maxDistance ? target : playerPos + direction.normalized * maxDistance;
        }

        /// <summary> Попробовать получить подходящий для удара объект в указанной клетке. </summary>
        /// <param name="tool"> Инструмент в руках игрока. </param>
        /// <param name="cellCenter"> Центр клетки. </param>
        /// <param name="hitable"> Найденный IHitable (если есть). </param>
        /// <returns> True, если найден подходящий IHitable. </returns>
        public bool TryGetValidHitableAt(Tool tool, Vector3 cellCenter, out IHitable hitable)
        {
            hitable = null;

            var hits = Physics.OverlapBox(cellCenter, GridPositionProvider.CellHalfExtents,
                Quaternion.identity, _hitableLayers);

            foreach (var collider in hits)
            {
                var candidate = collider.GetComponentInParent<IHitable>();
                if (candidate == null) continue;

                if (!candidate.CanBeHitBy(tool.ToolType, tool.ToolLevel)) continue;

                hitable = candidate;
                return true;
            }

            return false;
        }

        /// <summary> Отрисовывает в сцене отладочную визуализацию клетки, в которую был произведён удар. </summary>
        /// <param name="center"> Центр ячейки, в которую был направлен удар. </param>
        private static void DrawDebugHit(Vector3 center)
        {
            var color = Color.magenta;
            float duration = 1f;

            // ⬆ вертикальный луч из центра ячейки
            Debug.DrawRay(center, Vector3.up * 2f, color, duration);

            // ⬜ квадрат вокруг центра ячейки (Y=0)
            Vector3[] corners =
            {
                center + new Vector3(-0.5f, 0, -0.5f),
                center + new Vector3(-0.5f, 0, 0.5f),
                center + new Vector3(0.5f, 0, 0.5f),
                center + new Vector3(0.5f, 0, -0.5f)
            };

            Debug.DrawLine(corners[0], corners[1], color, duration);
            Debug.DrawLine(corners[1], corners[2], color, duration);
            Debug.DrawLine(corners[2], corners[3], color, duration);
            Debug.DrawLine(corners[3], corners[0], color, duration);
        }

        /// <summary> Запускает перезарядку между ударами инструмента и завершает использование. </summary>
        private async UniTaskVoid StartCooldownAsync()
        {
            _isOnCooldown = true;

            await UniTask.Delay(TimeSpan.FromSeconds(CooldownSeconds));

            UnequipTool();

            _player.SetBusyState(false);
            _isOnCooldown = false;
        }

        /// <summary> Активирует отображение инструмента в руке игрока на основе его типа. </summary>
        /// <param name="tool"> Инструмент, который нужно отобразить. </param>
        /// <remarks> Также блокирует прокрутку мыши, чтобы исключить случайную смену предмета. </remarks>
        private void EquipTool(Tool tool)
        {
            if (_activeTool) return;

            var prefab = _toolMappings.FirstOrDefault(mapping => mapping.ToolType == tool.ToolType)?.ToolPrefab;
            if (!prefab) return;

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