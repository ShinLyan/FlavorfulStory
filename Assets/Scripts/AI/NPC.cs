using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    public class NPC : MonoBehaviour
    {
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _stateController = new StateController();
        }

        private void Start()
        {
            InitializeStates();
            AddStatesToController();

            _stateController.SetState<RoutineState>();
        }

        /// <summary> Обновление логики состояний каждый кадр. </summary>
        private void Update() => _stateController.Update(Time.deltaTime);

        /// <summary> Создает экземпляры всех состояний NPC. </summary>
        private void InitializeStates()
        {
            _interactionState = new InteractionState(_stateController);
            _movementState = new MovementState(_stateController, _npcSchedule, _navMeshAgent, this,
                WarpGraph.Build(FindObjectsByType<Warp>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            );
            _routineState = new RoutineState(_stateController, _npcSchedule, this);
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

        /// <summary> Устанавливает новое расписание для NPC. </summary>
        /// <param name="newScheduleParams"> Новые параметры расписания. </param>
        public void SetNewSchedule(ScheduleParams newScheduleParams) => _currentScheduleParams = newScheduleParams;

        #region Fields and Properties

        /// <summary> Имя персонажа. </summary>
        [field: Tooltip("Имя персонажа."), SerializeField]
        public NpcName NpcName { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Текущая локация, в которой находится NPC. </summary>
        [field: Tooltip("Текущая локация, в которой находится NPC."), SerializeField]
        public LocationName CurrentLocationName { get; set; }

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private Animator _animator;

        /// <summary> Хэшированное значение параметра "скорость" для анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private NavMeshAgent _navMeshAgent;

        /// <summary> Состояние взаимодействия NPC с другими объектами. </summary>
        private InteractionState _interactionState;

        /// <summary> Состояние движения NPC по заданному маршруту. </summary>
        private MovementState _movementState;

        /// <summary> Состояние выполнения рутины, например, следование расписанию. </summary>
        private RoutineState _routineState;

        /// <summary> Состояние ожидания, когда NPC не выполняет активных действий. </summary>
        private WaitingState _waitingState;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Текущие параметры расписания NPC. </summary>
        private ScheduleParams _currentScheduleParams;

        #endregion
    }
}