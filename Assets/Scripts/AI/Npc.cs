using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(NpcCollisionHandler))]
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

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        private NpcMovementController _movementController;

        /// <summary> Обработчик расписания для управления временными точками NPC. </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        private NpcAnimationController _animationController;

        private NpcCollisionHandler _collisionHandler;

        [Inject] private PlayerController _playerController;

        private void Awake() { _collisionHandler = GetComponent<NpcCollisionHandler>(); }

        /// <summary> Выполняет инициализацию всех контроллеров и систем NPC при запуске. </summary>
        private void Start()
        {
            _animationController = new NpcAnimationController(GetComponent<Animator>());
            _npcScheduleHandler = new NpcScheduleHandler();

            _movementController = new NpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(
                    FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
                this,
                _animationController,
                _npcScheduleHandler
            );

            _stateController = new StateController(
                _npcSchedule,
                _movementController,
                _animationController,
                _npcScheduleHandler,
                _playerController,
                transform
            );

            _collisionHandler.Initialize(_stateController);

            WorldTime.OnTimePaused += _animationController.PauseAnimation;
            WorldTime.OnTimeUnpaused += _animationController.ContinueAnimation;
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        private void Update()
        {
            _stateController.Update();
            _movementController.UpdateMovement();
        }
    }
}