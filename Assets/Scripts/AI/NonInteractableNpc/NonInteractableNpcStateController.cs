using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using UnityEngine;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class NonInteractableNpcStateController : StateController
    {
        private readonly NonInteractableNpcMovementController _npcMovementController;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        public NonInteractableNpcStateController(NonInteractableNpcMovementController npcMovementController,
            NpcAnimationController npcAnimationController)
            : base(npcAnimationController)
        {
            _npcMovementController = npcMovementController;
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

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space)) Test();
        }

        private void Test() { _currentState.Enter(); } //TODO: remove this test method

        protected override void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(MovementState));
        }
    }
}