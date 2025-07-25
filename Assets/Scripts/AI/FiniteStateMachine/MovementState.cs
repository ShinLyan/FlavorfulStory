using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения персонажа, отвечающее за перемещение NPC к назначенным точкам. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Контроллер движения NPC для управления навигацией. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Находится ли персонаж в данный момент в состоянии движения? </summary>
        private bool _isInState;

        /// <summary> Флаг завершения состояния движения. Устанавливается в true при достижении цели. </summary>
        private bool _isComplete;

        /// <summary> Инициализирует новое состояние движения с заданным контроллером движения. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        public MovementState(NpcMovementController movementController)
        {
            _isComplete = false;
            _movementController = movementController;

            WorldTime.OnTimePaused += StopMovementOnPause;
            WorldTime.OnTimeUnpaused += ContinueMovementOnUnpause;
        }

        /// <summary> Входит в состояние движения и начинает перемещение к текущей точке расписания. </summary>
        public override void Enter()
        {
            base.Enter();
            _movementController.MoveToPoint();
            _isInState = true;
            _movementController.OnDestinationReached += OnCompleteMovement;
        }

        /// <summary> Выходит из состояния движения и останавливает NPC. </summary>
        public override void Exit()
        {
            _movementController.Stop();
            _isInState = false;
            _isComplete = false;
            _movementController.OnDestinationReached -= OnCompleteMovement;
        }

        /// <summary> Сбрасывает состояние движения, мгновенно останавливая NPC. </summary>
        public override void Reset() => _movementController.Stop(true);

        /// <summary> Обрабатывает завершение движения к точке назначения. </summary>
        /// <remarks> Устанавливает флаг завершения и инициирует переход к состоянию рутины. </remarks>
        private void OnCompleteMovement()
        {
            _isComplete = true;
            RequestStateChange(StateName.Routine);
        }

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

        /// <summary> Проверяет, завершено ли выполнение состояния движения. </summary>
        /// <returns> true, если персонаж достиг точки назначения; иначе false. </returns>
        public override bool IsComplete() => _isComplete;
    }
}