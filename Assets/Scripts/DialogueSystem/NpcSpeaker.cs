using FlavorfulStory.AI;
using FlavorfulStory.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии курсором. </summary>
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Диалог, который будет запущен при взаимодействии с NPC. </summary>
        [SerializeField] private Dialogue _dialogue;

        private IDialogueInitiator _dialogueInitiator;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            IsInteractionAllowed = true;
            NpcInfo = GetComponent<Npc>().NpcInfo;

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            _dialogueInitiator = playerObject.GetComponent<IDialogueInitiator>();

            if (_dialogueInitiator is PlayerSpeaker playerSpeaker)
                playerSpeaker.OnConversationEnded +=
                    () => EndInteraction(playerObject.GetComponent<PlayerController>());
        }

        #region IInteractable

        public bool IsInteractionAllowed { get; private set; }

        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        public void BeginInteraction(PlayerController player)
        {
            if (!IsInteractionAllowed) return;

            _dialogueInitiator?.StartDialogue(this, _dialogue);
        }

        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion

        #region ITooltipable

        /// <summary> Возвращает заголовок тултипа. </summary>
        /// <returns> Строка с заголовком тултипа. </returns>
        public string TooltipTitle => NpcInfo.NpcName.ToString();

        /// <summary> Возвращает описание тултипа. </summary>
        /// <returns> Строка с описанием тултипа. </returns>
        public string TooltipDescription => "Talk";

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