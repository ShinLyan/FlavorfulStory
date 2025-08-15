using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние ожидания NPC, в котором персонаж не выполняет активных действий. </summary>
    public class WaitingState : CharacterState
    {
        /// <summary> Провайдер позиции игрока. </summary>
        private readonly IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Трансформ NPC, необходимый для поворота в сторону игрока. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Конструктор состояния ожидания NPC. </summary>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        /// <param name="npcTransform"> Трансформ NPC, к которому применяется поворот. </param>
        public WaitingState(IPlayerPositionProvider playerPositionProvider, Transform npcTransform)
        {
            _playerPositionProvider = playerPositionProvider;
            _npcTransform = npcTransform;
        }

        /// <summary> Обновляет поведение NPC: поворачивает его к игроку. </summary>
        public override void Update()
        {
            _npcTransform.rotation = Quaternion.LookRotation(
                _playerPositionProvider.GetPlayerPosition() - _npcTransform.position);
        }
    }
}