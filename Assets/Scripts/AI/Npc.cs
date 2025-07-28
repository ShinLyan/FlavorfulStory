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

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        private NpcMovementController _movementController;

        /// <summary> Обработчик расписания для управления временными точками NPC. </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        private NpcAnimationController _animationController;

        /// <summary> Обработчик коллизий NPC для взаимодействия с игроком и триггерами. </summary>
        private NpcCollisionHandler _collisionHandler;

        /// <summary> Контроллер игрока, необходимый NPC для взаимодействия и логики состояний. </summary>
        private PlayerController _playerController;

        /// <summary> Внедряет зависимость от контроллера игрока. </summary>
        /// <param name="playerController"> Ссылка на компонент игрока. </param>
        [Inject]
        private void Construct(PlayerController playerController) => _playerController = playerController;

        /// <summary> Инициализация компонентов, подписка на события времени. </summary>
        private void Awake()
        {
            _animationController = new NpcAnimationController(GetComponent<Animator>());
            _npcScheduleHandler = new NpcScheduleHandler();

            WorldTime.OnTimePaused += _animationController.PauseAnimation;
            WorldTime.OnTimeUnpaused += _animationController.ContinueAnimation;
        }

        /// <summary> Выполняет инициализацию всех контроллеров и систем NPC при запуске. </summary>
        private void Start()
        {
            // TODO: Вынести всё в Awake
            _movementController = new NpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(
                    FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
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

            _collisionHandler = new NpcCollisionHandler(_stateController);
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        private void Update()
        {
            _stateController.Update();
            _movementController.UpdateMovement();
        }

        /// <summary> Обрабатывает вход другого объекта в триггер коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        private void OnTriggerEnter(Collider other) => _collisionHandler?.HandleTriggerEnter(other);

        /// <summary> Обрабатывает выход другого объекта из триггера коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        private void OnTriggerExit(Collider other) => _collisionHandler?.HandleTriggerExit(other);
    }
}