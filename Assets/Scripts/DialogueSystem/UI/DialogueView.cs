using FlavorfulStory.AI.States;
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
        [SerializeField] private TMP_Text _dialogueText;

        [Header("Speaker")] [SerializeField] private TMP_Text _speakerName;

        /// <summary> Иконка, показывающая, что персонаж доступен для романтики. </summary>
        [SerializeField] private Image _romanceableIcon;

        /// <summary> Отображение превью персонажа (например, портрет или модель). </summary>
        [SerializeField] private GameObject _speakerPreview;

        /// <summary> Контейнер для отображения вариантов ответов игрока. </summary>
        [Header("Choices")] [SerializeField] private Transform _choiceContainer;

        /// <summary> Префаб кнопки варианта ответа. </summary>
        [SerializeField] private DialogueChoiceButton _choiceButtonPrefab;

        /// <summary> Кнопка для перехода к следующей реплике. </summary>
        [Header("Other")] [SerializeField]
        private Button _nextButton;

        /// <summary> Объект текста кнопки Next. </summary>
        [SerializeField] private GameObject _nextButtonPreview;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private Canvas _hud;

        private PlayerSpeaker _speaker;

        #endregion

        public void Initialize(PlayerSpeaker speaker)
        {
            _speaker = speaker;
            _speaker.OnConversationUpdated += UpdateView;
            _nextButton.onClick.AddListener(() => _speaker.PlayNextDialogueNode());
        }

        /// <summary> Обновить отображение в зависимости от состояния диалога. </summary>
        private void UpdateView()
        {
            gameObject.SetActive(_speaker.IsDialogueActive);
            _hud.gameObject.SetActive(!_speaker.IsDialogueActive);

            if (!_speaker.IsDialogueActive) return;

            SetSpeakerView(_speaker.CurrentNpcSpeaker?.NpcInfo);

            _nextButton.enabled = !_speaker.IsChoosingDialogue;
            _nextButtonPreview.SetActive(!_speaker.IsChoosingDialogue);
            _choiceContainer.gameObject.SetActive(_speaker.IsChoosingDialogue);

            if (_speaker.IsChoosingDialogue) BuildChoiceList();
            else _dialogueText.text = _speaker.GetText();
        }

        private void SetSpeakerView(NpcInfo npc)
        {
            _speakerName.text = npc.NpcName.ToString();
            _romanceableIcon.gameObject.SetActive(npc.IsRomanceable);
            // TODO: Модельку персонажа добавить
        }

        private void BuildChoiceList()
        {
            foreach (Transform child in _choiceContainer) Destroy(child.gameObject);

            foreach (var choice in _speaker.GetChoices())
            {
                var instance = Instantiate(_choiceButtonPrefab, _choiceContainer);
                instance.SetText(choice.Text);
                var button = instance.GetComponent<UIButton>();
                button.OnClick += () => _speaker.SelectChoice(choice);
            }
        }
    }
}