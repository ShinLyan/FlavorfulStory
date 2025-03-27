using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    public class NPC : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Хэшированное значение параметра "скорость" для анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        /// <summary> Имя персонажа. </summary>
        [field: Tooltip("Имя персонажа."), SerializeField]
        public NpcName NpcName { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Текущая локация, в которой находится NPC. </summary>
        [field: Tooltip("Текущая локация, в которой находится NPC."), SerializeField]
        public LocationName CurrentLocationName { get; set; }

        /// <summary> Базовая точка спавна NPC. </summary>
        [field: Tooltip("Базовая точка спавна NPC."), SerializeField]
        private Transform _spawnPoint;

        /// <summary> Базовая точка спавна NPC. </summary>
        private LocationName _spawnLocation;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private Animator _animator;

        /// <summary> Текущие параметры расписания NPC. </summary>
        public ScheduleParams CurrentScheduleParams { get; private set; }

        /// <summary> Состояние взаимодействия NPC с другими объектами. </summary>
        private InteractionState _interactionState;

        /// <summary> Состояние движения NPC по заданному маршруту. </summary>
        private MovementState _movementState;

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private NavMeshAgent _navMeshAgent;

        /// <summary> Состояние выполнения рутины, например, следование расписанию. </summary>
        private RoutineState _routineState;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Состояние ожидания, когда NPC не выполняет активных действий. </summary>
        private WaitingState _waitingState;

        #endregion

        /// <summary> Получение компонентов. </summary>
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _stateController = new StateController();
        }

        /// <summary>  </summary>
        private void Start()
        {
            _spawnLocation = CurrentLocationName;
            InitializeStates();
            AddStatesToController();

            _stateController.SetState<RoutineState>();
            OnReset(WorldTime.GetCurrentGameTime());
        }

        /// <summary> Обновление логики состояний каждый кадр. </summary>
        private void Update() => _stateController.Update(Time.deltaTime);

        /// <summary> Подписка на события. </summary>
        private void OnEnable()
        {
            WorldTime.OnDayEnded += OnReset;
        }

        /// <summary> Отписка от событий. </summary>
        private void OnDisable()
        {
            WorldTime.OnDayEnded -= OnReset;
        }

        /// <summary> Создает экземпляры всех состояний NPC. </summary>
        private void InitializeStates()
        {
            _interactionState = new InteractionState(_stateController);
            _movementState = new MovementState(_stateController, _navMeshAgent, this,
                WarpGraph.Build(FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            );
            _routineState = new RoutineState(_stateController, this);
            _waitingState = new WaitingState(_stateController);
        }

        /// <summary> Добавляет состояния в контроллер. </summary>
        private void AddStatesToController()
        {
            _stateController.AddState(_interactionState);
            _stateController.AddState(_movementState);
            _stateController.AddState(_routineState);
            _stateController.AddState(_waitingState);
        }

        /// <summary> Воспроизведение анимации движения. </summary>
        /// <param name="speed"> Скорость движения. </param>
        /// <param name="dampTime"> Время сглаживания перехода анимации. </param>
        public void PlayMoveAnimation(float speed, float dampTime = 0.2f)
        {
            _animator.SetFloat(_speedParameterHash, speed, dampTime, Time.deltaTime);
        }

        /// <summary> Воспроизведение анимации состояния. </summary>
        /// <param name="animationStateName"> Название состояния анимации. </param>
        public void PlayStateAnimation(NpcAnimationClipName animationStateName)
        {
            _animator.Play(animationStateName.ToString());
        }

        /// <summary> Приоритизировать расписание. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void PrioritiseSchedule(DateTime currentTime)
        {
            var sortedList = _npcSchedule.GetSortedScheduleParams();

            var isRaining = false; //TODO: поменять на получение текущей погоды из спец. скрипта
            var hearts = 0; //TODO: поменять на получение текущих отношений с данным нпс

            foreach (var param in sortedList)
                if (param.AreConditionsSuitable(currentTime, param.Hearts, isRaining))
                {
                    SetNewSchedule(param);
                    return;
                }

            Debug.LogError("На текущую дату не подходит ни одно расписание!");
        }

        /// <summary> Устанавливает новое расписание для NPC. </summary>
        /// <param name="newScheduleParams"> Новые параметры расписания. </param>
        private void SetNewSchedule(ScheduleParams newScheduleParams) => CurrentScheduleParams = newScheduleParams;

        /// <summary> Обновление состояния NPC. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void OnReset(DateTime currentTime)
        {
            SetNewSchedule(null);

            _movementState.StopCoroutine();
            _navMeshAgent.ResetPath();
            _navMeshAgent.Warp(_spawnPoint.position);

            CurrentLocationName = _spawnLocation;
            PrioritiseSchedule(currentTime);
        }
    }
}