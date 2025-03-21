using FlavorfulStory.Control;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.ResourceContainer;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    /// <remarks> Может выполнять действия, специфичные для типа инструмента. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Tool")]
    public class Tool : InventoryItem, IUsable
    {
        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Максимальная дистанция взаимодействия инструментом. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        [field: Tooltip("Кнопка использования"), SerializeField]
        public UseActionType UseActionType { get; set; }

        /// <summary> Использовать инструмент для взаимодействия с объектами. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слои, по котормы будем делать удар. </param>
        public void Use(PlayerController player, LayerMask hitableLayers)
        {
            int playerLayer = player.gameObject.layer;
            if (WorldCoordinates.GetWorldCoordinatesFromScreenPoint(
                    //~(1 << playerLayer) = LayerMask.all except player.layer
                    InputWrapper.GetMousePosition(), ~(1 << playerLayer), out var targetPosition
                ))
            {
                player.RotateTowards(targetPosition);
                player.TriggerAnimation($"Use{ToolType}");
                UseToolInDirection(targetPosition, player, hitableLayers);
            }

            // TODO: Реализовать трату энергии игрока при использовании инструмента
        }

        /// <summary> Использовать инструмент в заданном направлении. </summary>
        /// <param name="targetPosition"> Целевая позиция для взаимодействия. </param>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слой объектов, с которыми можно взаимодействовать. </param>
        private void UseToolInDirection(Vector3 targetPosition, PlayerController player, LayerMask hitableLayers)
        {
            var origin = player.transform.position;
            var direction = (targetPosition - origin).normalized;
            var interactionCenter = origin + direction * (MaxInteractionDistance / 2);

            // TODO: Проверь что корректно работает. Было написано под пивом
            var hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius, hitableLayers);

            foreach (var collider in hitColliders)
                if (collider.transform.parent.TryGetComponent<IHitable>(out var hitable))
                    hitable.TakeHit(ToolType);

            // Debug
            Debug.DrawLine(origin, interactionCenter, Color.red, 50);
        }
    }
}