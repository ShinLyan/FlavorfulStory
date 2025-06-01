using System;
using FlavorfulStory.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> UI-компонент для отображения сводки или результатов с возможностью продолжения. </summary>
    public class SummaryView : MonoBehaviour
    {
        /// <summary> Основной контейнер UI сводки. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Текстовый элемент для отображения сводной информации. </summary>
        [SerializeField] private TMP_Text _summaryText;

        /// <summary> Кнопка для продолжения после просмотра сводки. </summary>
        [SerializeField] private Button _continueButton;

        /// <summary> Событие, вызываемое при нажатии кнопки продолжения. </summary>
        public Action OnContinuePressed;

        /// <summary> Подписка на нажаьте кнопки. </summary>
        private void Awake()
        {
            _continueButton.onClick.AddListener(() => OnContinuePressed?.Invoke());
        }

        /// <summary> Устанавливает текст сводки. </summary>
        /// <param name="text">Текст для отображения в сводке.</param>
        public void SetSummary(string text)
        {
            _summaryText.text = text;
        }

        /// <summary> Показывает UI сводки и блокирует пользовательский ввод. </summary>
        public void Show()
        {
            _content.SetActive(true);
            InputWrapper.BlockAllInput();
        }

        /// <summary> Скрывает UI сводки и разблокирует пользовательский ввод. </summary>
        public void Hide()
        {
            _content.SetActive(false);
            InputWrapper.UnblockAllInput();
        }
    }
}