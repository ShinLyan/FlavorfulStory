using System;

namespace FlavorfulStory.AI.FSM
{
    /// <summary> Абстрактный базовый класс для всех состояний, используемых в конечном автомате (FSM). </summary>
    public abstract class CharacterState
    {
        /// <summary> Событие, вызываемое для запроса перехода к другому состоянию. </summary>
        public event Action<StateName> OnStateChangeRequested;

        /// <summary> Контекст состояния, содержащий общие данные и зависимости. </summary>
        public StateContext Context { get; protected set; }

        /// <summary> Устанавливает контекст состояния с общими данными и зависимостями. </summary>
        /// <param name="context"> Контекст состояния для установки. </param>
        public void SetContext(StateContext context) => Context = context;

        /// <summary> Вызывается при входе в состояние. </summary>
        public virtual void Enter() { }

        /// <summary> Вызывается при выходе из состояния. </summary>
        public virtual void Exit() { }

        /// <summary> Обновление логики состояния, вызываемое каждый кадр. </summary>
        public virtual void Update() { }

        /// <summary> Сброс состояния в начальное состояние. </summary>
        public virtual void Reset() { }

        /// <summary> Запросить смену состояния. </summary>
        /// <param name="stateName"> Тип состояния, на которое требуется перейти. </param>
        protected void RequestStateChange(StateName stateName) => OnStateChangeRequested?.Invoke(stateName);

        /// <summary> Проверяет, завершено ли состояние. </summary>
        public virtual bool IsComplete() => false;
    }
}