using FlavorfulStory.Actions;
using FlavorfulStory.AI;
using FlavorfulStory.AI.InteractableNpc;
using FlavorfulStory.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии с игроком. </summary>
    [RequireComponent(typeof(InteractableNpc))]
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Компонент диалогов игрока. </summary>
        private PlayerSpeaker _playerSpeaker;

        /// <summary> Контроллер игрока. </summary>
        private PlayerController _playerController; // TODO: Удалить после рефакторинга InteractionSystem

        /// <summary> Провайдер позиции игрока. </summary>
        private IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Сервис диалогов. </summary>
        private DialogueService _dialogueService;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerSpeaker"> Компонент диалогов игрока. </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        /// <param name="dialogueService"> Сервис диалогов. </param>
        [Inject]
        private void Construct(PlayerSpeaker playerSpeaker, PlayerController playerController,
            IPlayerPositionProvider playerPositionProvider, DialogueService dialogueService)
        {
            _playerSpeaker = playerSpeaker;
            _playerController = playerController;
            _playerPositionProvider = playerPositionProvider;
            _dialogueService = dialogueService;
        }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            NpcInfo = GetComponent<InteractableNpc>().NpcInfo;

            _playerSpeaker.OnDialogueCompleted += OnDialogueCompleted;
        }

        /// <summary> Отписка от событий завершения диалога и окончания дня. </summary>
        private void OnDestroy() => _playerSpeaker.OnDialogueCompleted -= OnDialogueCompleted;

        /// <summary> Обработчик завершения диалога — завершает взаимодействие с NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <param name="dialogue"> Диалог, который завершился. </param>
        private void OnDialogueCompleted(NpcName npcName, Dialogue dialogue) => EndInteraction(_playerController);

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Talk, $"to {NpcInfo.NpcName}");

        /// <summary> Флаг, разрешено ли взаимодействие с NPC. </summary>
        public bool IsInteractionAllowed => IsPlayerInRange(transform.position);

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
            if (!dialogue)
            {
                player.SetBusyState(false);
                return;
            }

            _playerSpeaker.StartDialogue(this, dialogue);
        }

        /// <summary> Завершает взаимодействие с NPC. </summary>
        /// <param name="player"> Контроллер игрока, завершающий взаимодействие. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType CursorType =>
            IsInteractionAllowed ? CursorType.DialogueAvailable : CursorType.DialogueNotAvailable;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> <c>true</c>, если взаимодействие обработано. </returns>
        public bool TryInteractWithCursor(PlayerController controller)
        {
            if (!IsInteractionAllowed || !InputWrapper.GetRightMouseButtonDown()) return false;

            BeginInteraction(controller);
            controller.SetBusyState(true);
            return true;
        }

        /// <summary> Находится ли игрок в радиусе взаимодействия? </summary>
        /// <param name="position"> Позиция для проверки. </param>
        /// <returns> <c>true</c>, если игрок в радиусе взаимодействия. </returns>
        private bool IsPlayerInRange(Vector3 position) =>
            _playerPositionProvider.GetDistanceTo(position) <= InteractionController.InteractionDistance;

        #endregion
    }
}