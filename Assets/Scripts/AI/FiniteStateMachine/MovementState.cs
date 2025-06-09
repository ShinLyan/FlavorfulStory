namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class MovementState : CharacterState
    {
        private readonly NpcMovementController _movementController;

        public MovementState(NpcMovementController movementController)
        {
            _movementController = movementController;
            _movementController.OnDestinationReached += HandleDestinationReached;
        }

        public override void Enter() => _movementController.MoveToCurrentPoint();

        public override void Exit() => _movementController.Stop();

        public override void Update(float deltaTime) { }

        public override void Reset() => _movementController.Stop(true);

        private void HandleDestinationReached() => RequestStateChange(typeof(RoutineState));
    }
}