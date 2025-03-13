using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.ResourceContainer;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    /// <remarks> Может выполнять действия, специфичные для типа инструмента. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Tool")]
    public class Tool : ActionItem
    {
        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Максимальная дистанция взаимодействия инструментом. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        /// <summary> Использовать инструмент для взаимодействия с объектами. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Use(PlayerController player)
        {
            var targetPosition = PlayerController.GetCursorPosition();
            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            player.EquipTool(this);
            UseToolInDirection(targetPosition, player, player.HitableLayers);

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
            Debug.DrawLine(origin, interactionCenter, Color.red, 5f);
        }
    }
}