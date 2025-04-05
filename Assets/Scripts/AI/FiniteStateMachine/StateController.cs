using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Object = UnityEngine.Object;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний, управляющий переключением между состояниями персонажа. </summary>
    public class StateController
    {
        #region Fields and Properties

        /// <summary> Текущее состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Компонент NavMeshAgent. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Словарь, хранящий все возможные состояния персонажа, где ключ — тип состояния,
        /// а значение — экземпляр состояния. </summary>
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Точка спавна. </summary>
        private readonly Vector3 _spawnPoint;

        /// <summary> Расписания, отсортированные по приоритетам. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Событие изменения текущего расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        #endregion

        /// <summary> Конструктор контроллера состояний. </summary>
        /// <param name="navMeshAgent"> Компонент NavMesh. </param>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="npcSchedule"> Набор расписаний. </param>
        /// <param name="npcTransform"> Компонент Transform. </param>
        /// <param name="coroutineRunner"> Проигрыватель корутин. </param>
        public StateController(NavMeshAgent navMeshAgent, Animator animator,
            NpcSchedule npcSchedule, Transform npcTransform, MonoBehaviour coroutineRunner)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();
            _navMeshAgent = navMeshAgent;
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            if (_sortedScheduleParams == null) Debug.LogError("SortedScheduleParams is null");
            _spawnPoint = npcTransform.position;
            InitializeStates(animator, coroutineRunner, npcTransform);

            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.GetCurrentGameTime());
        }

        /// <summary> Инициализировать состояния. </summary>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="coroutineRunner"> Проигрыватель корутин. </param>
        /// <param name="npcTransform"> Компонент Transform. </param>
        private void InitializeStates(Animator animator,
            MonoBehaviour coroutineRunner, Transform npcTransform)
        {
            var states = new CharacterState[]
            {
                new InteractionState(),
                new MovementState(
                    _navMeshAgent,
                    WarpGraph.Build(
                        Object.FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                    animator,
                    npcTransform,
                    coroutineRunner
                ),
                new RoutineState(animator),
                new WaitingState()
            };

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

        /// <summary> Обновление состояния NPC. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            ResetStates();
            _navMeshAgent.Warp(_spawnPoint);
        }

        /// <summary> Приоритизировать расписание. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void PrioritizeSchedule(DateTime currentTime)
        {
            bool isRaining = IsWeatherRainyNow();
            int hearts = GetCurrentRelationshipHearts();
            var suitableSchedule = _sortedScheduleParams
                .FirstOrDefault(param => param.AreConditionsSuitable(currentTime, hearts, isRaining));

            OnCurrentScheduleParamsChanged?.Invoke(suitableSchedule);
        }

        /// <summary> Получить текущее количество "сердец" (уровень отношений с NPC). </summary>
        /// <returns> Текущее количество "сердец" (уровень отношений с NPC). </returns>
        private int GetCurrentRelationshipHearts() => 0; // TODO: поменять на получение текущих отношений с данным нпс

        /// <summary> Сейчас дождливая погода? </summary>
        /// <returns> <c>true</c>, если дождливая, <c>false</c>, если ясная.</returns>
        private bool IsWeatherRainyNow() => false; //TODO: поменять на получение текущей погоды из спец. скрипта

        /// <summary> Обновить состояния. </summary>
        private void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(RoutineState));
        }

        /// <summary> Установить текущее состояние. </summary>
        /// <param name="stateType"> Состояние, которое нужно установить. </param>
        private void SetState(Type stateType)
        {
            if (!_typeToCharacterStates.TryGetValue(stateType, out var newState) || _currentState == newState) return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
    }
}