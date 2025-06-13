using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние рутины NPC, в котором персонаж выполняет действия согласно расписанию. </summary>
    public class RoutineState : CharacterState, ICurrentSchedulePointDependable
    {
        #region Fields

        /// <summary> Контроллер анимации NPC для управления анимациями персонажа. </summary>
        private readonly NpcAnimatorController _animatorController;

        /// <summary> Текущая точка расписания, в которой находится NPC. </summary>
        private SchedulePoint _currentPoint;

        private bool _isGamePaused;

        #endregion

        /// <summary> Инициализирует новое состояние рутины с заданными зависимостями. </summary>
        /// <param name="animatorController"> Контроллер анимации для воспроизведения анимаций. </param>
        public RoutineState(NpcAnimatorController animatorController)
        {
            _animatorController = animatorController;
            _currentPoint = null;
        }

        /// <summary> Входит в состояние рутины и активирует необходимую логику. </summary>
        public override void Enter() { }

        /// <summary> Выходит из состояния рутины и выполняет очистку ресурсов. </summary>
        public override void Exit() => Reset();

        /// <summary> Обновляет логику состояния рутины каждый кадр и воспроизводит соответствующую анимацию. </summary>
        public override void Update() => PlayAnimation();

        /// <summary> Сбрасывает состояние рутины к начальному состоянию. </summary>
        public override void Reset() => _animatorController.Reset();

        /// <summary> Воспроизводит анимацию текущей точки расписания, если она задана. </summary>
        private void PlayAnimation()
        {
            if (_currentPoint == null) return;
            _animatorController.PlayStateAnimation(_currentPoint.NpcAnimation);
        }

        /// <summary> Устанавливает новую текущую точку расписания и переключает состояние на движение. </summary>
        /// <param name="newCurrentPont"> Новая текущая точка расписания. </param>
        public void SetNewCurrentPont(SchedulePoint newCurrentPont)
        {
            _currentPoint = newCurrentPont;
            RequestStateChange(typeof(MovementState));
        }
    }
}