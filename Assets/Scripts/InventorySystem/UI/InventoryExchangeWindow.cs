using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            _addToExistingButton.onClick.AddListener(() => 
                TransferStackablesToExisting(_playerInventory, _otherInventory));
            
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

        /// <summary> Переносит все стакающиеся предметы из одного инвентаря в другой, если они уже есть в целевом. </summary>
        private void TransferStackablesToExisting(Inventory from, Inventory to)
        {
            for (int i = 0; i < from.InventorySize; i++)
            {
                var item = from.GetItemInSlot(i);
                var count = from.GetNumberInSlot(i);
                if (item == null || count <= 0 || !item.IsStackable) continue;

                if (to.HasItem(item) && to.HasSpaceFor(item))
                {
                    if (to.TryAddToFirstAvailableSlot(item, count))
                    {
                        int slotIndex = i;
                        AnimateRemovedItem(item, _playerInventoryView, () =>
                        {
                            from.RemoveFromSlot(slotIndex, count);
                        });

                        ShakeTransferredItem(item, _otherInventoryView);
                    }
                }
            }
        }
        
        /// <summary> Потряхивает UI-слот, в который был перенесён предмет. </summary>
        private void ShakeTransferredItem(InventoryItem item, InventoryView view)
        {
            foreach (Transform child in view.transform)
            {
                var slotView = child.GetComponent<InventorySlotView>();
                if (slotView != null && slotView.GetItem() == item)
                {
                    var itemStackView = slotView.GetComponentInChildren<ItemStackView>();
                    var rect = itemStackView.GetComponent<RectTransform>();

                    rect.DOKill();
                    rect.anchoredPosition = Vector2.zero;

                    rect.DOShakeAnchorPos(
                        duration: 0.5f,
                        strength: Vector2.one,
                        vibrato: 20,
                        randomness: 0f,
                        snapping: false,
                        fadeOut: false,
                        randomnessMode: ShakeRandomnessMode.Full
                    );
                }
            }
        }
        
        /// <summary> Анимирует исчезновение предмета из UI-слота (сдвиг вверх + fade-out). </summary>
        /// <summary> Анимирует исчезновение предмета из UI-слота (сдвиг вверх + fade-out), затем вызывает onComplete. </summary>
        private void AnimateRemovedItem(InventoryItem item, InventoryView view, Action onComplete)
        {
            foreach (Transform child in view.transform)
            {
                var slotView = child.GetComponent<InventorySlotView>();
                if (slotView != null && slotView.GetItem() == item)
                {
                    var itemStackView = slotView.GetComponentInChildren<ItemStackView>();
                    var rect = itemStackView.GetComponent<RectTransform>();
                    var canvasGroup = itemStackView.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        canvasGroup = itemStackView.gameObject.AddComponent<CanvasGroup>();

                    rect.DOKill();
                    canvasGroup.DOKill();

                    canvasGroup.alpha = 1f;
                    
                    rect.DOAnchorPosY(rect.anchoredPosition.y + 20f, 0.5f)
                        .SetEase(Ease.InOutQuad);

                    canvasGroup.DOFade(0f, 0.5f)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() => onComplete?.Invoke());
                }
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