using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> ������ ������������.</summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> ������ ������ ������ ������������.</summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> ������ ���������� ��������.</summary>
        public int SelectedItemIndex { get; private set; }

        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> ������������� �����.</summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            _slots[SelectedItemIndex].Select();

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
            foreach (var slot in _slots) slot.Redraw();
        }

        /// <summary> ������� ������� �� ������.</summary>
        /// <param name="index"> ������ ��������, ������� ����� �������.</param>
        public void SelectItem(int index)
        {
            _slots[SelectedItemIndex].ResetSelection();

            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();
        }
    }
}