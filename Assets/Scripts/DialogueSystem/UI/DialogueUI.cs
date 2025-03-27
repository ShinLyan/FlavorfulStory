using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _nextButtonPreview;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private DialogueChoiceButton _choiceButtonPrefab;

        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            _nextButton.onClick.AddListener(OnClickNextDialogue);
            _playerConversant.OnConversationUpdated += UpdateUI;

            // TODO: УБРАТЬ
            UpdateUI();
        }

        private void OnClickNextDialogue()
        {
            _playerConversant.NextDialogue();
        }

        private void UpdateUI()
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

        private void BuildChoiceList()
        {
            ClearChoices();
            SpawnChoices();
        }

        private void ClearChoices()
        {
            foreach (Transform choice in _choiceContainer) Destroy(choice.gameObject);
        }

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

        private void OnClickSelectChoice(DialogueNode choice)
        {
            _playerConversant.SelectChoice(choice);
        }
    }
}