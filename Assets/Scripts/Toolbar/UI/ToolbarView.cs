using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Toolbar.UI
{
    /// <summary> Панель инструментов, отображающая и управляющая экипированными предметами. </summary>
    /// <remarks> Данный класс обрабатывает выбор предметов, ввод мыши и поддерживает
    /// визуальное состояние слотов панели инструментов. </remarks>
    public class ToolbarView : MonoBehaviour, ISaveable
    {
        /// <summary> Шина сигналов для оповещения других компонентов. </summary>
        private SignalBus _signalBus;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Массив слотов панели инструментов. </summary>
        private ToolbarSlotView[] _slots;

        /// <summary> Индекс выбранного предмета. </summary>
        public int SelectedItemIndex { get; private set; }

        /// <summary> Можно ли взаимодействовать? </summary>
        public bool IsInteractable { get; set; }

        private IInventoryProvider _inventoryProvider;
        
        /// <summary> Выбранный предмет. </summary>
        public InventoryItem SelectedItem => _slots[SelectedItemIndex].GetItem();

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="signalBus"> Сигнальная шина Zenject. </param>
        /// <param name="inventoryProvider"> Провайдер инвентарей. </param>
        [Inject]
        private void Construct(SignalBus signalBus, IInventoryProvider inventoryProvider, Inventory playerInventory)
        {
            _signalBus = signalBus;
            _inventoryProvider = inventoryProvider;
            _playerInventory = playerInventory;
        }

        //public void Initialize() => _playerInventory = _inventoryProvider.GetPlayerInventory();
        
        /// <summary> Инициализация полей и подписка на события слотов панели. </summary>
        private void Awake()
        {
            _signalBus.Subscribe<ToolbarHotkeyPressedSignal>(OnHotkeyPressed);
            _signalBus.Subscribe<ConsumeSelectedItemSignal>(OnConsumeSelected);

            _playerInventory.InventoryUpdated += RedrawToolbar;

            _slots = GetComponentsInChildren<ToolbarSlotView>();
            IsInteractable = true;
            foreach (var slot in _slots) slot.OnSlotClicked += SelectItem;
        }

        /// <summary> Обрабатывает сигнал нажатия горячей клавиши тулбара. </summary>
        /// <param name="signal"> Сигнал с индексом выбранного слота. </param>
        private void OnHotkeyPressed(ToolbarHotkeyPressedSignal signal) => SelectItem(signal.SlotIndex);

        /// <summary> Обрабатывает сигнал потребления текущего выбранного предмета. </summary>
        /// <param name="signal"> Сигнал с количеством единиц для потребления. </param>
        private void OnConsumeSelected(ConsumeSelectedItemSignal signal)
        {
            if (!IsInteractable) return;

            _playerInventory.RemoveFromSlot(SelectedItemIndex, signal.Amount);
            RedrawToolbar();
        }

        /// <summary> Первоначальная настройка панели инструментов. </summary>
        private void Start()
        {
            ResetToolbar();
            RedrawToolbar();
            _slots[SelectedItemIndex].Select();
        }

        private void OnDestroy() => _playerInventory.InventoryUpdated -= RedrawToolbar;

        /// <summary> Сбрасывает состояние выделения всех слотов панели инструментов. </summary>
        private void ResetToolbar()
        {
            foreach (var slot in _slots) slot.ResetSelection();
        }

        /// <summary> Обновляет визуальное представление всех слотов панели инструментов. </summary>
        private void RedrawToolbar()
        {
            foreach (var slot in _slots) slot.Redraw();

            _signalBus.Fire(new ToolbarSlotSelectedSignal(SelectedItem));
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
        private void SelectItem(int index)
        {
            if (!IsInteractable || index == SelectedItemIndex) return;

            _slots[SelectedItemIndex].ResetSelection();
            SelectedItemIndex = index;
            _slots[SelectedItemIndex].Select();

            _signalBus.Fire(new ToolbarSlotSelectedSignal(SelectedItem));
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