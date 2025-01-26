namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> ыКласс состояния взаимодействия, наследующий базовый класс <see cref="CharacterState"/>.
    /// Используется для реализации логики, связанной с взаимодействиями в конечном автомате (FSM). </summary>
    public class InteractionState : CharacterState
    {
        /// <summary> Ссылка на конечный автомат (FSM), к которому принадлежит это состояние. </summary>
        private StateController _stateController;

        /// <summary> Инициализирует новый экземпляр класса <see cref="InteractionState"/>.  </summary>
        /// <param name="stateController"> Конечный автомат (FSM), к которому принадлежит это состояние. </param>
        public InteractionState(StateController stateController) : base(stateController)
        {
            _stateController = stateController;
        }
    }
}