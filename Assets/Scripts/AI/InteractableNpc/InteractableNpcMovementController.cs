using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    public class InteractableNpcMovementController : NpcMovementController
    {
        private readonly NpcScheduleHandler _scheduleHandler;

        private readonly InteractableNpcNavigator _interactableNavigator;

        public InteractableNpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            NpcAnimationController animationController,
            NpcScheduleHandler scheduleHandler)
            : base(navMeshAgent, warpGraph, transform, animationController)
        {
            _scheduleHandler = scheduleHandler;
            _interactableNavigator = (InteractableNpcNavigator)_navigator;

            _interactableNavigator.OnDestinationReached += () => OnDestinationReached?.Invoke();
            _scheduleHandler.OnSchedulePointChanged += _interactableNavigator.OnSchedulePointChanged;
        }

        protected override INpcNavigator CreateNavigator(NavMeshAgent agent, WarpGraph graph, Transform transform)
        {
            return new InteractableNpcNavigator(agent, graph, transform);
        }

        public override void MoveToPoint()
        {
            if (_scheduleHandler.CurrentPoint != null) _interactableNavigator.MoveTo(_scheduleHandler.CurrentPoint);
        }
    }
}