using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения персонажа, отвечающее за перемещение NPC к назначенным точкам. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Контроллер движения NPC для управления навигацией. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Флаг, указывающий, находится ли персонаж в данный момент в состоянии движения. </summary>
        private bool _isInState;

        /// <summary> Инициализирует новое состояние движения с заданным контроллером движения. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        public MovementState(NpcMovementController movementController)
        {
            _movementController = movementController;
            _movementController.OnDestinationReached += () => RequestStateChange(typeof(RoutineState));

            WorldTime.OnTimePaused += StopMovementOnPause;
            WorldTime.OnTimeUnpaused += ContinueMovementOnUnpause;
        }

        /// <summary> Входит в состояние движения и начинает перемещение к текущей точке расписания. </summary>
        public override void Enter()
        {
            _movementController.MoveToPoint();
            _isInState = true;
        }

        /// <summary> Выходит из состояния движения и останавливает NPC. </summary>
        public override void Exit()
        {
            _movementController.Stop();
            _isInState = false;
        }

        /// <summary> Сбрасывает состояние движения, мгновенно останавливая NPC. </summary>
        public override void Reset() => _movementController.Stop(true);

        /// <summary> Останавливает движение NPC при паузе игрового времени, если персонаж находится в состоянии движения. </summary>
        private void StopMovementOnPause()
        {
            if (_isInState) _movementController.Stop();
        }

        /// <summary> Возобновляет движение NPC при снятии паузы игрового времени, если персонаж находится в состоянии движения. </summary>
        private void ContinueMovementOnUnpause()
        {
            if (_isInState) _movementController.MoveToPoint();
        }
    }
}