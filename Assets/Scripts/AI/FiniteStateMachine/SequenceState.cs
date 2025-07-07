using System;
using System.Collections.Generic;
using FlavorfulStory.AI.BaseNpc;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние, которое выполняет последовательность других состояний. </summary>
    public class SequenceState : CharacterState
    {
        private readonly Queue<CharacterState> _statesQueue;
        private CharacterState _currentState;
        private readonly StateController _stateController;

        public SequenceState(StateController stateController, IEnumerable<CharacterState> states)
        {
            _stateController = stateController;
            _statesQueue = new Queue<CharacterState>(states);
        }

        public override void Enter()
        {
            if (_statesQueue.Count == 0)
            {
                Exit();
                return;
            }

            _currentState = _statesQueue.Dequeue();
            _currentState.SetContext(Context);
            _currentState.OnStateChangeRequested += OnSubStateChangeRequested;
            _currentState.Enter();
        }

        private void OnSubStateChangeRequested(Type type)
        {
            // Игнорируем запросы на смену состояния от подсостояний
        }

        public override void Update()
        {
            if (_currentState == null) return;

            _currentState.Update();

            if (_currentState.IsComplete()) //TODO: добавить IsComplete во все состояния
            {
                _currentState.Exit();
                _currentState.OnStateChangeRequested -= OnSubStateChangeRequested;

                if (_statesQueue.Count == 0)
                {
                    Exit();
                    return;
                }

                _currentState = _statesQueue.Dequeue();
                _currentState.SetContext(Context);
                _currentState.OnStateChangeRequested += OnSubStateChangeRequested;
                _currentState.Enter();
            }
        }

        public override void Exit()
        {
            if (_currentState != null)
            {
                _currentState.Exit();
                _currentState.OnStateChangeRequested -= OnSubStateChangeRequested;
            }

            _stateController.ReturnFromSequence();
        }

        public override void Reset() { }
    }
}