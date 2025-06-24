using System;
using System.Collections.Generic;
using DG.Tweening;
using FlavorfulStory.AI;
using FlavorfulStory.UI.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        [Header("Speaker Info")]
        [Tooltip("Текстовое поле для имени говорящего персонажа."), SerializeField]
        private TMP_Text _speakerName;

        /// <summary> Иконка, показывающая, что персонаж доступен для романтики. </summary>
        [Tooltip("Иконка, показывающая, что персонаж доступен для романтики."), SerializeField]
        private Image _romanceableIcon;

        /// <summary> Превью персонажа. </summary>
        [Tooltip("Превью персонажа."), SerializeField]
        private RawImage _speakerPreview;

        /// <summary> Контейнер для кнопок вариантов ответа. </summary>
        [Header("Choices")]
        [Tooltip("Контейнер для кнопок вариантов ответа."), SerializeField]
        private RectTransform _choiceContainer;

        /// <summary> Префаб кнопки варианта ответа. </summary>
        [Tooltip("Префаб кнопки варианта ответа."), SerializeField]
        private DialogueChoiceButton _choiceButtonPrefab;

        /// <summary> Кнопка для перехода к следующей реплике. </summary>
        [Header("Other")]
        [Tooltip("Кнопка для перехода к следующей реплике."), SerializeField]
        private Button _nextButton;

        /// <summary> Объект текста кнопки Next. </summary>
        [Tooltip("Объект текста кнопки Next."), SerializeField]
        private GameObject _nextButtonPreview;

        /// <summary> Панель диалога. </summary>
        [Tooltip("Панель диалога."), SerializeField]
        private RectTransform _dialoguePanel;

        /// <summary> Аниматор для панели и модели. </summary>
        private DialogueViewAnimator _animator;

        /// <summary> Отображение модели персонажа в диалогах. </summary>
        private DialogueModelPresenter _dialogueModelPresenter;

        /// <summary> Компонент управления HUD затемнением. </summary>
        private CanvasGroupFader _hudFader;

        /// <summary> Текущая информация об NPC. </summary>
        private NpcInfo _currentSpeakerInfo;

        /// <summary> Событие при нажатии кнопки Next. </summary>
        public event Action OnNextClicked;

        /// <summary> Событие при выборе варианта ответа. </summary>
        public event Action<DialogueNode> OnChoiceSelected;

        /// <summary> Событие при скрытии окна диалога. </summary>
        public event Action OnHidden;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="hudFader"> Затемнитель интерфейса HUD. </param>
        /// <param name="dialogueModelPresenter"> Отображение модели персонажа в диалогах. </param>
        [Inject]
        private void Construct([Inject(Id = "HUD")] CanvasGroupFader hudFader,
            DialogueModelPresenter dialogueModelPresenter)
        {
            _hudFader = hudFader;
            _dialogueModelPresenter = dialogueModelPresenter;
        }

        /// <summary> Подписка на кнопку Next и инициализация аниматора. </summary>
        private void Awake()
        {
            _nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
            _animator = new DialogueViewAnimator(_dialoguePanel, _choiceContainer, _dialogueText, _speakerPreview);
        }

        /// <summary> Отображает окно диалога с переданными данными. </summary>
        /// <param name="data"> Данные текущего диалога. </param>
        public void Show(DialogueData data)
        {
            if (!_currentSpeakerInfo || _currentSpeakerInfo != data.SpeakerInfo)
            {
                SetSpeakerInfo(data.SpeakerInfo);
                gameObject.SetActive(true);
                _animator.AnimateEntrance();
                _hudFader.Hide();
            }

            _animator.AnimateText(data.Text);
            _nextButton.enabled = !data.IsChoosing;
            _nextButtonPreview.SetActive(!data.IsChoosing);
            _choiceContainer.gameObject.SetActive(data.IsChoosing);

            RenderChoices(data.Choices, data.IsChoosing);
        }

        /// <summary> Скрывает окно диалога и восстанавливает отображение HUD. </summary>
        public void Hide() => _animator.AnimateExit().OnComplete(() =>
        {
            _dialogueModelPresenter.DestroyModel();
            gameObject.SetActive(false);
            _hudFader.Show();
            _currentSpeakerInfo = null;
            ClearChoices();
            OnHidden?.Invoke();
        });

        /// <summary> Устанавливает информацию о говорящем персонаже. </summary>
        /// <param name="npc"> Информация о персонаже. </param>
        private void SetSpeakerInfo(NpcInfo npc)
        {
            _currentSpeakerInfo = npc;
            _dialogueModelPresenter.InstantiateModel(npc.DialogueModelPrefab);

            _speakerName.text = npc.NpcName.ToString();
            _romanceableIcon.gameObject.SetActive(npc.IsRomanceable);
        }

        /// <summary> Отрисовывает кнопки выбора реплик, если активен режим выбора. </summary>
        /// <param name="choices"> Список доступных вариантов. </param>
        /// <param name="isChoosing"> Флаг, указывающий, выбирает ли игрок. </param>
        private async void RenderChoices(IEnumerable<DialogueNode> choices, bool isChoosing)
        {
            ClearChoices();

            if (!isChoosing || choices == null) return;

            var buttons = new List<DialogueChoiceButton>();

            foreach (var choice in choices)
            {
                var choiceButton = Instantiate(_choiceButtonPrefab, _choiceContainer);
                choiceButton.SetText(choice.Text);
                choiceButton.Interactable = false;
                choiceButton.OnClick += async () =>
                {
                    await DialogueViewAnimator.AnimateChoiceSelection(choiceButton, buttons);
                    OnChoiceSelected?.Invoke(choice);
                };
                buttons.Add(choiceButton);
            }

            await _animator.AnimateChoicesContainer();

            foreach (var button in buttons) button.Interactable = true;
        }


        /// <summary> Очищает все текущие варианты ответа из UI. </summary>
        private void ClearChoices()
        {
            // TODO: REFACTOR ЧЕРЕЗ ФАБРИКУ
            foreach (Transform child in _choiceContainer) Destroy(child.gameObject);
        }
    }
}