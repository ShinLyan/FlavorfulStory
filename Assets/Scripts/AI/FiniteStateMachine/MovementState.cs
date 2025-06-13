namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения персонажа, отвечающее за перемещение NPC к назначенным точкам. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Контроллер движения NPC для управления навигацией. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Инициализирует новое состояние движения с заданным контроллером движения. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        public MovementState(NpcMovementController movementController)
        {
            _movementController = movementController;
            _movementController.OnDestinationReached += HandleDestinationReached;
        }

        /// <summary> Входит в состояние движения и начинает перемещение к текущей точке расписания. </summary>
        public override void Enter() => _movementController.MoveToCurrentPoint();

        /// <summary> Выходит из состояния движения и останавливает NPC. </summary>
        public override void Exit() => _movementController.Stop();

        /// <summary> Обновляет состояние движения каждый кадр. </summary>
        public override void Update() { }

        /// <summary> Сбрасывает состояние движения, мгновенно останавливая NPC. </summary>
        public override void Reset() => _movementController.Stop(true);

        /// <summary> Обрабатывает событие достижения пункта назначения и переключает на состояние рутины. </summary>
        private void HandleDestinationReached() => RequestStateChange(typeof(RoutineState));
    }
}