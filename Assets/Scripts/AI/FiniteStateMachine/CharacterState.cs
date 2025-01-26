namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Абстрактный базовый класс для состояний конечного автомата (FSM). </summary>
    public abstract class CharacterState
    {
        /// <summary> Ссылка на конечный автомат (FSM), к которому принадлежит это состояние. </summary>
        protected readonly StateController StateController;

        /// <summary>  Инициализирует новый экземпляр класса <see cref="CharacterState"/>. </summary>
        /// <param name="stateController"> Конечный автомат (FSM), к которому принадлежит это состояние. </param>
        protected CharacterState(StateController stateController)
        {
            StateController = stateController;
        }

        /// <summary> Вызывается при входе в состояние. Может быть переопределен в
        /// производных классах для реализации пользовательской логики. </summary>
        public virtual void Enter() { }

        /// <summary> Вызывается при выходе из состояния. Может быть переопределен в
        /// производных классах для реализации пользовательской логики. </summary>
        public virtual void Exit() { }

        /// <summary>  Вызывается каждый кадр для обновления логики состояния.
        /// Может быть переопределен в производных классах для реализации пользовательской логики. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public virtual void Update(float deltaTime) { }

        /// <summary> Вызывается каждый фиксированный кадр для обновления логики, связанной с физикой.
        /// Может быть переопределен в производных классах для реализации пользовательской логики. </summary>
        /// <param name="fixedDeltaTime"> Время в секундах, прошедшее с последнего фиксированного кадра. </param>
        public virtual void FixedUpdate(float fixedDeltaTime) { }
    }
}