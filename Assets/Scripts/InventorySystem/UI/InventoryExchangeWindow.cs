using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using InputWrapper = FlavorfulStory.InputSystem.InputWrapper;

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

        /// <summary> Фабрика для создания слотов в инвентаре. </summary>
        [Inject] private IGameFactory<InventorySlotView> _slotFactory;

        [Inject] private InventoryTransferService _transferService;        
        
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
            _onClose = onClose;
            
            _otherInventory = otherInventory;
            _otherInventoryView.Initialize(otherInventory, _slotFactory);
            _playerInventory = playerInventory;
            _playerInventoryView.Initialize(playerInventory, _slotFactory);
            
            _addToExistingButton.onClick.AddListener(OnAddToExistingClicked);
            
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Закрыть окно и вернуть управление игроку. </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();

            _onClose?.Invoke();
            _onClose = null;
            _addToExistingButton.onClick.RemoveAllListeners();
        }

        private void OnAddToExistingClicked()
        {
            var stackables = _transferService.GetStackablesToTransfer(_playerInventory, _otherInventory);
            foreach (var (slotIndex, stack) in stackables)
            {
                _playerInventoryView.AnimateRemoveAt(slotIndex, () =>
                {
                    _playerInventory.RemoveFromSlot(slotIndex, stack.Number);
                });
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