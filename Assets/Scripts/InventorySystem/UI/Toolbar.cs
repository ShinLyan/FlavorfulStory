using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов.</summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> Массив слотов панели инструментов.</summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> Индекс выбранного предмета.</summary>
        private int _selectedItemIndex = 0;

        private ToolbarSlotUI SelectedItem => _slots[_selectedItemIndex];

        /// <summary> Инициализация полей.</summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            _slots[_selectedItemIndex].Select();

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
            foreach (var slot in _slots)
            {
                slot.Redraw();
            }
        }

        /// <summary> Выбрать предмет на панели.</summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать.</param>
        public void SelectItem(int index)
        {
            _slots[_selectedItemIndex].ResetSelection();

            _selectedItemIndex = index;
            _slots[_selectedItemIndex].Select();
        }

        /// <summary> Попробовать использовать предмет в выбранном слоте.</summary>
        /// <remarks> Если предмет расходуемый, то один экземпляр будет уничтожен.</remarks>
        /// <returns> Возвращает False, если действие не удалось выполнить.</returns>
        public bool TryUseSelectedItem()
        {
            if (SelectedItem.GetItem() is ActionItem actionItem)
            {
                actionItem.Use();
                if (actionItem.IsConsumable)
                {
                    Inventory.PlayerInventory.RemoveFromSlot(_selectedItemIndex, 1);
                    print($"{this} потратитлся");
                }
                return true;
            }
            return false;
        }
    }
}