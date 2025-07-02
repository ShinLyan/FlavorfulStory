using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние рутины NPC, в котором персонаж выполняет действия согласно расписанию. </summary>
    public class RoutineState : CharacterState, ICurrentSchedulePointDependable
    {
        #region Fields

        /// <summary> Контроллер анимации NPC для управления анимациями персонажа. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Текущая точка расписания, в которой находится NPC. </summary>
        private SchedulePoint _currentPoint;

        #endregion

        /// <summary> Инициализирует новое состояние рутины с заданными зависимостями. </summary>
        /// <param name="animationController"> Контроллер анимации для воспроизведения анимаций. </param>
        public RoutineState(NpcAnimationController animationController)
        {
            _animationController = animationController;
            _currentPoint = null;
        }

        public override void Enter() => PlayAnimation();

        /// <summary> Сбрасывает состояние рутины к начальному состоянию. </summary>
        public override void Reset() => _animationController.Reset();

        /// <summary> Воспроизводит анимацию текущей точки расписания, если она задана. </summary>
        private void PlayAnimation()
        {
            if (_currentPoint == null) return;

            _animationController.TriggerAnimation(_currentPoint.NpcAnimation);
        }

        /// <summary> Устанавливает новую текущую точку расписания и переключает состояние на движение. </summary>
        /// <param name="newCurrentPont"> Новая текущая точка расписания. </param>
        public void SetNewCurrentPoint(SchedulePoint newCurrentPont) => _currentPoint = newCurrentPont;
    }
}