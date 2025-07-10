using System;
using DG.Tweening;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.UI.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Представляет UI-компонент для отображения окна подтверждения с кнопками "Да" и "Нет". </summary>
    [RequireComponent(typeof(CanvasGroupFader))]
    public class ConfirmationWindowView : MonoBehaviour
    {
        /// <summary> Текстовый элемент для отображения заголовка диалога. </summary>
        [SerializeField] private TMP_Text _titleText;

        /// <summary> Текстовый элемент для отображения описания или вопроса. </summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Кнопка для подтверждения действия ("Да"). </summary>
        [SerializeField] private Button _yesButton;

        /// <summary> Кнопка для отмены действия ("Нет"). </summary>
        [SerializeField] private Button _noButton;

        /// <summary> Компонент для управления плавным появлением и исчезновением UI через CanvasGroup. </summary>
        private CanvasGroupFader _fader;

        /// <summary> Инициализация компонентов. </summary>
        private void Awake() => _fader = GetComponent<CanvasGroupFader>();

        /// <summary> Настраивает диалог подтверждения с указанными параметрами. </summary>
        /// <param name="title"> Заголовок диалога. </param>
        /// <param name="description"> Описание или вопрос для подтверждения. </param>
        /// <param name="onYes"> Действие, выполняемое при нажатии кнопки "Да". </param>
        /// <param name="onNo"> Действие, выполняемое при нажатии кнопки "Нет". </param>
        public void Setup(string title, string description, Action onYes, Action onNo)
        {
            _titleText.text = title;
            _descriptionText.text = description;

            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();

            _yesButton.onClick.AddListener(() => onYes?.Invoke());
            _noButton.onClick.AddListener(() => onNo?.Invoke());
        }

        /// <summary> Показывает диалог подтверждения. </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            InputWrapper.BlockAllInput();
            WorldTime.Pause();

            _fader.Show();
        }

        /// <summary> Скрывает диалог подтверждения. </summary>
        public void Hide()
        {
            _fader.Hide().OnComplete(() =>
            {
                gameObject.SetActive(false);
                InputWrapper.UnblockAllInput();
                WorldTime.Unpause();
            });
        }
    }
}