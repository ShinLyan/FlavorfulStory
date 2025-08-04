using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Слот инвентаря в UI. </summary>
    public class InventorySlotView : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [field: SerializeField] public ItemStackView _itemStackView { get; private set; }

        /// <summary> Индекс слота в инвентаре. </summary>
        private int _index;

        /// <summary> Инвентарь, с которым связан слот. </summary>
        private Inventory _inventory;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        public void Construct(Inventory inventory) => _inventory = inventory;

        /// <summary> Установить индекс слота. </summary>
        /// <param name="index"> Индекс в инвентаре. </param>
        public void Setup(int index)
        {
            if (_inventory == null)
                throw new System.Exception("[InventorySlotView] Inventory not assigned! " +
                                           "Call Construct() before Setup()");
            
            _index = index;
            _itemStackView.UpdateView(_inventory.GetItemStackInSlot(_index));
        }

        /// <summary> Получить максимально допустимое количество элементов. </summary>
        /// <remarks> Если ограничения нет, то должно быть возвращено значение Int.MaxValue. </remarks>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Возвращает максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => _inventory.HasSpaceFor(item) ? int.MaxValue : 0;

        /// <summary> Обновить UI и все данные для отображения добавления элемента в это место назначения. </summary>
        /// <param name="item">Тип элемента. </param>
        /// <param name="number">Количество элементов. </param>
        public void AddItems(InventoryItem item, int number) => _inventory.TryAddItemToSlot(_index, item, number);

        /// <summary> Получить предмет, который в данный момент находится в этом источнике. </summary>
        /// <returns> Возвращает предмет, который в данный момент находится в этом источнике. </returns>
        public InventoryItem GetItem() => _inventory.GetItemInSlot(_index);

        /// <summary> Получить количество предметов. </summary>
        /// <returns> Возвращает количество предметов. </returns>
        public int GetNumber() => _inventory.GetNumberInSlot(_index);

        /// <summary> Удалить заданное количество предметов из источника. </summary>
        /// <param name="number"> Количество предметов, которое необходимо удалить. </param>
        /// <remarks> Значение number не должно превышать число, возвращаемое с помощью "GetNumber". </remarks>
        public void RemoveItems(int number) => _inventory.RemoveFromSlot(_index, number);
    }
}