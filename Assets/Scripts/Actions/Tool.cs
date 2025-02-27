using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    /// <remarks> Может выполнять действия, специфичные для типа инструмента. </remarks>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Tool"))]
    public class Tool : ActionItem
    {
        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Максимальная дистанция взаимодействия. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        /// <summary> Использовать инструмент. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Use(PlayerController player)
        {
            var targetPosition = PlayerController.GetCursorPosition();
            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            player.EquipTool(this);
            UseToolInDirection(targetPosition, player);

            // TODO: Реализовать трату энергии игрока при использовании инструмента
        }

        /// <summary> Использовать инструмент в заданном направлении. </summary>
        /// <param name="targetPosition"> Целевая позиция, куда направлено взаимодействие. </param>
        /// <param name="player"> Контроллер игрока. </param>
        private void UseToolInDirection(Vector3 targetPosition, PlayerController player)
        {
            var origin = player.transform.position;
            var direction = (targetPosition - origin).normalized;
            var interactionCenter = origin + direction * (MaxInteractionDistance / 2);
            var hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius);
            // foreach (var collider in hitColliders)
            // {
            //     if (collider.TryGetComponent<InteractableObject>(out var interactableObject))
            //         interactableObject.Interact(player);
            // }
            
            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent<IHitable>(out var hitable))
                    hitable.TakeHit(ToolType);
            }

            Debug.DrawLine(origin, interactionCenter, Color.red, 5f);
        }
    }
}