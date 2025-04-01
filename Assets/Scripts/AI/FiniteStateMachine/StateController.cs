using System;
using System.Collections.Generic;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний, управляющий переключением между состояниями персонажа. </summary>
    public class StateController
    {
        #region Fields and Properties

        /// <summary> Состояние взаимодействия. </summary>
        private InteractionState _interactionState;

        /// <summary> Состояние передвижения. </summary>
        private MovementState _movementState;

        /// <summary> Состояние рутины. </summary>
        private RoutineState _routineState;

        /// <summary> Состояние ожидания. </summary>
        private WaitingState _waitingState;

        /// <summary> Текущее состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Компонент NavMeshAgent. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Компонент Transform. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Словарь, хранящий все возможные состояния персонажа, где ключ — тип состояния,
        /// а значение — экземпляр состояния. </summary>
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Точка спавна. </summary>
        private readonly Vector3 _spawnPoint;

        /// <summary> Расписания, отсортированные по приоритетам. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Событие изменения текущего расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        /// <summary> Компонент MonoBehaviour от Npc. </summary>
        private readonly MonoBehaviour _npcMonoBehaviour;

        #endregion

        /// <summary> Конструктор контроллера состояний. </summary>
        /// <param name="navMeshAgent"> Компонент NavMesh. </param>
        /// <param name="portals"> Порталы. </param>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="npcSchedule"> Набор расписаний. </param>
        /// <param name="npcTransform"> Компонент Transform. </param>
        /// <param name="coroutineRunner"> Проигрыватель корутин. </param>
        /// <param name="locations"> Локации. </param>
        public StateController(NavMeshAgent navMeshAgent, IEnumerable<WarpPortal> portals, Animator animator,
            NpcSchedule npcSchedule, Transform npcTransform, MonoBehaviour coroutineRunner,
            IEnumerable<Location> locations)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();
            _navMeshAgent = navMeshAgent;
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            if (_sortedScheduleParams == null) Debug.LogError("SortedScheduleParams is null");
            _spawnPoint = npcTransform.position;
            _npcMonoBehaviour = coroutineRunner;
            InitializeStates(portals, animator, coroutineRunner, npcTransform, locations);

            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.GetCurrentGameTime());
        }

        /// <summary> Инициализировать состояния. </summary>
        /// <param name="portals"> Порталы. </param>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="coroutineRunner"> Проигрыватель корутин. </param>
        /// <param name="npcTransform"> Компонент Transform. </param>
        /// <param name="locations"> Локации. </param>
        private void InitializeStates(IEnumerable<WarpPortal> portals, Animator animator,
            MonoBehaviour coroutineRunner, Transform npcTransform, IEnumerable<Location> locations)
        {
            _interactionState = new InteractionState();
            _movementState = new MovementState(
                _navMeshAgent,
                WarpGraph.Build(portals),
                locations,
                animator,
                npcTransform,
                coroutineRunner
            );
            _routineState = new RoutineState(animator);
            _waitingState = new WaitingState();

            var states = new CharacterState[] { _interactionState, _movementState, _routineState, _waitingState };
            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;

                if (state is IScheduleDependable dependable)
                    OnCurrentScheduleParamsChanged += dependable.SetCurrentScheduleParams;
            }
        }

        /// <summary> Обновляет логику текущего состояния. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public void Update(float deltaTime) => _currentState?.Update(deltaTime);

        /// <summary> Установить текущее состояние. </summary>
        private void SetState(Type stateType)
        {
            if (!_typeToCharacterStates.TryGetValue(stateType, out var newState) || _currentState == newState) return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        /// <summary> Обновить состояния. </summary>
        private void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values)
                state.Reset();
            SetState(typeof(RoutineState));
        }

        /// <summary> Обновление состояния NPC. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void OnReset(DateTime currentTime)
        {
            PrioritiseSchedule(currentTime);
            ResetStates();
            _navMeshAgent.Warp(_spawnPoint);
        }

        /// <summary> Приоритизировать расписание. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void PrioritiseSchedule(DateTime currentTime)
        {
            bool isRaining = false; //TODO: поменять на получение текущей погоды из спец. скрипта
            int hearts = 0; //TODO: поменять на получение текущих отношений с данным нпс

            foreach (var param in _sortedScheduleParams)
                if (param.AreConditionsSuitable(currentTime, param.Hearts, isRaining))
                {
                    OnCurrentScheduleParamsChanged?.Invoke(param);
                    return;
                }

            Debug.LogError(
                $"На текущую дату ({currentTime.DateToString()}) не подходит ни одно расписание у НПС: {_npcMonoBehaviour.name}");
            OnCurrentScheduleParamsChanged?.Invoke(null);
        }
    }
}