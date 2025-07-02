using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние ожидания NPC, в котором персонаж не выполняет активных действий. </summary>
    public class WaitingState : CharacterState
    {
        private readonly PlayerController _playerController;

        private readonly Transform _npcTransform;

        public WaitingState(PlayerController playerController, Transform npcTransform)
        {
            _playerController = playerController;
            _npcTransform = npcTransform;
        }

        public override void Update()
        {
            _npcTransform.rotation =
                Quaternion.LookRotation(_playerController.transform.position - _npcTransform.position);
        }
    }
}