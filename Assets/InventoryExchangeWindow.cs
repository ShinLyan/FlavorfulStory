using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class InventoryExchangeWindow : MonoBehaviour
    {
        [SerializeField] private InventoryView _leftView;
        [SerializeField] private InventoryView _rightView;
        
        private Action _onClose;

        [Inject] private IGameFactory<InventorySlotView> _slotFactory;

        public void Show(Inventory first, Inventory second, Action onClose)
        {
            gameObject.SetActive(true);
            _onClose = onClose;

            _leftView.Initialize(first, _slotFactory);
            _rightView.Initialize(second, _slotFactory);

            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();

            _onClose?.Invoke();
            _onClose = null;
        }

        private void Update()
        {
            if (!InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Hide();
            BlockGameMenuForOneFrame().Forget();
        }

        private static async UniTaskVoid BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            await UniTask.Yield();
            InputWrapper.UnblockAllInput();
        }
    }
}