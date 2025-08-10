using System;
using FlavorfulStory.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Отображение сводки дня. </summary>
    public class SummaryView : MonoBehaviour
    {
        /// <summary> Основной контейнер UI сводки. </summary>
        [SerializeField] private GameObject _content; 

        /// <summary> Текстовый элемент для отображения сводной информации. </summary>
        [SerializeField] private TMP_Text _summaryText;

        /// <summary> Кнопка для продолжения после просмотра сводки. </summary>
        [SerializeField] private Button _continueButton;

        /// <summary> Камера, используемая для отображения сводки (активируется при показе UI). </summary>
        [SerializeField] private GameObject _camera;

        /// <summary> Текст сводки по умолчанию, отображается при отсутствии специального контента. </summary>
        public const string DefaultSummaryText = "BEST SUMMARY EVER";

        /// <summary> Событие, вызываемое при нажатии кнопки продолжения. </summary>
        public Action OnContinuePressed;

        /// <summary> Подписка на нажатие кнопки. </summary>
        private void Awake() => _continueButton.onClick.AddListener(() => OnContinuePressed?.Invoke());

        /// <summary> Устанавливает текст сводки. </summary>
        /// <param name="text">Текст для отображения в сводке.</param>
        public void SetSummary(string text) => _summaryText.text = text;

        /// <summary> Показывает UI сводки и блокирует пользовательский ввод. </summary>
        public void Show()
        {
            _content.SetActive(true);
            _camera.SetActive(true);
            InputWrapper.BlockAllInput();
        }

        /// <summary> Скрывает UI сводки и разблокирует пользовательский ввод. </summary>
        public void Hide()
        {
            _content.SetActive(false);
            _camera.SetActive(false);
            InputWrapper.UnblockAllInput();
        }
    }
}