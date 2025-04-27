using System;
using System.Collections;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Представляет отдельный слот на панели инструментов. </summary>
    /// <remarks> Управляет отображением предмета, обработкой нажатий и визуальной обратной связью. </remarks>
    public class ToolbarSlotUI : CustomButton, IItemHolder
    {
        /// <summary> Иконка предмета в инвентаре. </summary>
        [SerializeField] private InventoryItemIcon _icon;

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

        /// <summary> Инициализирует слот панели инструментов. </summary>
        protected override void Initialize()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = _index == 9 ? "0" : $"{_index + 1}";
            _defaultColor = ButtonImage.color;
        }

        /// <summary> Обрабатывает начало наведения курсора на слот. </summary>
        protected override void HoverStart() => _hoverImage.CrossFadeAlpha(1.0f, FadeDuration, true);

        /// <summary> Обрабатывает окончание наведения курсора на слот. </summary>
        protected override void HoverEnd() => _hoverImage.CrossFadeAlpha(0.0f, FadeDuration, true);

        /// <summary> Обрабатывает клик по слоту. </summary>
        protected override void Click()
        {
            base.Click();
            OnSlotClicked?.Invoke(_index);
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
        public void Redraw() => _icon.SetItem(
            Inventory.PlayerInventory.GetItemInSlot(_index),
            Inventory.PlayerInventory.GetNumberInSlot(_index)
        );

        /// <summary> Получает предмет, находящийся в данном слоте. </summary>
        /// <returns> Предмет инвентаря в текущем слоте. </returns>
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);

        /// <summary> Запускает плавное изменение цвета слота. </summary>
        /// <param name="color"> Целевой цвет. </param>
        private void FadeToColor(Color color) => StartCoroutine(FadeToColorCoroutine(color));

        /// <summary> Корутина для плавного изменения цвета слота. </summary>
        /// <param name="color"> Целевой цвет. </param>
        /// <returns> Перечислитель для корутины. </returns>
        private IEnumerator FadeToColorCoroutine(Color color)
        {
            var startColor = ButtonImage.color;
            float timeElapsed = 0;
            while (timeElapsed <= FadeDuration)
            {
                ButtonImage.color = Color.Lerp(startColor, color, timeElapsed / FadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            ButtonImage.color = color;
        }
    }
}