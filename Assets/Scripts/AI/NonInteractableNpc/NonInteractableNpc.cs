using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    [RequireComponent(typeof(ItemHandler))]
    public class NonInteractableNpc : Npc
    {
        [Inject] private LocationManager _locationManager;

        private ItemHandler _itemHandler;

        protected override void Awake() { _itemHandler = GetComponent<ItemHandler>(); }

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
                _locationManager,
                _animationController,
                _itemHandler
            );
        }
    }
}