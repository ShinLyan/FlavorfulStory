using FlavorfulStory.InputSystem;
using FlavorfulStory.Saving;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Панель инструментов, отображающая и управляющая экипированными предметами. </summary>
    /// <remarks> Данный класс обрабатывает выбор предметов, ввод мыши и поддерживает
    /// визуальное состояние слотов панели инструментов. </remarks>
    public class Toolbar : MonoBehaviour, ISaveable
    {
        /// <summary> Массив слотов панели инструментов. </summary>
        private ToolbarSlotView[] _slots;

        /// <summary> Можно ли взаимодействовать? </summary>
        private bool _isInteractable;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Шина сигналов для оповещения других компонентов. </summary>
        private SignalBus _signalBus;

        /// <summary> Индекс выбранного предмета. </summary>
        public int SelectedItemIndex { get; private set; }

        /// <summary> Выбранный предмет. </summary>
        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="signalBus"> Сигнальная шина Zenject. </param>
        [Inject]
        private void Construct(Inventory inventory, SignalBus signalBus)
        {
            _playerInventory = inventory;
            _signalBus = signalBus;
        }

        /// <summary> Инициализация полей и подписка на события слотов панели. </summary>
        private void Awake()
        {
            _slots = GetComponentsInChildren<ToolbarSlotView>();
            _isInteractable = true;

            _playerInventory.InventoryUpdated += RedrawToolbar;
            foreach (var slot in _slots) slot.OnSlotClicked += SelectItem;
        }

        /// <summary> Обновляет визуальное представление всех слотов панели инструментов. </summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();

            _signalBus.Fire(new ToolbarSlotSelectedSignal { SelectedItem = SelectedItem });
        }

        /// <summary> Первоначальная настройка панели инструментов. </summary>
        private void Start()
        {
            ResetToolbar();
            RedrawToolbar();
            _slots[SelectedItemIndex].Select();
        }

        /// <summary> Сбрасывает состояние выделения всех слотов панели инструментов. </summary>
        private void ResetToolbar()
        {
            foreach (var slot in _slots) slot.ResetSelection();
        }

        /// <summary> Обрабатывает ввод колесика мыши в каждом кадре. </summary>
        private void Update() => HandleMouseScrollInput();

        /// <summary> Обрабатывает ввод колесика мыши для смены выбранного предмета. </summary>
        private void HandleMouseScrollInput()
        {
            int scrollInput = InputWrapper.GetMouseScrollDelta();

            if (scrollInput == 0) return;

            int newSelectedItemIndex = Mathf.Clamp(SelectedItemIndex - scrollInput, 0, _slots.Length - 1);
            SelectItem(newSelectedItemIndex);
        }

        /// <summary> Выбирает предмет в панели инструментов по указанному индексу. </summary>
        /// <param name="index"> Индекс предмета, который нужно выбрать. </param>
        public void SelectItem(int index)
        {
            if (!_isInteractable || index == SelectedItemIndex) return;

            _slots[SelectedItemIndex].ResetSelection();
            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();

            _signalBus.Fire(new ToolbarSlotSelectedSignal { SelectedItem = SelectedItem });
        }

        /// <summary> Установить состояние взаимодействия. </summary>
        /// <param name="state"> Состояние взаимодействия. </param>
        public void SetInteractableState(bool state) => _isInteractable = state;

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