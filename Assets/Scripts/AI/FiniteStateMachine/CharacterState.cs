using System;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Абстрактный базовый класс для всех состояний, используемых в конечном автомате (FSM). </summary>
    public abstract class CharacterState
    {
        /// <summary> Событие, вызываемое для запроса перехода к другому состоянию. </summary>
        public event Action<string> OnStateChangeRequested;

        protected StateContext Context { get; private set; }

        public void SetContext(StateContext context) => Context = context;

        /// <summary> Вызывается при входе в состояние. </summary>
        public virtual void Enter()
        {
            // Debug.Log("Entering state: " + GetType().Name);
        }

        /// <summary> Вызывается при выходе из состояния. </summary>
        public virtual void Exit()
        {
            // Debug.Log("Exiting state: " + GetType().Name);
        }

        /// <summary> Обновление логики состояния, вызываемое каждый кадр. </summary>
        public virtual void Update() { }

        /// <summary> Сброс состояния в начальное состояние. </summary>
        public virtual void Reset() { }

        /// <summary> Запросить смену состояния. </summary>
        /// <param name="stateType"> Тип состояния, на которое требуется перейти. </param>
        protected void RequestStateChange(Type stateType) => OnStateChangeRequested?.Invoke(stateType.ToString());

        /// <summary> Проверяет, завершено ли состояние. </summary>
        public virtual bool IsComplete() => false;
    }
}