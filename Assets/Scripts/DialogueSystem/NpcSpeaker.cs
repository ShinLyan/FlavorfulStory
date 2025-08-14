using FlavorfulStory.Actions;
using FlavorfulStory.AI;
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
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Диалог, который будет запущен при взаимодействии с NPC. </summary>
        [SerializeField] private Dialogue _dialogue;

        /// <summary> Компонент диалогов игрока. </summary>
        private PlayerSpeaker _playerSpeaker;

        /// <summary> Контроллер игрока. </summary>
        private PlayerController _playerController; // TODO: Возможно удалить

        /// <summary> Провайдер позиции игрока. </summary>
        private PlayerPositionProvider _playerPositionProvider;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerSpeaker"> Компонент диалогов игрока. </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        [Inject]
        private void Construct(PlayerSpeaker playerSpeaker, PlayerController playerController,
            PlayerPositionProvider playerPositionProvider)
        {
            _playerSpeaker = playerSpeaker;
            _playerController = playerController;
            _playerPositionProvider = playerPositionProvider;
        }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            IsInteractionAllowed = true;
            NpcInfo = GetComponent<Npc>().NpcInfo;

            _playerSpeaker.OnDialogueCompleted += OnDialogueCompleted;
        }

        /// <summary> Обработчик завершения диалога — завершает взаимодействие с NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <param name="dialogue"> Диалог, который завершился. </param>
        private void OnDialogueCompleted(NpcName npcName, Dialogue dialogue) => EndInteraction(_playerController);

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Talk, $"to {NpcInfo.NpcName}");

        /// <summary> Флаг, разрешено ли взаимодействие с NPC. </summary>
        public bool IsInteractionAllowed { get; private set; }

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

            IsInteractionAllowed = false;
            _playerSpeaker.StartDialogue(this, _dialogue);
        }

        /// <summary> Завершает взаимодействие с NPC. </summary>
        /// <param name="player"> Контроллер игрока, завершающий взаимодействие. </param>
        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            IsInteractionAllowed = true;
        }

        #endregion

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType CursorType => IsPlayerInRange(transform.position)
            ? CursorType.DialogueAvailable
            : CursorType.DialogueNotAvailable;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> <c>true</c>, если взаимодействие обработано. </returns>
        public bool TryInteractWithCursor(PlayerController controller)
        {
            if (!_dialogue) return false;

            if (!IsPlayerInRange(transform.position) || !InputWrapper.GetRightMouseButtonDown()) return true;

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