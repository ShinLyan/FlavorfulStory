using System.Collections.Generic;
using FlavorfulStory.Infrastructure.Factories;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Управление отображением инвентаря в пользовательском интерфейсе. </summary>
    public class InventoryView : MonoBehaviour
    {
        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Фабрика для создания отображений ячеек инвентаря. </summary>
        private IGameFactory<InventorySlotView> _slotFactory;

        /// <summary> Список отображений ячеек инвентаря. </summary>
        private readonly List<InventorySlotView> _slots = new();

        /// <summary> Контейнер для размещения отображений ячеек. </summary>
        private Transform _slotsContainer;

        /// <summary> Инициализировать отображения и подписаться на обновление инвентаря. </summary>
        private void Awake()
        {
            _slotsContainer = transform;
            CacheInitialSlots();
        }

        /// <summary> Переинициализирует отображение для указанного инвентаря. </summary>
        public void Initialize(Inventory inventory, IGameFactory<InventorySlotView> slotFactory)
        {
            if (_playerInventory != null)
                _playerInventory.InventoryUpdated -= UpdateView;

            _playerInventory = inventory;
            _slotFactory = slotFactory;

            foreach (var slot in _slots)
                slot.Construct(_playerInventory);

            _playerInventory.InventoryUpdated += UpdateView;
            UpdateView();
        }
        
        /// <summary> Сохранить существующие отображения ячеек, если они уже присутствуют в иерархии. </summary>
        private void CacheInitialSlots()
        {
            foreach (Transform child in _slotsContainer)
            {
                var slot = child.GetComponent<InventorySlotView>();
                if (!slot || _slots.Contains(slot)) continue;

                slot.Construct(_playerInventory);
                slot.gameObject.SetActive(false);
                _slots.Add(slot);
            }
        }

        /// <summary> Выполнить первичное обновление отображения инвентаря. </summary>
        private void Start() => UpdateView();

        /// <summary> Обновить отображения ячеек в соответствии с данными инвентаря. </summary>
        private void UpdateView()
        {
            for (int i = 0; i < _playerInventory.InventorySize; i++)
            {
                var slot = EnsureSlot(i);
                UpdateSlotView(slot, i);
            }

            DisableExcessSlots(_playerInventory.InventorySize);
        }

        /// <summary> Получить или создать отображение ячейки по индексу. </summary>
        /// <param name="index"> Индекс ячейки. </param>
        /// <returns> Отображение ячейки инвентаря. </returns>
        private InventorySlotView EnsureSlot(int index)
        {
            if (index < _slots.Count) return _slots[index];

            var slot = _slotFactory.Create(_slotsContainer);
            slot.Construct(_playerInventory);
            _slots.Add(slot);
            return slot;
        }

        /// <summary> Обновить отображение ячейки по индексу. </summary>
        /// <param name="slot"> Отображение ячейки. </param>
        /// <param name="index"> Индекс слота. </param>
        private static void UpdateSlotView(InventorySlotView slot, int index)
        {
            slot.gameObject.SetActive(true);
            slot.Setup(index);
        }

        /// <summary> Отключить отображения ячеек, превышающие активный размер инвентаря. </summary>
        /// <param name="activeCount"> Количество активных ячеек. </param>
        private void DisableExcessSlots(int activeCount)
        {
            for (int i = activeCount; i < _slots.Count; i++) _slots[i].gameObject.SetActive(false);
        }

        /// <summary> Отписаться от событий и очистить отображения при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            _playerInventory.InventoryUpdated -= UpdateView;
            CleanupSlots();
        }
        
        /// <summary> Удалить все отображения ячеек и освободить ресурсы. </summary>
        private void CleanupSlots()
        {
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
                _slotFactory.Despawn(slot);
            }

            _slots.Clear();
        }
    }
}