using System.Collections.Generic;
using FlavorfulStory.InventorySystem;
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

        /// <summary> Свойство возможности взаимодействия с объектом. </summary>
        public virtual bool IsInteractionAllowed { get; set; } = true;

        /// <summary> Получить расстояние до другого трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Выполнить взаимодействие с объектом. </summary>
        public virtual void Interact()
        {
            if (!IsInteractionAllowed) return;

            IsInteractionAllowed = false;
            foreach (var dropItem in _harvestItems)
                Inventory.PlayerInventory.TryAddToFirstAvailableSlot(dropItem.ItemPrefab, dropItem.Quantity);
        }

        #region TooltipBehaviour

        /// <summary> Название объекта, отображаемое в тултипе. </summary>
        // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
        private const string Name = "Wending stump";

        /// <summary> Описание объекта, отображаемое в тултипе. </summary>
        // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
        private const string Description = "Press E, if hungry";

        /// <summary> Получить название для тултипа. </summary>
        /// <returns> Название объекта. </returns>
        public string GetTooltipTitle() => Name;

        /// <summary> Получить описание для тултипа. </summary>
        /// <returns> Описание объекта. </returns>
        public string GetTooltipDescription() => Description;

        /// <summary> Получить позицию объекта в мировых координатах. </summary>
        /// <returns> Мировая позиция объекта. </returns>
        public Vector3 GetWorldPosition() => transform.position;

        #endregion
    }
}