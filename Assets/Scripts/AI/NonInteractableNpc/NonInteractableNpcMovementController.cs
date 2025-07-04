using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    public class NonInteractableNpcMovementController : NpcMovementController
    {
        private readonly NonInteractableNpcNavigator _nonInteractableNavigator;

        public NonInteractableNpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            NpcAnimationController animationController)
            : base(navMeshAgent, warpGraph, transform, animationController)
        {
            _nonInteractableNavigator = (NonInteractableNpcNavigator)_navigator;

            _nonInteractableNavigator.OnDestinationReached += () => OnDestinationReached?.Invoke();
        }

        protected override INpcNavigator CreateNavigator(NavMeshAgent agent, WarpGraph graph, Transform transform)
        {
            return new NonInteractableNpcNavigator(agent, graph, transform);
        }

        public override void MoveToPoint()
        {
            // if (_scheduleHandler.CurrentPoint != null) _nonInteractableNavigator.MoveTo(_scheduleHandler.CurrentPoint); // TODO: Implement movement logic for NonInteractableNpc
        }
    }
}