using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние рутины NPC, в котором персонаж выполняет действия согласно расписанию. </summary>
    public class RoutineState : CharacterState, IScheduleDependable
    {
        #region Fields

        /// <summary> Компонент Animator. </summary>
        private readonly Animator _animator;

        /// <summary> Текущая точка расписания, в которой находится NPC. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Текущее расписание. </summary>  
        private ScheduleParams _currentScheduleParams;

        #endregion

        /// <summary> Инициализирует новое состояние рутины. </summary>
        public RoutineState(Animator animator) => _animator = animator;

        /// <summary> Вызывается при входе в состояние рутины. Подписывается на событие изменения времени. </summary>
        public override void Enter() => WorldTime.OnTimeUpdated += OnNewTimeTick;

        /// <summary> Вызывается при выходе из состояния рутины. Отписывается от события изменения времени
        /// и сбрасывает текущую точку. </summary>
        public override void Exit()
        {
            Reset();
            WorldTime.OnTimeUpdated -= OnNewTimeTick;
        }

        /// <summary> Обновляет логику состояния рутины каждый кадр.
        /// Воспроизводит анимацию, если текущая точка задана. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            if (_currentPoint == null) return;

            var animationClipName = _currentPoint.NpcAnimation;
            PlayStateAnimation(animationClipName);
        }

        /// <summary> Обновить состояние. </summary>
        public override void Reset()
        {
            _currentPoint = null;
            _animator.Rebind();
        }

        /// <summary> Проверяет, изменилось ли время, и обновляет текущую точку расписания.
        /// Переключает состояние на движение, если время совпадает с точкой. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void OnNewTimeTick(DateTime currentTime)
        {
            var closestPoint = _currentScheduleParams?.GetClosestSchedulePointInPath(currentTime);
            if (closestPoint == null || closestPoint == _currentPoint) return;

            _currentPoint = closestPoint;
            if (closestPoint.Hour == (int)currentTime.Hour && closestPoint.Minutes == (int)currentTime.Minute)
                RequestStateChange(typeof(MovementState));
        }

        /// <summary> Установить новое расписание. </summary>
        /// <param name="scheduleParams"> Новое расписание. </param>
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams) => _currentScheduleParams = scheduleParams;

        /// <summary> Воспроизведение анимации состояния. </summary>
        /// <param name="animationStateName"> Название состояния анимации. </param>
        private void PlayStateAnimation(AnimationType animationStateName)
        {
            if (animationStateName == AnimationType.Idle) return;
            _animator.Play(animationStateName.ToString());
        }
    }
}