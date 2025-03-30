using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент для управления диалогом игрока. </summary>
    public class PlayerConversant : MonoBehaviour
    {
        /// <summary> Текущий диалог. </summary>
        private Dialogue _currentDialogue;

        /// <summary> Текущий активный узел диалога. </summary>
        private DialogueNode _currentNode;

        /// <summary> Выбирает ли игрок сейчас реплику? </summary>
        public bool IsChoosingDialogue { get; private set; }

        /// <summary> Идёт ли в данный момент диалог? </summary>
        public bool IsDialogueActive => _currentDialogue;

        /// <summary> Событие, вызываемое при любом обновлении диалога (текста, вариантов и т.п.). </summary>
        public event Action OnConversationUpdated;

        /// <summary> Запуск нового диалога. </summary>
        /// <param name="dialogue"> Диалог для запуска. </param>
        public void StartDialogue(Dialogue dialogue)
        {
            _currentDialogue = dialogue;
            _currentNode = dialogue.RootNode;
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Получить текст текущего узла диалога. </summary>
        /// <returns> Текст текущей реплики или пустая строка. </returns>
        public string GetText() => _currentNode ? _currentNode.Text : string.Empty;

        /// <summary> Перейти к следующему узлу диалога. </summary>
        public void NextDialogue()
        {
            if (_currentDialogue.GetPlayerChildNodes(_currentNode).Any())
            {
                IsChoosingDialogue = true;
                TriggerExitAction();
                OnConversationUpdated?.Invoke();
                return;
            }

            if (!_currentDialogue.GetChildNodes(_currentNode).Any())
            {
                QuitDialogue();
                return;
            }

            var childAINodes = _currentDialogue.GetNpcChildNodes(_currentNode).ToArray();
            TriggerExitAction();
            _currentNode = childAINodes[Random.Range(0, childAINodes.Length)];
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Есть ли ещё реплики после текущей? </summary>
        /// <returns> True, если есть хотя бы одна дочерняя реплика. </returns>
        public bool HasNextDialogue() => _currentDialogue.GetChildNodes(_currentNode).Any();

        /// <summary> Получить список доступных вариантов для выбора игрока. </summary>
        /// <returns> Список узлов, представляющих выбор игрока. </returns>
        public IEnumerable<DialogueNode> GetChoices() => _currentDialogue.GetPlayerChildNodes(_currentNode);

        /// <summary> Выбрать вариант ответа игрока. </summary>
        /// <param name="chosenNode"> Выбранный игроком узел. </param>
        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosingDialogue = false;
            NextDialogue();
        }

        /// <summary> Завершить текущий диалог. </summary>
        public void QuitDialogue()
        {
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            IsChoosingDialogue = false;
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Выполнить действие, назначенное при входе в текущий узел. </summary>
        private void TriggerEnterAction()
        {
            if (_currentNode && _currentNode.OnEnterAction != string.Empty) print(_currentNode.OnEnterAction);
        }

        /// <summary> Выполнить действие, назначенное при выходе из текущего узла. </summary>
        private void TriggerExitAction()
        {
            if (_currentNode && _currentNode.OnExitAction != string.Empty) print(_currentNode.OnExitAction);
        }
    }
}