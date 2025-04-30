using System.Collections.Generic;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Абстрактный класс для собираемых объектов. </summary>
    /// <remarks> Реализует интерфейс <see cref="IInteractable" />. </remarks>
    public abstract class HarvestableObject : MonoBehaviour, IInteractable
    {
        /// <summary> Собираемые предметы/ресурсы, что будут добавлены в инвентарь при взаимодействии. </summary>
        [Tooltip("Собираемые предметы."), SerializeField]
        private List<DropItem> _harvestItems;

        /// <summary> Название объекта, отображаемое в тултипе. </summary>
        [Tooltip("Название объекта для отображения в интерфейсе."), SerializeField]
        private string _name;

        /// <summary> Описание объекта, отображаемое в тултипе. </summary>
        [Tooltip("Описание объекта для отображения в интерфейсе."), SerializeField]
        private string _description = "Press E, if hungry";

        #region IInteractable

        /// <summary> Свойство возможности взаимодействия с объектом. </summary>
        public virtual bool IsInteractionAllowed { get; protected set; } = true;

        /// <summary> Получить расстояние до другого трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начинает взаимодействие с объектом. </summary>
        /// <param name="player"> Контроллер игрока, который взаимодействует с объектом. </param>
        public virtual void BeginInteraction(PlayerController player)
        {
            if (!IsInteractionAllowed) return;

            player.TriggerAnimation(AnimationType.Gather);

            IsInteractionAllowed = false;

            foreach (var dropItem in _harvestItems)
                Inventory.PlayerInventory.TryAddToFirstAvailableSlot(dropItem.ItemPrefab, dropItem.Quantity);
        }

        /// <summary> Завершает взаимодействие с объектом. </summary>
        /// <param name="player"> Контроллер игрока, который взаимодействует с объектом. </param>
        public void EndInteraction(PlayerController player) { }

        #endregion

        #region ITooltipable

        /// <summary> Получить название для тултипа. </summary>
        /// <returns> Название объекта. </returns>
        public string TooltipTitle => _name;

        /// <summary> Получить описание для тултипа. </summary>
        /// <returns> Описание объекта. </returns>
        public string TooltipDescription => _description;

        /// <summary> Получить позицию объекта в мировых координатах. </summary>
        /// <returns> Мировая позиция объекта. </returns>
        public Vector3 WorldPosition => !this ? Vector3.zero : transform.position;

        #endregion
    }
}