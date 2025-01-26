using System;
using System.Collections.Generic;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний, управляющий переключением между состояниями персонажа. </summary>
    public class StateController
    {
        /// <summary> Текущее состояние персонажа. </summary>
        private CharacterState CurrentState { get; set; }

        /// <summary> Словарь, хранящий все возможные состояния персонажа, где ключ — тип состояния,
        /// а значение — экземпляр состояния. </summary>
        private Dictionary<Type, CharacterState> _states = new();

        /// <summary> Добавляет состояние в словарь состояний. </summary>
        /// <param name="state"> Экземпляр состояния, которое нужно добавить. </param>
        public void AddState(CharacterState state)
        {
            _states.Add(state.GetType(), state);
        }

        /// <summary> Устанавливает текущее состояние персонажа на состояние указанного типа. </summary>
        /// <typeparam name="T"> Тип состояния, на которое нужно переключиться. </typeparam>
        public void SetState<T>() where T : CharacterState
        {
            var type = typeof(T);

            if (CurrentState?.GetType() == type)
                return;

            if (_states.TryGetValue(type, out var newState))
            {
                CurrentState?.Exit();
                CurrentState = newState;
                CurrentState.Enter();
            }
        }

        /// <summary> Обновляет логику текущего состояния. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }

        /// <summary> Обновляет логику текущего состояния, связанную с физикой. </summary>
        /// <param name="fixedDeltaTime"> Время в секундах, прошедшее с последнего фиксированного кадра. </param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            CurrentState?.FixedUpdate(fixedDeltaTime);
        }
    }
}