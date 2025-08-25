using System;
using System.Collections.Generic;

namespace FlavorfulStory.AI.FSM
{
    /// <summary> Состояние, которое выполняет последовательность других состояний. </summary>
    public class SequenceState : CharacterState
    {
        /// <summary> Текущее выполняемое состояние в последовательности. </summary>
        private CharacterState _currentState;

        /// <summary> Индекс текущего состояния в последовательности. </summary>
        private int _currentStateIndex;

        /// <summary> Список состояний для выполнения в последовательности. </summary>
        private readonly List<CharacterState> _states;

        /// <summary> Событие, вызываемое при завершении всей последовательности. </summary>
        public event Action OnSequenceEnded;

        /// <summary> Инициализирует новый экземпляр состояния последовательности. </summary>
        /// <param name="states"> Коллекция состояний для выполнения в последовательности. </param>
        public SequenceState(IEnumerable<CharacterState> states) { _states = new List<CharacterState>(states); }

        /// <summary> Выполняет вход в состояние и запускает первое состояние последовательности. </summary>
        public override void Enter()
        {
            base.Enter();
            if (Context == null) Context = new StateContext();

            _currentStateIndex = 0;
            _currentState = _states[_currentStateIndex];
            _currentState.SetContext(Context);
            _currentState.Enter();
        }

        /// <summary> Обновляет текущее состояние и управляет переходом к следующему состоянию. </summary>
        public override void Update()
        {
            if (_currentState == null) return;

            _currentState.Update();

            if (!_currentState.IsComplete()) return;

            Context = _currentState.Context;
            _currentState.Exit();

            if (_currentStateIndex == _states.Count - 1)
            {
                Back();
                return;
            }

            _currentStateIndex += 1;

            _currentState = _states[_currentStateIndex];
            _currentState.SetContext(Context);
            _currentState.Enter();
        }

        /// <summary> Выполняет завершающие действия при выходе из текущего состояния. </summary>
        public override void Exit()
        {
            base.Exit();

            if (_currentState != null)
            {
                _currentState.Exit();
                _currentState = null;
            }
        }

        /// <summary> Сбрасывает состояние. </summary>
        public override void Reset()
        {
            base.Reset();

            SetContext(null);
            Exit();
        }

        /// <summary> Возвращает индекс к началу последовательности и вызывает событие завершения. </summary>
        private void Back()
        {
            _currentStateIndex = 0;
            OnSequenceEnded?.Invoke();
        }
    }
}