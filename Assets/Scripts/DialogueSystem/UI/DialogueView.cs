using FlavorfulStory.InputSystem;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Отображение диалогового окна. </summary>
    public class DialogueView : MonoBehaviour
    {
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
        [Header("Next Button")] [SerializeField]
        private Button _nextButton;

        /// <summary> Объект текста кнопки Next. </summary>
        [SerializeField] private GameObject _nextButtonPreview;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private Canvas _hud;

        /// <summary> Ссылка на компонент PlayerConversant для управления диалогом. </summary>
        private PlayerSpeaker _playerSpeaker;

        private bool IsActive => _playerSpeaker.IsDialogueActive;

        /// <summary> Инициализация компонентов и подписка на события. </summary>
        private void Awake()
        {
            _playerSpeaker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpeaker>();
            _nextButton.onClick.AddListener(OnClickNextDialogue);
            _playerSpeaker.OnConversationUpdated += UpdateView;
            _playerSpeaker.OnConversationUpdated += UpdateHud;
        }

        /// <summary> При старте обновляем отображение. </summary>
        private void Start() => UpdateView();

        private void Update()
        {
            if (!IsActive) return;

            if (InputWrapper.GetButtonDown(InputButton.NextDialogue)) OnNextClicked();

            if (InputWrapper.GetButtonDown(InputButton.SkipDialogue)) OnSkipClicked();
        }

        private void OnNextClicked()
        {
            print("Next button clicked");
        }

        private void OnSkipClicked()
        {
            print("Skip clicked");
        }


        /// <summary> Обработчик нажатия на кнопку перехода к следующей реплике. </summary>
        private void OnClickNextDialogue() => _playerSpeaker.PlayNextDialogueNode();

        /// <summary> Обновить отображение в зависимости от состояния диалога. </summary>
        private void UpdateView()
        {
            gameObject.SetActive(IsActive);
            if (!IsActive) return;

            SetSpeakerView(_playerSpeaker.CurrentNpcSpeaker);
            _nextButton.enabled = !_playerSpeaker.IsChoosingDialogue;
            _nextButtonPreview.SetActive(!_playerSpeaker.IsChoosingDialogue);
            _choiceContainer.gameObject.SetActive(_playerSpeaker.IsChoosingDialogue);

            if (_playerSpeaker.IsChoosingDialogue)
                BuildChoiceList();
            else
                _dialogueText.text = _playerSpeaker.GetText();
        }

        /// <summary> Установить отображение информации о текущем спикере. </summary>
        /// <param name="speaker"> Спикер, информацию которого нужно отобразить. </param>
        private void SetSpeakerView(NpcSpeaker speaker)
        {
            _speakerName.text = speaker.NpcInfo.NpcName.ToString();
            _romanceableIcon.gameObject.SetActive(speaker.NpcInfo.IsRomanceable);
            // TODO: Модельку персонажа добавить
        }

        /// <summary> Построить список доступных вариантов ответа. </summary>
        private void BuildChoiceList()
        {
            ClearChoices();
            SpawnChoices();
        }

        /// <summary> Удалить старые кнопки вариантов. </summary>
        private void ClearChoices()
        {
            foreach (Transform choice in _choiceContainer) Destroy(choice.gameObject);
        }

        /// <summary> Создать кнопки для всех доступных вариантов ответа. </summary>
        private void SpawnChoices()
        {
            foreach (var choice in _playerSpeaker.GetChoices())
            {
                var choiceInstance = Instantiate(_choiceButtonPrefab, _choiceContainer);
                choiceInstance.SetText(choice.Text);
                var button = choiceInstance.GetComponent<UIButton>();
                button.OnClick += () => { OnClickSelectChoice(choice); };
            }
        }

        /// <summary> Обработчик выбора варианта ответа. </summary>
        /// <param name="choice"> Выбранный узел диалога. </param>
        private void OnClickSelectChoice(DialogueNode choice) => _playerSpeaker.SelectChoice(choice);

        /// <summary>
        /// 
        /// </summary>
        private void UpdateHud() => _hud.gameObject.SetActive(!IsActive);
    }
}