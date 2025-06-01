using System;
using FlavorfulStory.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Представляет UI-компонент для отображения диалога подтверждения с кнопками "Да" и "Нет". </summary>
    public class ConfirmationView : MonoBehaviour
    {
        /// <summary> Основной контейнер диалога подтверждения. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Текстовый элемент для отображения заголовка диалога. </summary>
        [SerializeField] private TMP_Text _titleText;

        /// <summary> Текстовый элемент для отображения описания или вопроса. </summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Кнопка для подтверждения действия ("Да"). </summary>
        [SerializeField] private Button _yesButton;

        /// <summary> Кнопка для отмены действия ("Нет"). </summary>
        [SerializeField] private Button _noButton;

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
            _content.SetActive(true);
            InputWrapper.BlockAllInput();
        }

        /// <summary> Скрывает диалог подтверждения. </summary>
        public void Hide()
        {
            _content.SetActive(false);
            InputWrapper.UnblockAllInput();
        }
    }
}