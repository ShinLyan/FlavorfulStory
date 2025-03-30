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

        /// <summary> Кнопка для перехода к следующей реплике. </summary>
        [SerializeField] private Button _nextButton;

        /// <summary> Объект текста кнопки Next. </summary>
        [SerializeField] private GameObject _nextButtonPreview;

        /// <summary> Контейнер для отображения вариантов ответов игрока. </summary>
        [SerializeField] private Transform _choiceContainer;

        /// <summary> Префаб кнопки варианта ответа. </summary>
        [SerializeField] private DialogueChoiceButton _choiceButtonPrefab;

        /// <summary> Ссылка на компонент PlayerConversant для управления диалогом. </summary>
        private PlayerConversant _playerConversant;

        /// <summary> Инициализация компонентов и подписка на события. </summary>
        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            _nextButton.onClick.AddListener(OnClickNextDialogue);
            _playerConversant.OnConversationUpdated += UpdateView;

            // TODO: УБРАТЬ
            UpdateView();
        }

        /// <summary> Обработчик нажатия на кнопку перехода к следующей реплике. </summary>
        private void OnClickNextDialogue() => _playerConversant.NextDialogue();

        /// <summary> Обновить отображение в зависимости от состояния диалога. </summary>
        private void UpdateView()
        {
            gameObject.SetActive(_playerConversant.IsDialogueActive);
            if (!_playerConversant.IsDialogueActive) return;

            _nextButton.enabled = !_playerConversant.IsChoosingDialogue;
            _nextButtonPreview.SetActive(!_playerConversant.IsChoosingDialogue);
            _choiceContainer.gameObject.SetActive(_playerConversant.IsChoosingDialogue);

            if (_playerConversant.IsChoosingDialogue)
                BuildChoiceList();
            else
                _dialogueText.text = _playerConversant.GetText();
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
            foreach (var choice in _playerConversant.GetChoices())
            {
                var choiceInstance = Instantiate(_choiceButtonPrefab, _choiceContainer);
                choiceInstance.SetText(choice.Text);
                var button = choiceInstance.GetComponent<UIButton>();
                button.OnClick += () => { OnClickSelectChoice(choice); };
            }
        }

        /// <summary> Обработчик выбора варианта ответа. </summary>
        /// <param name="choice"> Выбранный узел диалога. </param>
        private void OnClickSelectChoice(DialogueNode choice) => _playerConversant.SelectChoice(choice);
    }
}