using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент для управления диалогом игрока. </summary>
    public class PlayerSpeaker : MonoBehaviour
    {
        /// <summary> NPC, с которым в данный момент ведётся диалог. </summary>
        public NpcSpeaker CurrentNpcSpeaker { get; private set; }

        /// <summary> Текущий активный диалог. </summary>
        private Dialogue _currentDialogue;

        /// <summary> Текущий активный узел диалога. </summary>
        private DialogueNode _currentNode;

        /// <summary> Выбирает ли игрок в данный момент реплику? </summary>
        public bool IsChoosingDialogue { get; private set; }

        /// <summary> Идёт ли сейчас диалог. </summary>
        public bool IsDialogueActive => _currentDialogue;

        /// <summary> Событие, вызываемое при любом изменении состояния диалога (обновление текста, вариантов и т.п.). </summary>
        public event Action OnConversationUpdated;

        /// <summary> Запуск нового диалога. </summary>
        /// <param name="npcSpeaker"> NPC, с которым начинается диалог. </param>
        /// <param name="dialogue"> Диалог для запуска. </param>
        public void StartDialogue(NpcSpeaker npcSpeaker, Dialogue dialogue)
        {
            CurrentNpcSpeaker = npcSpeaker;
            _currentDialogue = dialogue;
            _currentNode = dialogue.RootNode;
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Получить текст текущего узла диалога. </summary>
        /// <returns> Текст текущей реплики или пустая строка, если диалог не активен. </returns>
        public string GetText() => _currentNode ? _currentNode.Text : string.Empty;

        /// <summary> Воспроизвести следующий узел диалога. </summary>
        public void PlayNextDialogueNode()
        {
            // Если есть выбор для игрока — перейти в режим выбора
            if (_currentDialogue.GetPlayerChildNodes(_currentNode).Any())
            {
                IsChoosingDialogue = true;
                TriggerExitAction();
                OnConversationUpdated?.Invoke();
                return;
            }

            // Если нет дальнейших узлов — завершить диалог
            if (!_currentDialogue.GetChildNodes(_currentNode).Any())
            {
                QuitDialogue();
                return;
            }

            // Выбрать случайный ответ NPC
            var childAINodes = _currentDialogue.GetNpcChildNodes(_currentNode).ToArray();
            TriggerExitAction();
            _currentNode = childAINodes[Random.Range(0, childAINodes.Length)];
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Есть ли ещё доступные узлы после текущего? </summary>
        /// <returns> True, если есть хотя бы одна дочерняя реплика. </returns>
        public bool HasNextDialogue() => _currentDialogue.GetChildNodes(_currentNode).Any();

        /// <summary> Получить список доступных для игрока вариантов ответа. </summary>
        /// <returns> Список узлов, которые представляет выбор игрока. </returns>
        public IEnumerable<DialogueNode> GetChoices() => _currentDialogue.GetPlayerChildNodes(_currentNode);

        /// <summary> Выбрать вариант ответа игрока. </summary>
        /// <param name="chosenNode"> Узел, выбранный игроком. </param>
        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosingDialogue = false;
            PlayNextDialogueNode();
        }

        /// <summary> Завершить текущий диалог. </summary>
        public void QuitDialogue()
        {
            _currentDialogue = null;
            TriggerExitAction();
            CurrentNpcSpeaker = null;
            _currentNode = null;
            IsChoosingDialogue = false;
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Выполнить действие, назначенное при входе в текущий узел. </summary>
        private void TriggerEnterAction()
        {
            if (_currentNode) TriggerAction(_currentNode.EnterActionName);
        }

        /// <summary> Выполнить действие, назначенное при выходе из текущего узла. </summary>
        private void TriggerExitAction()
        {
            if (_currentNode) TriggerAction(_currentNode.ExitActionName);
        }

        /// <summary> Выполнить указанное действие для всех триггеров диалога у текущего NPC. </summary>
        /// <param name="action"> Имя действия. </param>
        private void TriggerAction(string action)
        {
            if (action == string.Empty) return;
            CurrentNpcSpeaker.GetComponent<DialogueTrigger>().TriggerDialogue(action);
        }
    }
}