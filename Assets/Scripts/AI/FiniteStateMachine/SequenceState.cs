using System;
using System.Collections.Generic;
using FlavorfulStory.AI.BaseNpc;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние, которое выполняет последовательность других состояний. </summary>
    public class SequenceState : CharacterState
    {
        private CharacterState _currentState;
        private readonly StateController _stateController;
        private int _currentStateIndex;
        private readonly List<CharacterState> _states;
        public event Action OnSequenceEnded;

        public SequenceState(StateController stateController, IEnumerable<CharacterState> states)
        {
            _stateController = stateController;
            _states = new List<CharacterState>(states);
            _currentStateIndex = 0;
        }

        public override void Enter()
        {
            if (_currentStateIndex == _states.Count - 1)
            {
                Back();
                return;
            }

            SetContext(new StateContext());

            _currentState = _states[_currentStateIndex];
            _currentState.SetContext(Context);
            _currentState.OnStateChangeRequested += OnSubStateChangeRequested;
            _currentState.Enter();
        }

        private void OnSubStateChangeRequested(string type) { }

        public override void Update()
        {
            if (_currentState == null) return;

            _currentState.Update();

            if (_currentState.IsComplete())
            {
                Context = _currentState.Context;
                _currentState.Exit();
                _currentState.OnStateChangeRequested -= OnSubStateChangeRequested;

                if (_currentStateIndex == _states.Count - 1)
                {
                    Back();
                    return;
                }

                _currentStateIndex += 1;

                _currentState = _states[_currentStateIndex];
                _currentState.SetContext(Context);
                _currentState.OnStateChangeRequested += OnSubStateChangeRequested;
                _currentState.Enter();
            }
        }

        private void Back()
        {
            _currentStateIndex = 0;
            OnSequenceEnded?.Invoke();
        }
    }
}