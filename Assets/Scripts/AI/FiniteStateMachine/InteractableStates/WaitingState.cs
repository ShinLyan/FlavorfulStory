using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние ожидания NPC, в котором персонаж не выполняет активных действий. </summary>
    public class WaitingState : CharacterState
    {
        /// <summary> Ссылка на контроллер игрока, используемая для определения его позиции. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Трансформ NPC, необходимый для поворота в сторону игрока. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Конструктор состояния ожидания NPC. </summary>
        /// <param name="playerController"> Ссылка на контроллер игрока. </param>
        /// <param name="npcTransform"> Трансформ NPC, к которому применяется поворот. </param>
        public WaitingState(PlayerController playerController, Transform npcTransform)
        {
            _playerController = playerController;
            _npcTransform = npcTransform;
        }

        /// <summary> Обновляет поведение NPC: поворачивает его к игроку. </summary>
        public override void Update()
        {
            _npcTransform.rotation =
                Quaternion.LookRotation(_playerController.transform.position - _npcTransform.position);
        }
    }
}