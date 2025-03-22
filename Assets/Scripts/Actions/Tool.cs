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
        #region Fields and Properties

        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Кнопка использования предмета. </summary>
        [field: Tooltip("Кнопка использования"), SerializeField]
        public UseActionType UseActionType { get; set; }

        /// <summary> Максимальная дистанция взаимодействия инструментом. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        #endregion

        /// <summary> Использовать инструмент для взаимодействия с объектами. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слои, по которым будем делать удар. </param>
        public void Use(PlayerController player, LayerMask hitableLayers)
        {
            if (!WorldCoordinates.GetWorldCoordinatesFromScreenPoint(
                    InputWrapper.GetMousePosition(),
                    ~(1 << player.gameObject.layer), // LayerMask.all except player.layer
                    out var targetPosition
                )) return;

            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            UseToolInDirection(targetPosition, player, hitableLayers);
            InputWrapper.BlockPlayerMovement();

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

            var hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius, hitableLayers);

            foreach (var collider in hitColliders)
                if (collider.transform.parent.TryGetComponent<IHitable>(out var hitable))
                    hitable.TakeHit(ToolType);
        }
    }
}