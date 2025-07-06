using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    public class NonInteractableNpcNavigator : NpcNavigator, INpcNavigatorMover<Location>
    {
        public NonInteractableNpcNavigator(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform)
            : base(navMeshAgent, warpGraph, transform)
        {
        }

        public void MoveTo(Location location)
        {
            var point = location.GetRandomPointOnNavMesh();
            // _currentTargetPoint = point; //TODO: Use Location instead of SchedulePoint
            _isNotMoving = false;
            ResumeAgent();

            // if (_currentLocation != point.LocationName)
            //     StartWarpTransition(point);
            // else
            _navMeshAgent.SetDestination(point);
        }
    }
}