using System;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> ������ ������������.</summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> ������ ������ ������ ������������.</summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> Количество тулбар слотов. </summary>
        private int _toolbarSize;
        
        /// <summary> ������ ���������� ��������.</summary>
        public int SelectedItemIndex { get; private set; }

        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> ������������� �����.</summary>
        private void Awake()
        {
            var toolbarSlotsUI = GetComponentsInChildren<ToolbarSlotUI>(); 
            _slots = toolbarSlotsUI;
            _toolbarSize = toolbarSlotsUI.Length;
            _slots[SelectedItemIndex].Select();

            Inventory.PlayerInventory.InventoryUpdated += RedrawToolbar;
        }

        /// <summary> ��� ������ �������������� ���������.</summary>
        private void Start()
        {
            RedrawToolbar();
        }

        /// <summary> Коллбек из UnityAPI. </summary>
        private void Update()
        {
            HandleMouseScrollInput();
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
        
        /// <summary> Метод обработки ввода колесика мыши. </summary>
        private void HandleMouseScrollInput()
        {
            var scrollInput = (int) Input.mouseScrollDelta.y;
            var newSelectedItemIndex = Math.Clamp(SelectedItemIndex - scrollInput, 0, _toolbarSize - 1);
            SelectItem(newSelectedItemIndex);
        }
    }
}