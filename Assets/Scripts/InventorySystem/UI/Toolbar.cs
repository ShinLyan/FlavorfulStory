using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов.</summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> Массив слотов панели инструментов.</summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> Индекс выбранного предмета.</summary>
        public int SelectedItemIndex { get; private set; }

        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Инициализация полей.</summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            _slots[SelectedItemIndex].Select();

            Inventory.PlayerInventory.InventoryUpdated += RedrawToolbar;
        }

        /// <summary> При старте перерисовываем инвентарь.</summary>
        private void Start()
        {
            RedrawToolbar();
        }

        /// <summary> Перерисовать панель инструментов.</summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();
        }

        /// <summary> Выбрать предмет на панели.</summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать.</param>
        public void SelectItem(int index)
        {
            _slots[SelectedItemIndex].ResetSelection();

            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();
        }
    }
}