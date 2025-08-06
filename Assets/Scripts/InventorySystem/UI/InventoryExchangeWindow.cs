using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Окно обмена между двумя инвентарями (игрок и хранилище). </summary>
    public class InventoryExchangeWindow : MonoBehaviour
    {
        /// <summary> View для отображения второго инвентаря (например, сундука). </summary>
        [SerializeField] private InventoryView _otherInventoryView;

        /// <summary> View для отображения инвентаря игрока. </summary>
        [SerializeField] private InventoryView _playerInventoryView;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _addToExistingButton;

        /// <summary> Второй инвентарь (например, сундук). </summary>
        private Inventory _otherInventory;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Колбэк, вызываемый при закрытии окна. </summary>
        private Action _onClose;

        /// <summary> Сервис для передачи предметов между инвентарями. </summary>
        private InventoryTransferService _transferService;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="transferService"> Сервис передачи предметов между инвентарями. </param>
        [Inject]
        private void Construct(InventoryTransferService transferService) => _transferService = transferService;

        /// <summary> Подписывает обработчик на кнопку переноса предметов. </summary>
        private void Awake() => _addToExistingButton.onClick.AddListener(OnAddToExistingClicked);

        /// <summary> Удаляет все подписки при уничтожении объекта. </summary>
        private void OnDestroy() => _addToExistingButton.onClick.RemoveAllListeners();

        /// <summary> Закрывает окно по нажатию кнопки выхода из меню (например, Escape). </summary>
        private void Update()
        {
            if (!InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Hide();
            BlockGameMenuForOneFrame().Forget();
        }

        /// <summary> Открыть окно обмена между двумя инвентарями. </summary>
        /// <param name="playerInventory"> Инвентарь игрока. </param>
        /// <param name="otherInventory"> Инвентарь объекта (сундук и тд.). </param>
        /// <param name="onClose"> Колбэк, вызываемый при закрытии окна. </param>
        public void Show(Inventory playerInventory, Inventory otherInventory, Action onClose)
        {
            gameObject.SetActive(true);

            _otherInventory = otherInventory;
            _playerInventory = playerInventory;

            _otherInventoryView.Initialize(otherInventory);
            _playerInventoryView.Initialize(playerInventory);

            _onClose = onClose;

            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Закрыть окно и вернуть управление игроку. </summary>
        public void Hide()
        {
            gameObject.SetActive(false);

            _otherInventory = null;
            _playerInventory = null;

            _onClose?.Invoke();
            _onClose = null;

            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
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

        /// <summary> Блокирует кнопку выхода из окна на один кадр (предотвращает повторное закрытие). </summary>
        private static async UniTaskVoid BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            await UniTask.Yield();
            InputWrapper.UnblockAllInput();
        }
    }
}