using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Слот инвентаря в UI.</summary>
    public class InventorySlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        /// <summary> Инонка слота инвентаря.</summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> Индекс слота в инвентаре.</summary>
        private int _index;

        /// <summary> Инвентарь.</summary>
        private Inventory _inventory;

        /// <summary> Инициализация полей.</summary>
        private void Awake()
        {
            _inventory = Inventory.PlayerInventory;
        }

        /// <summary> Установить значения класса.</summary>
        /// <param name="inventory"> Инвентарь.</param>
        /// <param name="index"> Индекс слота в инвентаре.</param>
        public void Setup(Inventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
            _icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        /// <summary> Получить максимально допустимое количество элементов.</summary>
        /// <remarks> Если ограничения нет, то должно быть возвращено значение Int.MaxValue.</remarks>
        /// <param name="item">Тип элемента, который потенциально может быть добавлен.</param>
        /// <returns> Возвращает максимально допустимое количество элементов.</returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item)
        {
            if (_inventory.HasSpaceFor(item)) return int.MaxValue;
            return 0;
        }

        /// <summary> Обновить UI и все данные для отображения добавления элемента в это место назначения.</summary>
        /// <param name="item">Тип элемента.</param>
        /// <param name="number">Количество элементов.</param>
        public void AddItems(InventoryItem item, int number)
        {
            _inventory.TryAddItemToSlot(_index, item, number);
        }

        /// <summary> Получить предмет, который в данный момент находится в этом источнике.</summary>
        /// <returns> Возвращает предмет, который в данный момент находится в этом источнике.</returns>
        public InventoryItem GetItem() => _inventory.GetItemInSlot(_index);

        /// <summary> Получить количество предметов.</summary>
        /// <returns> Возвращает количество предметов.</returns>
        public int GetNumber() => _inventory.GetNumberInSlot(_index);

        /// <summary> Удалить заданное количество предметов из источника.</summary>
        /// <param name="number"> Количество предметов, которое необходимо удалить.</param>
        /// <remarks> Значение number не должно превышать число, возвращаемое с помощью "GetNumber".</remarks>
        public void RemoveItems(int number)
        {
            _inventory.RemoveFromSlot(_index, number);
        }
    }
}