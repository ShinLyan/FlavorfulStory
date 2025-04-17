using FlavorfulStory.AI;
using FlavorfulStory.AI.States;
using FlavorfulStory.Control;
using FlavorfulStory.Control.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии курсором. </summary>
    public class NpcSpeaker : MonoBehaviour, IInteractable, ICursorInteractable
    {
        /// <summary> Диалог, который будет запущен при взаимодействии с NPC. </summary>
        [SerializeField] private Dialogue _dialogue;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        private PlayerController _playerController;

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake()
        {
            NpcInfo = GetComponent<Npc>().NpcInfo;
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            IsInteractionAllowed = true;
        }

        #region IInteractable

        public bool IsInteractionAllowed { get; private set; }

        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        public void BeginInteraction(PlayerController player)
        {
            if (!IsInteractionAllowed) return;

            player.GetComponent<PlayerSpeaker>().StartDialogue(this, _dialogue);
        }

        public void EndInteraction(PlayerController player) { }

        #endregion

        #region ITooltipable

        public string TooltipTitle => NpcInfo.NpcName.ToString();

        public string TooltipDescription => "Talk";

        public Vector3 WorldPosition => transform.position;

        #endregion

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType CursorType => _playerController.IsPlayerInRange(transform.position)
            ? CursorType.DialogueAvailable
            : CursorType.DialogueNotAvailable;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> True, если взаимодействие обработано. </returns>
        public bool TryInteractWithCursor(PlayerController controller)
        {
            if (!_dialogue) return false;

            if (Input.GetMouseButtonDown(1))
            {
                InputWrapper.BlockAllInput();
                BeginInteraction(controller);
            }

            return true;
        }

        #endregion
    }
}