using System;
using DG.Tweening;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Представляет отдельный слот на панели инструментов. </summary>
    /// <remarks> Управляет отображением предмета, обработкой нажатий и визуальной обратной связью. </remarks>
    public class ToolbarSlotView : CustomButton, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [SerializeField] private ItemStackView _itemStackView;

        /// <summary> Текстовое поле для отображения клавиши быстрого доступа. </summary>
        [SerializeField] private TMP_Text _keyText;

        /// <summary> Изображение обводки тулбар слота. </summary>
        [SerializeField] private Image _hoverImage;

        /// <summary> Цвет слота по умолчанию. </summary>
        private Color _defaultColor;

        /// <summary> Цвет выбранного слота. </summary>
        private readonly Color _selectedColor = new(1.0f, 1.0f, 1.0f, 0.35f);

        /// <summary> Длительность анимации затухания в секундах. </summary>
        private const float FadeDuration = 0.05f;

        /// <summary> Индекс слота в панели инструментов. </summary>
        private int _index;

        /// <summary> Событие, возникающее при клике на слот. </summary>
        public event Action<int> OnSlotClicked;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Анимация затухания подсветки при наведении курсора. </summary>
        private Tween _hoverTween;

        /// <summary> Анимация изменения цвета слота при выборе или сбросе. </summary>
        private Tween _colorTween;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;

        /// <summary> Освобождает ресурсы и завершает активные анимации при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            _hoverTween?.Kill();
            _colorTween?.Kill();
        }

        /// <summary> Инициализирует слот панели инструментов. </summary>
        protected override void Initialize()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = _index == 9 ? "0" : $"{_index + 1}";
            _defaultColor = ButtonImage.color;
        }

        /// <summary> Обрабатывает начало наведения курсора на слот. </summary>
        protected override void HoverStart() => FadeHoverImage(1f);

        /// <summary> Обрабатывает окончание наведения курсора на слот. </summary>
        protected override void HoverEnd() => FadeHoverImage(0f);

        /// <summary> Обрабатывает клик по слоту. </summary>
        protected override void Click()
        {
            base.Click();
            OnSlotClicked?.Invoke(_index);
        }

        /// <summary> Выполняет анимацию появления/исчезновения подсветки при наведении курсора. </summary>
        /// <param name="targetAlpha"> Целевое значение прозрачности (0 — невидимо, 1 — полностью видно). </param>
        private void FadeHoverImage(float targetAlpha)
        {
            _hoverTween?.Kill();
            _hoverTween = _hoverImage.DOFade(targetAlpha, FadeDuration).SetUpdate(true);
        }

        /// <summary> Выполняет анимацию изменения цвета фона слота. </summary>
        /// <param name="color"> Целевой цвет. </param>
        private void FadeToColor(Color color)
        {
            _colorTween?.Kill();
            _colorTween = ButtonImage.DOColor(color, FadeDuration).SetUpdate(true);
        }

        /// <summary> Выделяет данный слот. </summary>
        public void Select() => FadeToColor(_selectedColor);

        /// <summary> Сбрасывает выделение слота. </summary>
        public void ResetSelection()
        {
            FadeToColor(_defaultColor);
            if (!IsMouseOver) HoverEnd();
        }

        /// <summary> Обновляет отображение предмета в слоте. </summary>
        public void Redraw() => _itemStackView.UpdateView(_playerInventory.GetItemStackInSlot(_index));

        /// <summary> Получает предмет, находящийся в данном слоте. </summary>
        /// <returns> Предмет инвентаря в текущем слоте. </returns>
        public InventoryItem GetItem() => _playerInventory.GetItemInSlot(_index);
    }
}