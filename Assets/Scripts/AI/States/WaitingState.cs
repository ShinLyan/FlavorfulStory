namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние ожидания NPC, в котором персонаж не выполняет активных действий. </summary>
    public class WaitingState : CharacterState
    {
        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Инициализирует новое состояние ожидания. </summary>
        /// <param name="stateController"> Контроллер состояний. </param>
        public WaitingState(StateController stateController) : base(stateController)
        {
            _stateController = stateController;
        }
    }
}