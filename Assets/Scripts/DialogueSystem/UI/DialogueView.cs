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
        [Header("Speaker Info")] [Tooltip("Текстовое поле для имени говорящего персонажа."), SerializeField]
        private TMP_Text _speakerName;

        /// <summary> Иконка, показывающая, что персонаж доступен для романтики. </summary>
        [Tooltip("Иконка, показывающая, что персонаж доступен для романтики."), SerializeField]
        private Image _romanceableIcon;

        /// <summary> Превью персонажа. </summary>
        [Tooltip("Превью персонажа."), SerializeField]
        private RawImage _speakerPreview;

        /// <summary> Контейнер для кнопок вариантов ответа. </summary>
        [Header("Choices")] [Tooltip("Контейнер для кнопок вариантов ответа."), SerializeField]
        private RectTransform _choiceContainer;

        /// <summary> Префаб кнопки варианта ответа. </summary>
        [Tooltip("Префаб кнопки варианта ответа."), SerializeField]
        private DialogueChoiceButton _choiceButtonPrefab;

        /// <summary> Кнопка для перехода к следующей реплике. </summary>
        [Header("Other")] [Tooltip("Кнопка для перехода к следующей реплике."), SerializeField]
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

        /// <summary> Текущая анимация текста. </summary>
        private Tween _textTween;

        /// <summary> Список кнопок с ответами. </summary>
        private readonly List<DialogueChoiceButton> _choiceButtons = new();

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
            _animator = new DialogueViewAnimator(_dialoguePanel, _choiceContainer, _dialogueText, _speakerPreview);
            _nextButton.onClick.AddListener(CompleteOrProceed);
            _dialogueText.text = string.Empty;
            CacheInitialChoiceButtons();
        }

        /// <summary> Кэширует уже созданные на сцене кнопки вариантов ответа,
        /// чтобы переиспользовать их вместо уничтожения/создания. </summary>
        private void CacheInitialChoiceButtons()
        {
            foreach (Transform child in _choiceContainer)
            {
                var button = child.GetComponent<DialogueChoiceButton>();
                if (!button || _choiceButtons.Contains(button)) continue;

                button.gameObject.SetActive(false);
                _choiceButtons.Add(button);
            }
        }

        /// <summary> Если текст печатается — мгновенно показать его полностью,
        /// иначе перейти к следующей реплике. </summary>
        public void CompleteOrProceed()
        {
            if (_textTween != null && _textTween.IsActive() && _textTween.IsPlaying())
                _textTween.Complete();
            else
                OnNextClicked?.Invoke();
        }

        /// <summary> Отображает окно диалога с переданными данными. </summary>
        /// <param name="data"> Данные текущего диалога. </param>
        public async void Show(DialogueData data)
        {
            if (!_currentSpeakerInfo || _currentSpeakerInfo != data.SpeakerInfo)
            {
                await _hudFader.Hide().AsyncWaitForCompletion();

                gameObject.SetActive(true);
                _animator.AnimateEntrance();
                SetSpeakerInfo(data.SpeakerInfo);
            }

            _nextButton.enabled = !data.IsChoosing;
            _nextButtonPreview.SetActive(!data.IsChoosing);
            _choiceContainer.gameObject.SetActive(data.IsChoosing);

            if (data.IsChoosing)
                RenderChoices(data.Choices);
            else
                _textTween = await _animator.FadeOutAndAnimateNewText(data.Text);
        }

        /// <summary> Скрывает окно диалога и восстанавливает отображение HUD. </summary>
        public void Hide() => _animator.AnimateExit().OnComplete(() =>
        {
            _dialogueModelPresenter.DestroyModel();
            gameObject.SetActive(false);
            _hudFader.Show();
            _currentSpeakerInfo = null;
            _dialogueText.text = string.Empty;
            DisableChoices();
            OnHidden?.Invoke();
        });

        /// <summary> Очищает все текущие варианты ответа из UI. </summary>
        private void DisableChoices()
        {
            foreach (var button in _choiceButtons) button.gameObject.SetActive(false);
        }

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
        private async void RenderChoices(IEnumerable<DialogueNode> choices)
        {
            int index = 0;
            foreach (var choice in choices)
            {
                DialogueChoiceButton button;
                if (index < _choiceButtons.Count)
                {
                    button = _choiceButtons[index];
                }
                else
                {
                    button = Instantiate(_choiceButtonPrefab, _choiceContainer);
                    _choiceButtons.Add(button);
                }

                ResetChoiceButtonState(button);
                button.SetText(choice.Text);
                button.OnClick = async () =>
                {
                    await DialogueViewAnimator.AnimateChoiceSelection(button, _choiceButtons);
                    OnChoiceSelected?.Invoke(choice);
                };

                index++;
            }

            // Деактивировать лишние кнопки, если их больше, чем текущих вариантов
            for (int i = index; i < _choiceButtons.Count; i++) _choiceButtons[i].gameObject.SetActive(false);

            await _animator.AnimateChoicesContainer();

            // Включаем интерактивность после анимации
            for (int i = 0; i < index; i++) _choiceButtons[i].Interactable = true;
        }

        /// <summary> Сбрасывает состояние кнопки выбора перед повторным использованием. </summary>
        /// <param name="button"> Кнопка выбора. </param>
        private static void ResetChoiceButtonState(DialogueChoiceButton button)
        {
            var canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup) canvasGroup.alpha = 1f;

            button.Interactable = false;
            button.gameObject.SetActive(true);
        }
    }
}