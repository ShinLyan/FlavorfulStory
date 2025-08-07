using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Stats;
using FlavorfulStory.Utils;
using UnityEngine;

namespace FlavorfulStory.Tools
{
    public class ToolUsageService
    {
        private readonly LayerMask _hitableLayers;

        /// <summary> Максимальная дистанция взаимодействия инструментом. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        public ToolUsageService(LayerMask hitableLayers) { _hitableLayers = hitableLayers; }

        public bool TryUseTool(PlayerController player, Tool tool)
        {
            if (!RaycastUtils.TryGetScreenPointToWorld(
                    InputWrapper.GetMousePosition(),
                    ~(1 << player.gameObject.layer),
                    out var targetPosition))
                return false;

            var stamina = player.GetComponent<PlayerStats>().GetStat<Stamina>();
            if (stamina == null || stamina.CurrentValue < tool.StaminaCost)
                // TODO: Показываем уведомление о нехватке стамины и проигрываем звук
                return false;

            bool didHit = UseToolInDirection(targetPosition, player, tool);
            if (!didHit) return false;

            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{tool.ToolType}");
            InputWrapper.BlockPlayerInput();

            return true;
        }

        /// <summary> Использовать инструмент в заданном направлении. </summary>
        /// <param name="targetPosition"> Целевая позиция для взаимодействия. </param>
        /// <param name="player"> Контроллер игрока. </param>
        private bool UseToolInDirection(Vector3 targetPosition, PlayerController player, Tool tool)
        {
            var origin = player.transform.position;
            var direction = (targetPosition - origin).normalized;
            var interactionCenter = origin + direction * (MaxInteractionDistance / 2);

            //TODO: Тута бага. Получает удар только первый блжайшй. Плохой рейкаст сферой по ударяемым прколам!!!
            var hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius, _hitableLayers);
            foreach (var collider in hitColliders)
                if (collider.transform.parent.TryGetComponent<IHitable>(out var hitable))
                {
                    hitable.TakeHit(tool.ToolType);
                    player.GetComponent<PlayerStats>().GetStat<Stamina>().ChangeValue(-tool.StaminaCost);
                    return true;
                }

            return hitColliders.Length > 0;
        }
    }
}