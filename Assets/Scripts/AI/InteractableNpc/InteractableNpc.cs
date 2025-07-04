using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    [RequireComponent(typeof(NpcCollisionHandler))]
    public class InteractableNpc : Npc
    {
        private NpcCollisionHandler _collisionHandler;

        private NpcScheduleHandler _npcScheduleHandler;

        protected override void Awake() => _collisionHandler = GetComponent<NpcCollisionHandler>();

        protected override void Start()
        {
            _npcScheduleHandler = new NpcScheduleHandler();
            base.Start();
            _collisionHandler.Initialize(_stateController as ICharacterCollisionHandler);
        }

        protected override NpcMovementController CreateMovementController()
        {
            return new InteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
                _animationController,
                _npcScheduleHandler
            );
        }

        protected override StateController CreateStateController()
        {
            return new StateControllerInteractableNpc(
                _npcSchedule,
                _movementController as InteractableNpcMovementController,
                _animationController,
                _npcScheduleHandler,
                _playerController,
                transform
            );
        }
    }
}