using FlavorfulStory.AI;
using FlavorfulStory.CursorSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии курсором. </summary>
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Диалог, который будет запущен при взаимодействии с NPC. </summary>
        [SerializeField] private Dialogue _dialogue;

        /// <summary> Инициатор диалога, связанный с игроком. </summary>
        private IDialogueInitiator _dialogueInitiator;

        private PlayerController _playerController;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        [Inject]
        private void Construct(IDialogueInitiator dialogueInitiator, PlayerController playerController)
        {
            _dialogueInitiator = dialogueInitiator;
            _playerController = playerController;
        }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            IsInteractionAllowed = true;
            NpcInfo = GetComponent<Npc>().NpcInfo;

            if (_dialogueInitiator is PlayerSpeaker playerSpeaker)
                playerSpeaker.OnConversationEnded += () => EndInteraction(_playerController);
        }

        #region IInteractable

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
            _dialogueInitiator?.StartDialogue(this, _dialogue);
        }

        /// <summary> Завершает взаимодействие с NPC. </summary>
        /// <param name="player"> Контроллер игрока, завершающий взаимодействие. </param>
        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            IsInteractionAllowed = true;
        }

        #endregion

        #region ITooltipable

        /// <summary> Возвращает заголовок тултипа. </summary>
        /// <returns> Строка с заголовком тултипа. </returns>
        public string TooltipTitle => NpcInfo.NpcName.ToString();

        /// <summary> Возвращает описание тултипа. </summary>
        /// <returns> Строка с описанием тултипа. </returns>
        public string TooltipDescription => "Speak";

        /// <summary> Возвращает мировую позицию объекта. </summary>
        /// <returns> Вектор позиции объекта в мировых координатах. </returns>
        public Vector3 WorldPosition => transform.position;

        #endregion

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType CursorType => PlayerModel.IsPlayerInRange(transform.position)
            ? CursorType.DialogueAvailable
            : CursorType.DialogueNotAvailable;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> True, если взаимодействие обработано. </returns>
        public bool TryInteractWithCursor(PlayerController controller)
        {
            if (!_dialogue) return false;

            if (PlayerModel.IsPlayerInRange(transform.position) && Input.GetMouseButtonDown(1))
            {
                BeginInteraction(controller);
                controller.SetBusyState(true);
            }

            return true;
        }

        #endregion
    }
}