using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    public class InteractableNpcNavigator : NpcNavigator, INpcNavigatorMover<SchedulePoint>
    {
        public InteractableNpcNavigator(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform)
            : base(navMeshAgent, warpGraph, transform)
        {
        }

        public void MoveTo(SchedulePoint point)
        {
            _currentTargetPoint = point;
            _isNotMoving = false;
            ResumeAgent();

            if (_currentLocation != point.LocationName)
                StartWarpTransition(point);
            else
                _navMeshAgent.SetDestination(point.Position);
        }

        /// <summary> Обрабатывает изменение текущей точки расписания:
        /// если персонаж в движении — остановить и перенаправить. </summary>
        public void OnSchedulePointChanged(SchedulePoint point)
        {
            if (_isNotMoving) return;

            Stop();
            MoveTo(point);
        }
    }
}