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
        [field: Tooltip("Тип инструмента.")]
        [field: SerializeField] public ToolType ToolType { get; private set; }

        /// <summary> Максимальная дистанция взаимодействия. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        /// <summary> Использовать инструмент. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Use(PlayerController player)
        {
            Debug.Log("Use Tool");
            
            var targetPosition = PlayerController.GetCursorPosition();
            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            player.EquipTool(this);
            player.ScheduleUnequipTool();
            UseToolInDirection(player.transform.position, targetPosition, player);

            // TODO: Реализовать трату энергии игрока при использовании инструмента
        }

        /// <summary> Использовать инструмент в заданном направлении. </summary>
        /// <param name="origin"> Начальная точка взаимодействия. </param>
        /// <param name="targetPosition"> Целевая позиция, куда направлено взаимодействие. </param>
        /// <param name="player"> Контроллер игрока. </param>
        private void UseToolInDirection(Vector3 origin, Vector3 targetPosition, PlayerController player)
        {
            Vector3 direction = (targetPosition - origin).normalized;
            Vector3 interactionCenter = origin + direction * (MaxInteractionDistance / 2);
            Collider[] hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius);
            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent<InteractableObject>(out var interactableObject))
                {
                    interactableObject.Interact(player);
                    Debug.Log($"Interacted with object: {interactableObject.name}");
                }
            }
            Debug.DrawLine(origin, interactionCenter, Color.red, 5f);
        }
    }
}