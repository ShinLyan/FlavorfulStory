using System;
using System.Collections.Generic;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний, управляющий переключением между состояниями персонажа. </summary>
    public class StateController
    {
        /// <summary> Текущее состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Словарь, хранящий все возможные состояния персонажа, где ключ — тип состояния,
        /// а значение — экземпляр состояния. </summary>
        private readonly Dictionary<Type, CharacterState> _states = new();

        /// <summary> Добавляет состояние в словарь состояний. </summary>
        /// <param name="state"> Экземпляр состояния, которое нужно добавить. </param>
        public void AddState(CharacterState state) => _states.Add(state.GetType(), state);

        /// <summary> Устанавливает текущее состояние персонажа на состояние указанного типа. </summary>
        /// <typeparam name="T"> Тип состояния, на которое нужно переключиться. </typeparam>
        public void SetState<T>() where T : CharacterState
        {
            var type = typeof(T);
            if (_currentState?.GetType() == type ||
                !_states.TryGetValue(type, out var newState)) return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        /// <summary> Обновляет логику текущего состояния. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public void Update(float deltaTime) => _currentState?.Update(deltaTime);
    }
}