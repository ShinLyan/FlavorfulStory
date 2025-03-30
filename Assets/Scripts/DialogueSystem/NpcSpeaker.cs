using FlavorfulStory.AI;
using FlavorfulStory.AI.States;
using FlavorfulStory.Control;
using FlavorfulStory.Control.CursorSystem;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, позволяющий NPC инициировать диалог при взаимодействии курсором. </summary>
    public class NpcSpeaker : MonoBehaviour, ICursorInteractable
    {
        /// <summary> Диалог, который будет запущен при взаимодействии с NPC. </summary>
        [SerializeField] private Dialogue _dialogue;

        /// <summary> Информация о NPC. </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Инициализация свойств класса. </summary>
        private void Awake() => NpcInfo = GetComponent<Npc>().NpcInfo;

        #region ICursorInteractable

        /// <summary> Возвращает тип курсора "Диалог" при наведении на NPC. </summary>
        /// <returns> Тип курсора Dialogue. </returns>
        public CursorType GetCursorType() => CursorType.Dialogue;

        /// <summary> Обрабатывает взаимодействие игрока с NPC: запускает диалог по правому клику. </summary>
        /// <param name="controller"> Контроллер игрока, инициирующий взаимодействие. </param>
        /// <returns> True, если взаимодействие обработано. </returns>
        public bool HandleCursorInteraction(PlayerController controller)
        {
            if (!_dialogue) return false;

            if (Input.GetMouseButtonDown(1))
                controller.GetComponent<PlayerSpeaker>().StartDialogue(this, _dialogue);
            return true;
        }

        #endregion
    }
}