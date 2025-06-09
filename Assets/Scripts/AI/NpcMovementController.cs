using System;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI
{
    public class NpcMovementController : IScheduleDependable
    {
        private readonly NpcNavigator _navigator;
        private readonly NpcScheduleHandler _scheduleHandler;
        private readonly NpcAnimatorController _animatorController;
        private readonly NavMeshAgent _agent;
        private SchedulePoint _currentSchedulePoint;

        public Action OnDestinationReached;

        public NpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Animator animator,
            Transform transform,
            MonoBehaviour coroutineRunner)
        {
            _agent = navMeshAgent;

            _navigator = new NpcNavigator(_agent, warpGraph, transform, coroutineRunner);
            _scheduleHandler = new NpcScheduleHandler();
            _animatorController = new NpcAnimatorController(animator);
            _currentSchedulePoint = null;

            _navigator.OnDestinationReached += () => OnDestinationReached?.Invoke();

            WorldTime.OnTimePaused += () => Stop();
            WorldTime.OnTimeUnpaused += MoveToCurrentPoint;
            WorldTime.OnTimeTick += UpdateSchedulePoint;
        }

        public void UpdateMovement()
        {
            float speed = Mathf.Clamp01(_agent.velocity.magnitude) * 0.5f;
            _animatorController.SetSpeed(speed);
            _navigator.Update();
        }

        public void MoveToCurrentPoint()
        {
            if (_currentSchedulePoint != null) _navigator.MoveTo(_currentSchedulePoint);
        }

        public void Stop(bool warp = false) => _navigator.Stop(warp);

        private void UpdateSchedulePoint(DateTime currentTime)
        {
            var nextSchedulePoint = _scheduleHandler.GetNextSchedulePoint();
            if (nextSchedulePoint == null) return;

            if ((int)currentTime.Hour == nextSchedulePoint.Hour &&
                (int)currentTime.Minute == nextSchedulePoint.Minutes)
            {
                _currentSchedulePoint = _scheduleHandler.PopNextSchedulePoint();
                _navigator.MoveTo(_currentSchedulePoint);
            }
        }

        public void SetCurrentScheduleParams(ScheduleParams scheduleParams) =>
            _scheduleHandler.SetCurrentScheduleParams(scheduleParams);
    }
}