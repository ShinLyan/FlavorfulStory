using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    public class NonInteractableNpcNavigator : NpcNavigator, INpcNavigatorMover<SchedulePoint>
    {
        public NonInteractableNpcNavigator(NavMeshAgent navMeshAgent,
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

        // public void MoveTo(Vector3 point) //TODO: подумать над тем как это сделать
        // {
        //     // var point = location.GetRandomPointOnNavMesh();
        //     _currentTargetPoint = point;
        //     _isNotMoving = false;
        //     ResumeAgent();
        //
        //     // if (_currentLocation != point.LocationName)
        //     //     StartWarpTransition(point);
        //     // else
        //     _navMeshAgent.SetDestination(point);
        // }
    }
}