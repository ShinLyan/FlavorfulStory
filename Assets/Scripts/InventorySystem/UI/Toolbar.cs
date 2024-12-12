using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> ������ ������������.</summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> ������ ������ ������ ������������.</summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> ������ ���������� ��������.</summary>
        private int _selectedItemIndex = 0;

        private ToolbarSlotUI SelectedItem => _slots[_selectedItemIndex];

        /// <summary> ������������� �����.</summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            _slots[_selectedItemIndex].Select();

            Inventory.PlayerInventory.InventoryUpdated += RedrawToolbar;
        }

        /// <summary> ��� ������ �������������� ���������.</summary>
        private void Start()
        {
            RedrawToolbar();
        }

        /// <summary> ������������ ������ ������������.</summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots)
            {
                slot.Redraw();
            }
        }

        /// <summary> ������� ������� �� ������.</summary>
        /// <param name="index"> ������ ��������, ������� ����� �������.</param>
        public void SelectItem(int index)
        {
            _slots[_selectedItemIndex].ResetSelection();

            _selectedItemIndex = index;
            _slots[_selectedItemIndex].Select();
        }

        /// <summary> ����������� ������������ ������� � ��������� �����.</summary>
        /// <remarks> ���� ������� �����������, �� ���� ��������� ����� ���������.</remarks>
        /// <returns> ���������� False, ���� �������� �� ������� ���������.</returns>
        public bool TryUseSelectedItem()
        {
            if (SelectedItem.GetItem() is ActionItem actionItem)
            {
                actionItem.Use();
                if (actionItem.IsConsumable)
                {
                    Inventory.PlayerInventory.RemoveFromSlot(_selectedItemIndex, 1);
                    print($"{this} �����������");
                }
                return true;
            }
            return false;
        }
    }
}