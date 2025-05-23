﻿using System;
using System.Collections.Generic;
using FlavorfulStory.AI;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Отображение диалогового окна. </summary>
    public class DialogueView : MonoBehaviour
    {
        #region Fields

        /// <summary> Текстовое поле для отображения текста диалога. </summary>
        [Tooltip("Текстовое поле для отображения текста диалога."), SerializeField]
        private TMP_Text _dialogueText;

        /// <summary> Текстовое поле для имени говорящего персонажа. </summary>
        [Header("Speaker Info")] [Tooltip("Текстовое поле для имени говорящего персонажа."), SerializeField]
        private TMP_Text _speakerName;

        /// <summary> Иконка, показывающая, что персонаж доступен для романтики. </summary>
        [Tooltip("Иконка, показывающая, что персонаж доступен для романтики."), SerializeField]
        private Image _romanceableIcon;

        /// <summary> Превью персонажа (например, портрет или модель). </summary>
        [Tooltip("Превью персонажа (например, портрет или модель)."), SerializeField]
        private GameObject _speakerPreview;

        /// <summary> Контейнер для кнопок вариантов ответа. </summary>
        [Header("Choices")] [Tooltip("Контейнер для кнопок вариантов ответа."), SerializeField]
        private Transform _choiceContainer;

        /// <summary> Префаб кнопки варианта ответа. </summary>
        [Tooltip("Префаб кнопки варианта ответа."), SerializeField]
        private DialogueChoiceButton _choiceButtonPrefab;

        /// <summary> Кнопка для перехода к следующей реплике. </summary>
        [Header("Other")] [Tooltip("Кнопка для перехода к следующей реплике."), SerializeField]
        private Button _nextButton;

        /// <summary> Объект текста кнопки Next. </summary>
        [Tooltip("Объект текста кнопки Next."), SerializeField]
        private GameObject _nextButtonPreview;

        /// <summary> Канвас с основным игровым интерфейсом (HUD). </summary>
        [Tooltip("Канвас с основным игровым интерфейсом (HUD)."), SerializeField]
        private Canvas _hud;

        /// <summary> Событие при нажатии кнопки Next. </summary>
        public event Action OnNextClicked;

        /// <summary> Событие при выборе варианта ответа. </summary>
        public event Action<DialogueNode> OnChoiceSelected;

        #endregion

        /// <summary> Подписка на кнопку Next. </summary>
        private void Awake() => _nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());

        /// <summary> Отображает окно диалога с переданными данными. </summary>
        /// <param name="data"> Данные текущего диалога. </param>
        public void ShowDialogue(DialogueData data)
        {
            gameObject.SetActive(true);
            _hud.gameObject.SetActive(false);

            _dialogueText.text = data.Text;
            SetSpeakerInfo(data.SpeakerInfo);

            _nextButton.enabled = !data.IsChoosing;
            _nextButtonPreview.SetActive(!data.IsChoosing);
            _choiceContainer.gameObject.SetActive(data.IsChoosing);

            RenderChoices(data.Choices, data.IsChoosing);
        }

        /// <summary> Скрывает окно диалога и восстанавливает отображение HUD. </summary>
        public void HideDialogue()
        {
            gameObject.SetActive(false);
            _hud.gameObject.SetActive(true);
            ClearChoices();
        }

        /// <summary> Устанавливает информацию о говорящем персонаже. </summary>
        /// <param name="npc"> Информация о персонаже. </param>
        private void SetSpeakerInfo(NpcInfo npc)
        {
            _speakerName.text = npc.NpcName.ToString();
            _romanceableIcon.gameObject.SetActive(npc.IsRomanceable);
            // TODO: подключить _speakerPreview (портрет/анимация)
        }

        /// <summary> Отрисовывает кнопки выбора реплик, если активен режим выбора. </summary>
        /// <param name="choices"> Список доступных вариантов. </param>
        /// <param name="isChoosing"> Флаг, указывающий, выбирает ли игрок. </param>
        private void RenderChoices(IEnumerable<DialogueNode> choices, bool isChoosing)
        {
            ClearChoices();

            if (!isChoosing || choices == null) return;

            foreach (var choice in choices)
            {
                var choiceButton = Instantiate(_choiceButtonPrefab, _choiceContainer);
                choiceButton.SetText(choice.Text);
                choiceButton.GetComponent<UIButton>().OnClick += () => OnChoiceSelected?.Invoke(choice);
            }
        }

        /// <summary> Очищает все текущие варианты ответа из UI. </summary>
        private void ClearChoices()
        {
            foreach (Transform child in _choiceContainer) Destroy(child.gameObject);
        }
    }
}