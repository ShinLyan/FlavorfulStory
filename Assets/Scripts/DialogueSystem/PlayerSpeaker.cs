using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.DialogueSystem.UI;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент для управления диалогом игрока. </summary>
    public class PlayerSpeaker : MonoBehaviour, IDialogueInitiator
    {
        #region Fields

        /// <summary> NPC, с которым в данный момент ведётся диалог. </summary>
        private NpcSpeaker CurrentNpcSpeaker { get; set; }

        /// <summary> Текущий активный диалог. </summary>
        private Dialogue _currentDialogue;

        /// <summary> Текущий активный узел диалога. </summary>
        private DialogueNode _currentNode;

        /// <summary> UI-компонент отображения диалога. </summary>
        private DialogueView _dialogueView;

        /// <summary> Выбирает ли игрок в данный момент реплику? </summary>
        private bool IsChoosingDialogue { get; set; }

        /// <summary> Идёт ли сейчас диалог? </summary>
        private bool IsDialogueActive => _currentDialogue;

        /// <summary> Событие обновления состояния диалога. </summary>
        public event Action OnConversationUpdated;

        /// <summary> Событие завершения диалога. </summary>
        public event Action OnConversationEnded;

        #endregion

        /// <summary> Инициализация компонента и подписка на события UI. </summary>
        private void Awake()
        {
            // TODO: ZENJECT
            _dialogueView = FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include);

            _dialogueView.OnNextClicked += PlayNextDialogueNode;
            _dialogueView.OnChoiceSelected += SelectChoice;
        }

        /// <summary> Обработка ввода для перехода к следующей реплике. </summary>
        private void Update()
        {
            if (!IsDialogueActive || IsChoosingDialogue ||
                !InputWrapper.GetButtonDown(InputButton.NextDialogue))
                return;

            PlayNextDialogueNode();
        }

        /// <summary> Очистить подписки и события при уничтожении компонента. </summary>
        private void OnDestroy()
        {
            if (_dialogueView)
            {
                _dialogueView.OnNextClicked -= PlayNextDialogueNode;
                _dialogueView.OnChoiceSelected -= SelectChoice;
            }

            OnConversationUpdated = null;
        }

        #region IDialogueInitiator

        /// <summary> Запуск нового диалога. </summary>
        /// <param name="npcSpeaker"> NPC, с которым начинается диалог. </param>
        /// <param name="dialogue"> Диалог для запуска. </param>
        public void StartDialogue(NpcSpeaker npcSpeaker, Dialogue dialogue)
        {
            WorldTime.Pause();

            CurrentNpcSpeaker = npcSpeaker;
            _currentDialogue = dialogue;
            _currentNode = dialogue.RootNode;

            TriggerEnterAction();
            UpdateDialogueView();

            StartCoroutine(EnableNextDialogueInput());
        }

        /// <summary> Завершить текущий диалог. </summary>
        public void EndDialogue()
        {
            WorldTime.Unpause();

            _currentDialogue = null;
            TriggerExitAction();
            CurrentNpcSpeaker = null;
            _currentNode = null;
            IsChoosingDialogue = false;

            _dialogueView.HideDialogue();

            OnConversationUpdated?.Invoke();
            OnConversationEnded?.Invoke();
        }

        #endregion

        /// <summary> Разблокировка ввода для перехода к следующей реплике. </summary>
        /// <remarks> Блокирует ввод на один кадр, чтобы избежать случайного пропуска. </remarks>
        private static IEnumerator EnableNextDialogueInput()
        {
            yield return null; // Пропустить кадр, в котором был вызван StartDialogue
            InputWrapper.UnblockInput(new[] { InputButton.NextDialogue, InputButton.SkipDialogue });
        }

        /// <summary> Получить текст текущего узла диалога. </summary>
        /// <returns> Текст текущей реплики или пустая строка. </returns>
        private string GetText() => _currentNode ? _currentNode.Text : string.Empty;

        /// <summary> Воспроизвести следующий узел диалога. </summary>
        private void PlayNextDialogueNode()
        {
            if (!IsDialogueActive) return;

            // Если есть выбор для игрока — перейти в режим выбора
            if (_currentDialogue.GetPlayerChildNodes(_currentNode).Any())
            {
                IsChoosingDialogue = true;
                TriggerExitAction();
                UpdateDialogueView();
                OnConversationUpdated?.Invoke();
                return;
            }

            // Если нет дальнейших узлов — завершить диалог
            if (!HasNextDialogue())
            {
                EndDialogue();
                return;
            }

            // Выбрать случайный ответ NPC
            var childAINodes = _currentDialogue.GetNpcChildNodes(_currentNode).ToArray();
            TriggerExitAction();
            _currentNode = childAINodes[Random.Range(0, childAINodes.Length)];
            TriggerEnterAction();

            IsChoosingDialogue = false;
            UpdateDialogueView();
            OnConversationUpdated?.Invoke();
        }

        /// <summary> Есть ли ещё доступные узлы после текущего? </summary>
        /// <returns> <c>true</c>, если есть хотя бы одна дочерняя реплика. </returns>
        private bool HasNextDialogue() => _currentDialogue.GetChildNodes(_currentNode).Any();

        /// <summary> Получить список доступных для игрока вариантов ответа. </summary>
        /// <returns> Список узлов, которые представляет выбор игрока. </returns>
        private IEnumerable<DialogueNode> GetChoices() => _currentDialogue.GetPlayerChildNodes(_currentNode);

        /// <summary> Выбрать вариант ответа игрока. </summary>
        /// <param name="chosenNode"> Узел, выбранный игроком. </param>
        private void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosingDialogue = false;
            PlayNextDialogueNode();
        }

        /// <summary> Обновить UI диалога в соответствии с текущим узлом. </summary>
        private void UpdateDialogueView()
        {
            if (!IsDialogueActive)
            {
                _dialogueView.HideDialogue();
                return;
            }

            var data = new DialogueData(GetText(), CurrentNpcSpeaker?.NpcInfo, IsChoosingDialogue, GetChoices());
            _dialogueView.ShowDialogue(data);
        }

        /// <summary> Выполнить действие входа, назначенное текущему узлу. </summary>
        private void TriggerEnterAction()
        {
            if (_currentNode) TriggerAction(_currentNode.EnterActionName);
        }

        /// <summary> Выполнить действие выхода, назначенное текущему узлу. </summary>
        private void TriggerExitAction()
        {
            if (_currentNode) TriggerAction(_currentNode.ExitActionName);
        }

        /// <summary> Выполнить указанное действие у триггеров текущего NPC. </summary>
        /// <param name="action"> Имя действия. </param>
        private void TriggerAction(string action)
        {
            if (action == string.Empty) return;
            CurrentNpcSpeaker.GetComponent<DialogueTrigger>().TriggerDialogue(action);
        }
    }
}