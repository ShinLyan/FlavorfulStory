namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary>
    /// Класс состояния взаимодействия, наследующий базовый класс <see cref="CharacterState"/>.
    /// Используется для реализации логики, связанной с взаимодействиями в конечном автомате (FSM).
    /// </summary>
    public class InteractionState : CharacterState
    {
        /// <summary> Инициализирует новый экземпляр класса <see cref="InteractionState"/>. </summary>
        /// <param name="stateController">Конечный автомат (FSM), к которому принадлежит это состояние.</param>
        public InteractionState(StateController stateController) : base(stateController)
        {
            _stateController = stateController;
        }

        /// <summary> Вызывается при входе в состояние. 
        /// Используется для выполнения начальных действий при переходе в это состояние. </summary>
        public override void Enter()
        {
        }

        /// <summary> Вызывается при выходе из состояния.
        /// Используется для очистки или завершения действий перед выходом из состояния. </summary>
        public override void Exit()
        {
        }

        /// <summary> Вызывается в каждом кадре во время нахождения в этом состоянии.
        /// Используется для обновления логики взаимодействия. </summary>
        /// <param name="deltaTime"> Время, прошедшее с предыдущего кадра. </param>
        public override void Update(float deltaTime)
        {
        }
    }
}