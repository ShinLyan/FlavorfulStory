using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.AI.FSM
{
    /// <summary> Состояние движения персонажа, отвечающее за перемещение NPC к назначенным точкам. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Контроллер движения NPC для управления навигацией. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Контроллер анимаций NPC. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Флаг завершения состояния движения. Устанавливается в true при достижении цели. </summary>
        private bool _isComplete;

        /// <summary> Текущая целевая точка назначения для движения. </summary>
        private NpcDestinationPoint _destinationPoint;

        /// <summary> Инициализирует новое состояние движения с заданным контроллером движения. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="animationController"> Контроллер анимаций NPC. </param>
        public MovementState(NpcMovementController movementController, NpcAnimationController animationController)
        {
            _movementController = movementController;
            _animationController = animationController;
            _isComplete = false;
        }

        /// <summary> Входит в состояние движения и начинает перемещение к текущей точке расписания. </summary>
        public override void Enter()
        {
            base.Enter();

            if (Context.TryGet(FsmContextType.DestinationPoint, out NpcDestinationPoint destinationPoint))
            {
                _destinationPoint = destinationPoint;
                _movementController.MoveToPoint(destinationPoint);
                _animationController.TriggerAnimation(AnimationType.Locomotion);
            }

            _movementController.OnDestinationReached += OnCompleteMovement;

            WorldTime.OnTimePaused += StopMovementOnPause;
            WorldTime.OnTimeUnpaused += ContinueMovementOnUnpause;
        }

        /// <summary> Выходит из состояния движения и останавливает NPC. </summary>
        public override void Exit()
        {
            base.Exit();

            _isComplete = false;

            _movementController.Stop();
            _movementController.OnDestinationReached -= OnCompleteMovement;

            WorldTime.OnTimePaused -= StopMovementOnPause;
            WorldTime.OnTimeUnpaused -= ContinueMovementOnUnpause;
        }

        /// <summary> Сбрасывает состояние движения, мгновенно останавливая NPC. </summary>
        public override void Reset()
        {
            base.Reset();

            _movementController.Stop(true);
        }

        /// <summary> Обрабатывает завершение движения к точке назначения. </summary>
        /// <remarks> Устанавливает флаг завершения и инициирует переход к состоянию рутины. </remarks>
        private void OnCompleteMovement() => _isComplete = true;

        /// <summary> Останавливает движение NPC при паузе игрового времени, если персонаж находится в состоянии движения. </summary>
        private void StopMovementOnPause() => _movementController.Stop();

        /// <summary> Возобновляет движение NPC при снятии паузы игрового времени, если персонаж находится в состоянии движения. </summary>
        private void ContinueMovementOnUnpause() => _movementController.MoveToPoint(_destinationPoint);

        /// <summary> Проверяет, завершено ли выполнение состояния движения. </summary>
        /// <returns> true, если персонаж достиг точки назначения; иначе false. </returns>
        public override bool IsComplete() => _isComplete;
    }
}