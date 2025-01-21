using FlavorfulStory.Input;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов, отображающая и управляющая экипированными предметами. </summary>
    /// <remarks> Данный класс обрабатывает выбор предметов, ввод мыши и поддерживает визуальное состояние слотов панели инструментов. </remarks>
    public class Toolbar : MonoBehaviour, ISaveable
    {
        /// <summary> Массив слотов панели инструментов. </summary>
        private ToolbarSlotUI[] _slots;

        /// <summary> Индекс выбранного предмета. </summary>
        public int SelectedItemIndex { get; private set; }

        /// <summary> Выбранный предмет. </summary>
        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Инициализация полей и подписка на события тулбар слотов. </summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotUI>();
            Inventory.PlayerInventory.InventoryUpdated += RedrawToolbar;
            foreach (var slot in _slots)
            {
                slot.OnSlotClicked += SelectItem;
            }
        }

        /// <summary> Первоначальная настройка панели инструментов. </summary>
        private void Start()
        {
            ResetToolbar();
            RedrawToolbar();
            _slots[SelectedItemIndex].Select();
        }

        /// <summary> Обрабатывает ввод колесика мыши в каждом кадре. </summary>
        private void Update()
        {
            HandleMouseScrollInput();
        }

        /// <summary> Сбрасывает состояние выделения всех слотов панели инструментов. </summary>
        private void ResetToolbar()
        {
            foreach (var slot in _slots)
            {
                slot.ResetSelection();
            }
        }

        /// <summary> Обновляет визуальное представление всех слотов панели инструментов. </summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();
        }

        /// <summary> Выбирает предмет в панели инструментов по указанному индексу. </summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать. </param>
        public void SelectItem(int index)
        {
            _slots[SelectedItemIndex].ResetSelection();
            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();
        }

        /// <summary> Обрабатывает ввод колесика мыши для смены выбранного предмета. </summary>
        private void HandleMouseScrollInput()
        {
            var scrollInput = InputWrapper.GetMouseScrollDelta();

            if (scrollInput == 0) return;

            var newSelectedItemIndex = Mathf.Clamp(SelectedItemIndex - scrollInput, 0, _slots.Length - 1);
            SelectItem(newSelectedItemIndex);
        }

        #region Saving

        /// <summary> Сохраняет текущее состояние выбранного индекса. </summary>
        /// <returns> Индекс текущего выбранного предмета. </returns>
        public object CaptureState() => SelectedItemIndex;

        /// <summary> Восстанавливает сохраненное состояние выбранного индекса. </summary>
        /// <param name="state"> Сохраненный индекс предмета. </param>
        public void RestoreState(object state) => SelectedItemIndex = (int)state;

        #endregion
    }
}