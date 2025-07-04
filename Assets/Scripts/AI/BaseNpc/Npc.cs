using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.BaseNpc
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class Npc : MonoBehaviour
    {
        [field: SerializeField] public NpcInfo NpcInfo { get; private set; }

        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        protected NpcSchedule _npcSchedule;

        protected StateController _stateController;
        protected NpcMovementController _movementController;
        protected NpcAnimationController _animationController;

        [Inject] protected PlayerController _playerController;

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            _animationController = CreateAnimationController();
            _movementController = CreateMovementController();
            _stateController = CreateStateController();

            WorldTime.OnTimePaused += _animationController.PauseAnimation;
            WorldTime.OnTimeUnpaused += _animationController.ContinueAnimation;
        }

        protected virtual void Update()
        {
            _stateController.Update();
            _movementController.UpdateMovement();
        }

        protected virtual NpcAnimationController CreateAnimationController()
        {
            return new NpcAnimationController(GetComponent<Animator>());
        }

        protected abstract NpcMovementController CreateMovementController();

        protected abstract StateController CreateStateController();
    }
}