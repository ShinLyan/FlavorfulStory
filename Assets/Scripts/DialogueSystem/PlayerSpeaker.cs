using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Core;
using FlavorfulStory.DialogueSystem.UI;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;
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

        /// <summary> Событие завершения диалога. </summary>
        public event Action OnConversationEnded;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="dialogueView"> Отображение диалогового окна. </param>
        [Inject]
        private void Construct(DialogueView dialogueView) => _dialogueView = dialogueView;

        /// <summary> Инициализация компонента и подписка на события UI. </summary>
        private void Awake()
        {
            _dialogueView.OnNextClicked += PlayNextDialogueNode;
            _dialogueView.OnChoiceSelected += SelectChoice;
        }

        /// <summary> Обработка ввода для перехода к следующей реплике. </summary>
        private void Update()
        {
            // TODO: УДАЛИТЬ КОГДА ПЕРЕПИШЕМ НА НОВЫЙ INTERACT SYSTEM
            if (!IsDialogueActive || IsChoosingDialogue ||
                !InputWrapper.GetButtonDown(InputButton.NextDialogue))
                return;

            _dialogueView.CompleteOrProceed();
        }

        /// <summary> Очистить подписки и события при уничтожении компонента. </summary>
        private void OnDestroy()
        {
            if (!_dialogueView) return;

            _dialogueView.OnNextClicked -= PlayNextDialogueNode;
            _dialogueView.OnChoiceSelected -= SelectChoice;
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

            InputWrapper.UnblockInputNextFrame(InputButton.NextDialogue, InputButton.SkipDialogue);
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

            _dialogueView.OnHidden += () => OnConversationEnded?.Invoke();
            _dialogueView.Hide();

            // Защищаем от повторного запуска взаимодействия
            InputWrapper.BlockInput(InputButton.Interact);
            InputWrapper.UnblockInputNextFrame(InputButton.Interact);
        }

        #endregion

        /// <summary> Воспроизвести следующий узел диалога. </summary>
        private void PlayNextDialogueNode()
        {
            if (!IsDialogueActive) return;

            var playerChoices = FilterOnCondition(_currentDialogue.GetPlayerChildNodes(_currentNode)).ToList();
            if (playerChoices.Any())
            {
                IsChoosingDialogue = true;
                TriggerExitAction();
                UpdateDialogueView();
                return;
            }

            var allChildren = FilterOnCondition(_currentDialogue.GetChildNodes(_currentNode)).ToList();
            if (!allChildren.Any())
            {
                EndDialogue();
                return;
            }

            var npcChoices = FilterOnCondition(_currentDialogue.GetNpcChildNodes(_currentNode)).ToList();
            var nextNode = npcChoices[Random.Range(0, npcChoices.Count)];

            TriggerExitAction();
            _currentNode = nextNode;
            TriggerEnterAction();

            IsChoosingDialogue = false;
            UpdateDialogueView();
        }

        /// <summary> Отфильтровать узлы диалога на основе выполнения условий. </summary>
        /// <param name="inputNodes"> Список узлов для фильтрации. </param>
        /// <returns> Список узлов, условия которых выполнены. </returns>
        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes) =>
            inputNodes.Where(node => node.CheckCondition(GetEvaluators()));

        /// <summary> Получить список всех компонентов-предикатов для оценки условий. </summary>
        /// <returns> Коллекция компонентов, реализующих IPredicateEvaluator. </returns>
        private IEnumerable<IPredicateEvaluator> GetEvaluators() => GetComponents<IPredicateEvaluator>();

        /// <summary> Получить список доступных для игрока вариантов ответа. </summary>
        /// <returns> Список узлов, которые представляет выбор игрока. </returns>
        private IEnumerable<DialogueNode> GetChoices() =>
            FilterOnCondition(_currentDialogue.GetPlayerChildNodes(_currentNode));

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
                _dialogueView.Hide();
                return;
            }

            var data = new DialogueData(_currentNode.Text, CurrentNpcSpeaker?.NpcInfo, IsChoosingDialogue,
                GetChoices());
            _dialogueView.Show(data);
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