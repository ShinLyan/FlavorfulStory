using System;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary>  Абстрактный базовый класс для всех состояний, используемых в конечном автомате (FSM).
    /// Определяет основные методы, которые должны быть реализованы в производных классах. </summary>
    public abstract class CharacterState
    {
        // <summary> Событие для запроса смены состояния </summary>
        public event Action<Type> OnStateChangeRequested;

        /// <summary> Вызывается при входе в состояние. Должен быть переопределен в производных классах
        /// для реализации логики, которая выполняется при активации состояния. </summary>
        public abstract void Enter();

        /// <summary> Вызывается при выходе из состояния. Должен быть переопределен в производных классах
        /// для реализации логики, которая выполняется при деактивации состояния. </summary>
        public abstract void Exit();

        /// <summary> Вызывается каждый кадр для обновления логики состояния. Должен быть переопределен
        /// в производных классах для реализации логики, которая выполняется каждый кадр. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public abstract void Update(float deltaTime);

        public abstract void Reset();

        protected void RequestStateChange(Type stateType)
        {
            OnStateChangeRequested?.Invoke(stateType);
        }
    }
}