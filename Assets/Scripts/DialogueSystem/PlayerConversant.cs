using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue _testDialogue;

        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        public bool IsChoosingDialogue { get; private set; }
        public bool IsDialogueActive => _currentDialogue;


        public event Action OnConversationUpdated;

        // private IEnumerator Start()
        // {
        //     yield return new WaitForSeconds(2);
        //     StartDialogue(_testDialogue);
        // }

        public void StartDialogue(Dialogue dialogue)
        {
            _currentDialogue = dialogue;
            _currentNode = dialogue.RootNode;
            OnConversationUpdated?.Invoke();
        }

        public string GetText() => _currentNode ? _currentNode.Text : string.Empty;

        public void NextDialogue()
        {
            if (_currentDialogue.GetPlayerChildNodes(_currentNode).Any())
            {
                IsChoosingDialogue = true;
                OnConversationUpdated?.Invoke();
                return;
            }

            if (!_currentDialogue.GetChildNodes(_currentNode).Any())
            {
                QuitDialogue();
                return;
            }

            var childAINodes = _currentDialogue.GetAIChildNodes(_currentNode).ToArray();
            _currentNode = childAINodes[Random.Range(0, childAINodes.Length)];
            OnConversationUpdated?.Invoke();
        }

        public bool HasNextDialogue() => _currentDialogue.GetChildNodes(_currentNode).Any();

        public IEnumerable<DialogueNode> GetChoices() => _currentDialogue.GetPlayerChildNodes(_currentNode);

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            IsChoosingDialogue = false;
            NextDialogue();
        }

        public void QuitDialogue()
        {
            _currentDialogue = null;
            _currentNode = null;
            IsChoosingDialogue = false;
            OnConversationUpdated?.Invoke();
        }
    }
}