using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Окно обмена между двумя инвентарями (игрок и хранилище). </summary>
    public class InventoryExchangeWindow : BaseWindow
    {
        /// <summary> View для отображения второго инвентаря (например, сундука). </summary>
        [SerializeField] private InventoryView _otherInventoryView;

        /// <summary> View для отображения инвентаря игрока. </summary>
        [SerializeField] private InventoryView _playerInventoryView;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _addToExistingButton;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _closeButton;
        
        /// <summary> Второй инвентарь (например, сундук). </summary>
        private Inventory _otherInventory;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

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

        public void Setup(Inventory otherInventory, Inventory playerInventory)
        {
            _otherInventory = otherInventory;
            _playerInventory = playerInventory;

            _otherInventoryView.Initialize(otherInventory);
            _playerInventoryView.Initialize(playerInventory);
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            _otherInventory = null;
            _playerInventory = null;
        }

        /// <summary> Обрабатывает нажатие на кнопку "Add to Existing". </summary>
        /// <remarks> Переносит стакающиеся предметы из инвентаря игрока во второй инвентарь. </remarks>
        private void OnAddToExistingClicked()
        {
            var stackables = _transferService.GetStackablesToTransfer(_playerInventory, _otherInventory);
            foreach ((int slotIndex, var stack) in stackables)
            {
                _playerInventoryView.AnimateRemoveAt(slotIndex,
                    () => { _playerInventory.RemoveFromSlot(slotIndex, stack.Number); });
                _otherInventoryView.AnimateAdd(stack.Item);
            }
        }
    }
}