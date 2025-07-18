using FlavorfulStory.Actions;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Saving;
using FlavorfulStory.TooltipSystem;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов, отображающая и управляющая экипированными предметами. </summary>
    /// <remarks> Данный класс обрабатывает выбор предметов, ввод мыши и поддерживает
    /// визуальное состояние слотов панели инструментов. </remarks>
    public class Toolbar : MonoBehaviour, ISaveable
    {
        /// <summary> Последний предмет, по коотороому показываем тултип. </summary>
        private InventoryItem _lastTooltipItem;

        /// <summary> Массив слотов панели инструментов. </summary>
        private ToolbarSlotView[] _slots;

        /// <summary> Можно ли взаимодействовать? </summary>
        private bool _isInteractable;

        /// <summary> Индекс выбранного предмета. </summary>
        public int SelectedItemIndex { get; private set; }

        /// <summary> Выбранный предмет. </summary>
        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Показчик тултипов. </summary>
        private IActionTooltipShower _tooltipShower;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="tooltipShower"> Показчик тултипов. </param>
        [Inject]
        private void Construct(Inventory inventory, IActionTooltipShower tooltipShower)
        {
            _playerInventory = inventory;
            _tooltipShower = tooltipShower;
        }

        /// <summary> Инициализация полей и подписка на события тулбар слотов. </summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotView>();
            _isInteractable = true;

            _playerInventory.InventoryUpdated += RedrawToolbar;
            foreach (var slot in _slots) slot.OnSlotClicked += SelectItem;
        }

        /// <summary> Первоначальная настройка панели инструментов. </summary>
        private void Start()
        {
            ResetToolbar();
            RedrawToolbar();
            _slots[SelectedItemIndex].Select();
        }

        /// <summary> Обрабатывает ввод колесика мыши в каждом кадре. </summary>
        private void Update() => HandleMouseScrollInput();

        /// <summary> Установить состояние взаимодействия. </summary>
        /// <param name="state"> Состояние взаимодействия. </param>
        public void SetInteractableState(bool state) => _isInteractable = state;

        /// <summary> Сбрасывает состояние выделения всех слотов панели инструментов. </summary>
        private void ResetToolbar()
        {
            foreach (var slot in _slots) slot.ResetSelection();
        }

        /// <summary> Выбирает предмет в панели инструментов по указанному индексу. </summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать. </param>
        public void SelectItem(int index)
        {
            if (!_isInteractable) return;

            _slots[SelectedItemIndex].ResetSelection();
            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();

            ShowItemTooltip();
        }

        /// <summary> Обновляет визуальное представление всех слотов панели инструментов. </summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();

            ShowItemTooltip();
        }

        /// <summary> Обрабатывает ввод колесика мыши для смены выбранного предмета. </summary>
        private void HandleMouseScrollInput()
        {
            int scrollInput = InputWrapper.GetMouseScrollDelta();

            if (scrollInput == 0) return;

            int newSelectedItemIndex = Mathf.Clamp(SelectedItemIndex - scrollInput, 0, _slots.Length - 1);
            SelectItem(newSelectedItemIndex);
        }

        /// <summary> Показать тултип для выбранного предмета. </summary>
        private void ShowItemTooltip()
        {
            if (_lastTooltipItem && _lastTooltipItem.CanBeDropped)
            {
                var oldTooltip = new ActionTooltipData("G", ActionType.Drop, _lastTooltipItem.ItemName);
                _tooltipShower.Remove(oldTooltip);
            }

            var item = SelectedItem;
            if (item && item.CanBeDropped)
            {
                var newTooltip = new ActionTooltipData("G", ActionType.Drop, item.ItemName);
                _tooltipShower.Add(newTooltip);
                _lastTooltipItem = item;
            }
            else
            {
                _lastTooltipItem = null;
            }
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