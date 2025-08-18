using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.AI.FSM
{
    /// <summary> Состояние движения персонажа, отвечающее за перемещение NPC к назначенным точкам. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Контроллер движения NPC для управления навигацией. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Флаг завершения состояния движения. Устанавливается в true при достижении цели. </summary>
        private bool _isComplete;

        private readonly bool _isInteractableNpc;

        /// <summary> Инициализирует новое состояние движения с заданным контроллером движения. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="isInteractableNpc"> NPC интерактивный? </param>
        public MovementState(NpcMovementController movementController, bool isInteractableNpc)
        {
            _isComplete = false;
            _movementController = movementController;
            _isInteractableNpc = isInteractableNpc;
        }

        /// <summary> Входит в состояние движения и начинает перемещение к текущей точке расписания. </summary>
        public override void Enter()
        {
            base.Enter();
            _movementController.MoveToPoint();
            _movementController.OnDestinationReached += OnCompleteMovement;

            WorldTime.OnTimePaused += StopMovementOnPause;
            WorldTime.OnTimeUnpaused += ContinueMovementOnUnpause;
        }

        /// <summary> Выходит из состояния движения и останавливает NPC. </summary>
        public override void Exit()
        {
            _movementController.Stop();
            _isComplete = false;
            _movementController.OnDestinationReached -= OnCompleteMovement;

            WorldTime.OnTimePaused -= StopMovementOnPause;
            WorldTime.OnTimeUnpaused -= ContinueMovementOnUnpause;
        }

        /// <summary> Сбрасывает состояние движения, мгновенно останавливая NPC. </summary>
        public override void Reset() => _movementController.Stop(true);

        /// <summary> Обрабатывает завершение движения к точке назначения. </summary>
        /// <remarks> Устанавливает флаг завершения и инициирует переход к состоянию рутины. </remarks>
        private void OnCompleteMovement()
        {
            _isComplete = true;
            if (_isInteractableNpc) RequestStateChange(NpcStateName.Routine);
        }

        /// <summary> Останавливает движение NPC при паузе игрового времени, если персонаж находится в состоянии движения. </summary>
        private void StopMovementOnPause() { _movementController.Stop(); }

        /// <summary> Возобновляет движение NPC при снятии паузы игрового времени, если персонаж находится в состоянии движения. </summary>
        private void ContinueMovementOnUnpause() { _movementController.MoveToPoint(); }

        /// <summary> Проверяет, завершено ли выполнение состояния движения. </summary>
        /// <returns> true, если персонаж достиг точки назначения; иначе false. </returns>
        public override bool IsComplete() => _isComplete;
    }
}