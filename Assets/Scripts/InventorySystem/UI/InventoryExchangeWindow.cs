using FlavorfulStory.UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Окно обмена между двумя инвентарями (игрок и хранилище). </summary>
    public class InventoryExchangeWindow : BaseWindow
    {
        /// <summary> View для отображения второго инвентаря. </summary>
        [SerializeField] private InventoryView _secondInventoryView;

        /// <summary> View для отображения первого инвентаря. </summary>
        [SerializeField] private InventoryView _firstInventoryView;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _addToExistingButton;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _closeButton;
        
        /// <summary> Второй инвентарь . </summary>
        private Inventory _secondInventory;

        /// <summary> Первый инвентарь. </summary>
        private Inventory _firstInventory;

        /// <summary> Сервис для передачи предметов между инвентарями. </summary>
        private InventoryTransferService _transferService;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="transferService"> Сервис передачи предметов между инвентарями. </param>
        [Inject]
        private void Construct(InventoryTransferService transferService) => _transferService = transferService;

        /// <summary> Подписывает обработчик на кнопку переноса предметов. </summary>
        private void Awake()
        {
            _addToExistingButton.onClick.AddListener(OnAddToExistingClicked);
            _closeButton.onClick.AddListener(Close);
        }

        /// <summary> Удаляет все подписки при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            _addToExistingButton.onClick.RemoveListener(OnAddToExistingClicked);
            _closeButton.onClick.RemoveListener(Close);
        }

        /// <summary> Инициализирует окно двумя инвентарями и обновляет UI. </summary>
        public void Setup(Inventory secondInventory, Inventory firstInventory)
        {
            _secondInventory = secondInventory;
            _firstInventory = firstInventory;

            _secondInventoryView.Initialize(secondInventory);
            _firstInventoryView.Initialize(firstInventory);

            _secondInventory.InventoryUpdated += UpdateAddToExistingButton;
            _firstInventory.InventoryUpdated += UpdateAddToExistingButton;

            _addToExistingButton.interactable = CheckForDuplicates();
        }

        /// <summary> Обработка закрытия окна. Очищает ссылки и снимает подписки. </summary>
        protected override void OnClosed()
        {
            base.OnClosed();

            _secondInventory.InventoryUpdated -= UpdateAddToExistingButton;
            _firstInventory.InventoryUpdated -= UpdateAddToExistingButton;
            
            _secondInventory = null;
            _firstInventory = null;
        }

        /// <summary> Обрабатывает нажатие на кнопку "Add to Existing". </summary>
        /// <remarks> Переносит стакающиеся предметы из инвентаря игрока во второй инвентарь. </remarks>
        private void OnAddToExistingClicked()
        {
            var stackables = _transferService.GetStackablesToTransfer(_firstInventory, _secondInventory);
            foreach ((int slotIndex, var stack) in stackables)
            {
                _firstInventoryView.AnimateRemoveAt(slotIndex,
                    () => { _firstInventory.RemoveFromSlot(slotIndex, stack.Number); });
                _secondInventoryView.AnimateAdd(stack.Item);
            }
        }

        /// <summary> Обновляет активность кнопки "Add to Existing". </summary>
        private void UpdateAddToExistingButton() => _addToExistingButton.interactable = CheckForDuplicates();

        /// <summary> Проверяет наличие стакающихся предметов, которые можно перенести. </summary>
        private bool CheckForDuplicates()
        {
            if (_firstInventory == null || _secondInventory == null) 
                return false;

            for (int i = 0; i < _firstInventory.InventorySize; i++)
            {
                var stack = _firstInventory.GetItemStackInSlot(i);
                var item = stack.Item;
                
                if (!item || !item.IsStackable || stack.Number <= 0)
                    continue;
                
                if (_secondInventory.HasItem(item) && _secondInventory.HasSpaceFor(item))
                    return true;
            }
            
            return false;
        }
    }
}