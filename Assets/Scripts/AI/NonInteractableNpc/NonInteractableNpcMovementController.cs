using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
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
            _nonInteractableNavigator.MoveTo(GetCurrentLocation());
            // TODO: Implement movement logic for NonInteractableNpc
        }

        private Location GetCurrentLocation() //TODO: убрать это
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location;

            Debug.LogWarning("No current location found for NonInteractableNpcMovementController.");
            return null;
        }
    }
}