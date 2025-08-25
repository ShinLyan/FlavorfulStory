using UnityEngine;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.InventorySystem.UI.Dragging;

namespace FlavorfulStory
{
    //TODO: Сделать не по-уебански когда будет принят фикс из WindowsRework ветки
    /// <summary> Слот корзины для удаления предметов. </summary>
    public class TrashSlotView : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        [SerializeField] private ItemStackView _itemStackView;
        private Inventory _trashInventory;

        public void Initialize(Inventory inventory)
        {
            if (_trashInventory != null)
                _trashInventory.InventoryUpdated -= UpdateView;

            _trashInventory = inventory;

            if (_trashInventory != null)
            {
                _trashInventory.InventoryUpdated += UpdateView;
                UpdateView();
            }
        }

        private void OnDestroy()
        {
            if (_trashInventory != null)
                _trashInventory.InventoryUpdated -= UpdateView;
        }

        private void UpdateView()
        {
            if (_trashInventory == null) return;
            _itemStackView.UpdateView(_trashInventory.GetItemStackInSlot(0));
        }

        public int GetMaxAcceptableItemsNumber(InventoryItem item)
            => (_trashInventory != null && item != null) ? item.StackSize : 0;

        public void AddItems(InventoryItem item, int number)
        {
            if (_trashInventory == null) return;
            _trashInventory.TryAddItemToSlot(0, item, number);
        }

        public InventoryItem GetItem() => _trashInventory ? _trashInventory.GetItemInSlot(0) : null;
        public int GetNumber() => _trashInventory ? _trashInventory.GetNumberInSlot(0) : 0;

        public void RemoveItems(int number)
        {
            if (_trashInventory != null) _trashInventory.RemoveFromSlot(0, number);
        }
    }
}