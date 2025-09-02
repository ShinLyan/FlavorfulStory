using System.Collections.Generic;
using FlavorfulStory.Windows.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Окно обмена между двумя инвентарями (игрок и хранилище). </summary>
    public class InventoryExchangeWindow : BaseWindow
    {
        /// <summary> Отображение инвентаря сундука. </summary>
        [SerializeField] private InventoryView _chestInventoryView;

        /// <summary> Отображение инвентаря игрока. </summary>
        [SerializeField] private InventoryView _playerInventoryView;

        /// <summary> Кнопка "Add to Existing" — переносит стакающиеся предметы. </summary>
        [SerializeField] private Button _addToExistingButton;

        /// <summary> Кнопка закрытия окна. </summary>
        [SerializeField] private Button _closeButton;

        /// <summary> Первый инвентарь. </summary>
        private Inventory _chestInventory;

        /// <summary> Второй инвентарь. </summary>
        private Inventory _playerInventory;

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
        public void Setup(Inventory chestInventory, Inventory playerInventory)
        {
            _chestInventory = chestInventory;
            _playerInventory = playerInventory;

            _chestInventoryView.Initialize(_chestInventory);
            _playerInventoryView.Initialize(_playerInventory);
        }

        /// <summary> Обработка закрытия окна. </summary>
        /// <remarks> Очищает ссылки и снимает подписки. "Освобождает" игрока. </remarks>
        protected override void OnClosed()
        {
            _playerInventory = null;
            _chestInventory = null;
        }

        /// <summary> Обрабатывает нажатие на кнопку "Add to Existing". </summary>
        /// <remarks> Переносит стакающиеся предметы из инвентаря игрока во второй инвентарь. </remarks>
        private void OnAddToExistingClicked() // TODO: ТУТ БАГА С АНИМКОЙ И ЕЩЕ ЧЕМ-то
        {
            var stackables = GetStackablesToTransfer();
            foreach ((int slotIndex, var stack) in stackables)
            {
                _playerInventoryView.AnimateRemoveAt(slotIndex,
                    () => _playerInventory.RemoveFromSlot(slotIndex, stack.Number));
                _chestInventoryView.AnimateAdd(stack.Item);
            }
        }

        /// <summary> Получить список предметов, которые можно перенести из инвентаря игрока в сундук. </summary>
        /// <returns> Список пар (индекс слота, предмет), которые были успешно добавлены. </returns>
        private List<(int SlotIndex, ItemStack Stack)> GetStackablesToTransfer()
        {
            var result = new List<(int, ItemStack)>();
            for (int i = 0; i < _playerInventory.InventorySize; i++)
            {
                var itemStack = _playerInventory.GetItemStackInSlot(i);
                var item = itemStack.Item;
                int number = itemStack.Number;

                if (!item || !item.IsStackable || itemStack.Number <= 0 || !_chestInventory.HasItem(item) ||
                    !_chestInventory.HasSpaceFor(item))
                    continue;

                if (_chestInventory.TryAddToFirstAvailableSlot(item, number)) result.Add((i, itemStack));
                // TODO: У тебя метод называется Get - а ты еще тут добавляешь в слоты. Вытаскивай отсюда
            }

            return result;
        }
    }
}