using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    public class NonInteractableNpc : Npc
    {
        protected override NpcMovementController CreateMovementController()
        {
            return new NonInteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
                _animationController
            );
        }

        protected override StateController CreateStateController()
        {
            return new NonInteractableNpcStateController(
                _movementController as NonInteractableNpcMovementController,
                _animationController
            );
        }
    }
}