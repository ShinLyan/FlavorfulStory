using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов. </summary>
    public class Toolbar : MonoBehaviour
    {
        /// <summary> Массив слотов панели инструментов. </summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> Индекс выбранного предмета. </summary>
        public int SelectedItemIndex { get; private set; }

        /// <summary> Выбранный предмет. </summary>
        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Инициализация полей. </summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            Inventory.PlayerInventory.InventoryUpdated += RedrawToolbar;
            foreach (var slot in _slots)
            {
                slot.OnSlotClicked += SelectItem;
            }
        }

        /// <summary> При старте перерисовываем инвентарь. </summary>
        private void Start()
        {
            ResetToolbar();
            RedrawToolbar();
            _slots[SelectedItemIndex].Select();
        }

        /// <summary> Обрабатываем ввод колесика мыши. </summary>
        private void Update()
        {
            HandleMouseScrollInput();
        }

        /// <summary> Снять выделение со всех слотов тулбара. </summary>
        private void ResetToolbar()
        {
            foreach (var slot in _slots)
            {
                slot.ResetSelection();
            }
        }
        
        /// <summary> Перерисовать панель инструментов. </summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();
        }

        /// <summary> Выбрать предмет на панели. </summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать. </param>
        public void SelectItem(int index)
        {
            _slots[SelectedItemIndex].ResetSelection();
            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();
        }
        
        /// <summary> Обработка ввода колесика мыши. </summary>
        private void HandleMouseScrollInput()
        {
            var scrollInput = (int)Input.mouseScrollDelta.y;

            if (scrollInput == 0) return;
            
            var newSelectedItemIndex = Mathf.Clamp(SelectedItemIndex - scrollInput, 0, _slots.Length - 1);
            SelectItem(newSelectedItemIndex);
        }
    }
}