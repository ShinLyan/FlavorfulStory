using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Npc : MonoBehaviour
    {
        /// <summary> Информация о персонаже. </summary>
        [field: Tooltip("Информация о персонаже."), SerializeField]
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        private NpcMovementController _movementController;

        private Animator _animator;

        private void Awake() { _animator = GetComponent<Animator>(); }

        /// <summary> Инициализация контроллера состояний и контроллера передвижения. </summary>
        private void Start()
        {
            _movementController = new NpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(
                    FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                _animator,
                transform,
                this
            );

            _stateController = new StateController(
                _npcSchedule,
                _animator,
                _movementController
            );
        }

        /// <summary> Обновление логики состояний каждый кадр. </summary>
        private void Update()
        {
            _stateController.Update(Time.deltaTime);
            _movementController.UpdateMovement();
        }
    }
}