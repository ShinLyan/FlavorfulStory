namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения, использующее контроллер перемещения </summary>
    public class MovementState : CharacterState
    {
        private readonly NpcMovementController _movementController;

        public MovementState(NpcMovementController movementController)
        {
            _movementController = movementController;
            _movementController.OnDestinationReached += HandleDestinationReached;
        }

        public override void Enter() => _movementController.MoveToNextPoint();

        public override void Exit() => _movementController.StopMovementWithoutWarp();

        public override void Update(float deltaTime) { }

        public override void Reset() => _movementController.StopMovementAndWarp();

        private void HandleDestinationReached() => RequestStateChange(typeof(RoutineState));
    }
}