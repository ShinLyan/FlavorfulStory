using System;
using System.Collections.Generic;
using DG.Tweening;
using FlavorfulStory.Infrastructure.Factories;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Управление отображением инвентаря в пользовательском интерфейсе. </summary>
    public class InventoryView : MonoBehaviour
    {
        /// <summary> Инвентарь. </summary>
        private Inventory _inventory;

        /// <summary> Фабрика для создания отображений ячеек инвентаря. </summary>
        private IPrefabFactory<InventorySlotView> _slotFactory;

        /// <summary> Список отображений ячеек инвентаря. </summary>
        private readonly List<InventorySlotView> _slots = new();

        /// <summary> Контейнер для размещения отображений ячеек. </summary>
        private Transform _slotsContainer;
        
        /// <summary> Флаг инициализации вьюхи. </summary>
        private bool _initialized;
        
        /// <summary> Внедрить зависимости: инвентарь и фабрику отображений ячеек. </summary>
        /// <param name="slotFactory"> Фабрика отображений ячеек. </param>
        [Inject]
        private void Construct(IPrefabFactory<InventorySlotView> slotFactory) => _slotFactory = slotFactory;
        
        /// <summary> Переинициализирует отображение для указанного инвентаря. </summary>
        public void Initialize(Inventory inventory)
        {
            if (_inventory == inventory && _initialized)
                return; 
            
            if (_inventory) _inventory.InventoryUpdated -= UpdateView;
            _inventory = inventory;
            
            if (!_initialized)
            {
                _initialized = true;
                _slotsContainer = transform;
                CacheInitialSlots();
            }

            _inventory.InventoryUpdated += UpdateView;
            UpdateView();
        }
        
        /// <summary> Сохранить существующие отображения ячеек, если они уже присутствуют в иерархии. </summary>
        private void CacheInitialSlots()
        {
            foreach (Transform child in _slotsContainer)
            {
                var slot = child.GetComponent<InventorySlotView>();
                if (!slot || _slots.Contains(slot)) continue;

                slot.gameObject.SetActive(false);
                _slots.Add(slot);
            }
        }

        /// <summary> Отписаться от событий и очистить отображения при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            if (_inventory != null) _inventory.InventoryUpdated -= UpdateView;
            ClearView();
        }

        /// <summary> Обновить отображения ячеек в соответствии с данными инвентаря. </summary>
        private void UpdateView()
        {
            
            for (int i = 0; i < _inventory.InventorySize; i++)
            {
                var slot = EnsureSlot(i);
                UpdateSlotView(slot, i);
            }

            DisableExcessSlots(_inventory.InventorySize);
        }

        /// <summary> Получить или создать отображение ячейки по индексу. </summary>
        /// <param name="index"> Индекс ячейки. </param>
        /// <returns> Отображение ячейки инвентаря. </returns>
        private InventorySlotView EnsureSlot(int index)
        {
            if (index < _slots.Count) return _slots[index];

            var slot = _slotFactory.Create(parentTransform: _slotsContainer);
            _slots.Add(slot);
            return slot;
        }

        /// <summary> Обновить отображение ячейки по индексу. </summary>
        /// <param name="slot"> Отображение ячейки. </param>
        /// <param name="index"> Индекс слота. </param>
        private void UpdateSlotView(InventorySlotView slot, int index)
        {
            slot.gameObject.SetActive(true);
            slot.Setup(index, _inventory);
        }

        /// <summary> Отключить отображения ячеек, превышающие активный размер инвентаря. </summary>
        /// <param name="activeCount"> Количество активных ячеек. </param>
        private void DisableExcessSlots(int activeCount)
        {
            for (int i = activeCount; i < _slots.Count; i++) _slots[i].gameObject.SetActive(false);
        }

        /// <summary> Очистить представление. </summary>
        private void ClearView()
        {
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
                _slotFactory.Despawn(slot);
            }

            _slots.Clear();
        }

        /// <summary> Анимировать добавление предмета в инвентарь (визуальное подчёркивание появления). </summary>
        /// <param name="item"> Предмет, который был добавлен. </param>
        public void AnimateAdd(InventoryItem item)
        {
            if (!TryFindSlotView(item, out var slotView)) return;

            var itemStackView = slotView.GetComponentInChildren<ItemStackView>();
            var rect = itemStackView.GetComponent<RectTransform>();

            rect.DOKill();
            rect.anchoredPosition = Vector2.zero;
            rect.DOShakeAnchorPos(0.5f, Vector2.one, 20, 0f, false, false);
        }

        /// <summary> Найти вьюху слота по предмету. </summary>
        /// <param name="item"> Предмет, который нужно найти. </param>
        /// <param name="slotView"> Найденное отображение слота. </param>
        /// <returns> Найден ли слот. </returns>
        private bool TryFindSlotView(InventoryItem item, out InventorySlotView slotView)
        {
            foreach (Transform child in transform)
            {
                slotView = child.GetComponent<InventorySlotView>();
                if (slotView && slotView.GetItem() == item) return true;
            }

            slotView = null;
            return false;
        }

        /// <summary> Анимировать удаление предмета из слота (визуальное затухание и сдвиг вверх). </summary>
        /// <param name="slotIndex"> Индекс слота, из которого удаляется предмет. </param>
        /// <param name="onComplete"> Действие, вызываемое после завершения анимации. </param>
        public void AnimateRemoveAt(int slotIndex, Action onComplete)
        {
            var slotView = _slots[slotIndex];
            var itemStackView = slotView.GetComponentInChildren<ItemStackView>();
            if (!itemStackView) return;

            var rect = itemStackView.GetComponent<RectTransform>();
            var canvasGroup = itemStackView.GetComponent<CanvasGroup>()
                              ?? itemStackView.gameObject.AddComponent<CanvasGroup>();

            rect.DOKill();
            canvasGroup.DOKill();

            canvasGroup.alpha = 1f;
            rect.anchoredPosition = Vector2.zero;

            rect.DOAnchorPosY(rect.anchoredPosition.y + 20f, 0.5f).SetEase(Ease.InOutQuad);
            canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                canvasGroup.alpha = 1f;
                rect.anchoredPosition = Vector2.zero;
                onComplete?.Invoke();
            });
        }
    }
}