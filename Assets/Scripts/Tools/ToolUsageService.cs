using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Stats;
using UnityEngine;

namespace FlavorfulStory.Tools
{
    public class ToolUsageService
    {
        private readonly LayerMask _hitableLayers;

        /// <summary> Максимальная дистанция взаимодействия инструментом (в клетках). </summary>
        private const int MaxDistanceInCells = 2;

        public ToolUsageService(LayerMask hitableLayers) => _hitableLayers = hitableLayers;

        public bool TryUseTool(PlayerController player, Tool tool, Vector3 targetCellCenter)
        {
            var stamina = player.GetComponent<PlayerStats>().GetStat<Stamina>();
            if (stamina == null || stamina.CurrentValue < tool.StaminaCost) return false;

            // Ограничиваем позицию до диапазона 2 клеток от игрока
            var clampedTarget = ClampTargetToRange(player.transform.position, targetCellCenter, MaxDistanceInCells);

            DrawDebugHit(clampedTarget);

            bool hitSuccessful = TryHit(clampedTarget, tool);

            player.RotateTowards(clampedTarget);
            player.TriggerAnimation($"Use{tool.ToolType}");
            InputWrapper.BlockPlayerInput();

            if (!hitSuccessful) return true;

            stamina.ChangeValue(-tool.StaminaCost);
            SfxPlayer.Play(tool.SfxType);

            return true;
        }

        /// <summary> Ограничивает целевую точку до радиуса в клетках от игрока. </summary>
        private static Vector3 ClampTargetToRange(Vector3 playerPos, Vector3 target, int maxCells)
        {
            var direction = target - playerPos;
            float maxDistance = GridPositionProvider.CellsToWorldDistance(maxCells);

            if (direction.magnitude <= maxDistance) return target;

            return playerPos + direction.normalized * maxDistance;
        }

        private bool TryHit(Vector3 cellCenter, Tool tool)
        {
            var hits = Physics.OverlapBox(cellCenter, GridPositionProvider.CellHalfExtents,
                Quaternion.identity, _hitableLayers);
            foreach (var collider in hits)
            {
                var hitable = collider.GetComponentInParent<IHitable>();
                if (hitable == null) continue;

                hitable.TakeHit(tool.ToolType);
                return true;
            }

            return false;
        }

        private void DrawDebugHit(Vector3 center)
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
    }
}