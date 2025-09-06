using FlavorfulStory.Actions;
using FlavorfulStory.AI;
using FlavorfulStory.CursorSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии с игроком. </summary>
    [RequireComponent(typeof(Npc))]
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Компонент диалогов игрока. </summary>
        private PlayerSpeaker _playerSpeaker;

        /// <summary> Контроллер игрока. </summary>
        private PlayerController _playerController;

        /// <summary> Сервис диалогов. </summary>
        private DialogueService _dialogueService;

        /// <summary> Количество проигранных контекстных диалогов. </summary>
        private int _timesContextTalked;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerSpeaker"> Компонент диалогов игрока. </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="dialogueService"> Сервис диалогов. </param>
        [Inject]
        private void Construct(PlayerSpeaker playerSpeaker, PlayerController playerController,
            DialogueService dialogueService)
        {
            _playerSpeaker = playerSpeaker;
            _playerController = playerController;
            _dialogueService = dialogueService;
        }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            NpcInfo = GetComponent<Npc>().NpcInfo;

            _playerSpeaker.OnDialogueCompleted += OnDialogueCompleted;
            WorldTime.OnDayEnded += OnDayEnded;
        }

        /// <summary> Отписка от событий завершения диалога и окончания дня. </summary>
        private void OnDestroy()
        {
            _playerSpeaker.OnDialogueCompleted -= OnDialogueCompleted;
            WorldTime.OnDayEnded -= OnDayEnded;
        }

        /// <summary> Обнуляет счётчик контекстных диалогов при завершении игрового дня. </summary>
        private void OnDayEnded(DateTime _) => _timesContextTalked = 0;

        /// <summary> Обработчик завершения диалога — завершает взаимодействие с NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <param name="dialogue"> Диалог, который завершился. </param>
        private void OnDialogueCompleted(NpcName npcName, Dialogue dialogue) => EndInteraction(_playerController);

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Talk, $"to {NpcInfo.NpcName}");

        /// <summary> Флаг, разрешено ли взаимодействие с NPC. </summary>
        public bool IsInteractionAllowed => _timesContextTalked < 3;

        /// <summary> Вычисляет расстояние до другого трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Начинает взаимодействие с NPC. </summary>
        /// <param name="player"> Контроллер игрока, инициирующий взаимодействие. </param>
        public void BeginInteraction(PlayerController player)
        {
            if (!IsInteractionAllowed) return;

            var dialogue = _dialogueService.GetDialogue(NpcInfo);
            _playerSpeaker.StartDialogue(this, dialogue);

            // if (dialogue.DialogueType == DialogueType.Context) _timesContextTalked++;
        }

        /// <summary> Завершает взаимодействие с NPC. </summary>
        /// <param name="player"> Контроллер игрока, завершающий взаимодействие. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType CursorType => IsInteractionAllowed && PlayerModel.IsPlayerInRange(transform.position)
            ? CursorType.DialogueAvailable
            : CursorType.DialogueNotAvailable;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> True, если взаимодействие обработано. </returns>
        public bool TryInteractWithCursor(PlayerController controller)
        {
            if (IsInteractionAllowed && PlayerModel.IsPlayerInRange(transform.position) && Input.GetMouseButtonDown(1))
            {
                BeginInteraction(controller);
                controller.SetBusyState(true);
            }

            return true;
        }

        #endregion
    }
}