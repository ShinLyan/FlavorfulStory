using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class StateControllerNonInteractableNpc : StateController
    {
        private readonly NonInteractableNpcMovementController _npcMovementController;

        private readonly Transform _npcTransform;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="playerController"> Контроллер игрока для взаимодействия. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        public StateControllerNonInteractableNpc(NonInteractableNpcMovementController npcMovementController,
            NpcAnimationController npcAnimationController,
            PlayerController playerController, Transform npcTransform)
            : base(npcAnimationController)
        {
            _npcMovementController = npcMovementController;
            _npcTransform = npcTransform;
            Initialize();
        }

        protected override void InitializeStates()
        {
            var states = new CharacterState[]
            {
                new MovementState(_npcMovementController) //TODO: new state for non-interactable NPCs
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;
            }
        }

        protected override void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(MovementState));
        }
    }
}