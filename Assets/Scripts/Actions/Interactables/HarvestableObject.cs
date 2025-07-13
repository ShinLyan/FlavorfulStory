using System.Collections.Generic;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Абстрактный класс для собираемых объектов. </summary>
    /// <remarks> Реализует интерфейс <see cref="IInteractable" />. </remarks>
    public abstract class HarvestableObject : MonoBehaviour, IInteractable
    {
        /// <summary> Собираемые предметы/ресурсы, что будут добавлены в инвентарь при взаимодействии. </summary>
        [Tooltip("Собираемые предметы."), SerializeField]
        private List<DropItem> _harvestItems;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public TooltipActionData TooltipAction => new("E", ActionType.Gather, _harvestItems[0].ItemPrefab.ItemName);

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
                _playerInventory.TryAddToFirstAvailableSlot(dropItem.ItemPrefab, dropItem.Quantity);
        }

        /// <summary> Завершает взаимодействие с объектом. </summary>
        /// <param name="player"> Контроллер игрока, который взаимодействует с объектом. </param>
        public void EndInteraction(PlayerController player) { }

        #endregion
    }
}