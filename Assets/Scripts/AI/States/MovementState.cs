using System.Collections;
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
    /// <summary> Состояние движения NPC, в котором персонаж перемещается к заданной точке. </summary>
    public class MovementState : CharacterState
    {
        #region Variables

        /// <summary> Минимальное расстояние до точки, при котором считается, что NPC достиг её. </summary>
        private const float DistanceToReachPoint = 0.3f;

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Граф варпов, используемый для поиска пути между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private readonly Animator _animator;

        /// <summary> Текущая корутина, выполняющая перемещение NPC по пути. </summary>
        private Coroutine _currentPathCoroutine;

        /// <summary> Текущая точка маршрута, к которой движется NPC. </summary>
        private SchedulePoint _currentPoint;

        private LocationName _currentLocation;
        private readonly LocationName _spawnLocation;

        private ScheduleParams _currentScheduleParams;

        /// <summary> Текущая скорость NPC, используемая для анимации. </summary>
        private float _speed;

        private readonly Transform _npcTransform;

        private readonly MonoBehaviour _coroutineRunner;

        /// <summary> Хэшированное значение параметра "скорость" для анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        #endregion

        /// <summary> Инициализирует новое состояние движения. </summary>
        /// <param name="navMeshAgent"> Компонент для навигации по NavMesh. </param>
        /// <param name="warpGraph"> Граф варпов. </param>
        /// <param name="spawnLocation"> </param>
        public MovementState(NavMeshAgent navMeshAgent, WarpGraph warpGraph, LocationName spawnLocation,
            Animator animator, Transform npcTransform, MonoBehaviour coroutineRunner)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _spawnLocation = spawnLocation;
            _currentLocation = spawnLocation;
            _animator = animator;
            _currentScheduleParams = null;
            _npcTransform = npcTransform;
            _coroutineRunner = coroutineRunner;
        }

        /// <summary> Вызывается при входе в состояние движения. </summary>
        public override void Enter()
        {
            WorldTime.OnTimeUpdated += FindDestinationPoint;
        }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit()
        {
            Reset();
            WorldTime.OnTimeUpdated -= FindDestinationPoint;
        }

        public override void Reset()
        {
            StopCoroutine();
            PlayMoveAnimation(0f, 0f);
            _currentPoint = null;
            _navMeshAgent.ResetPath();
            _currentLocation = _spawnLocation;
        }

        /// <summary> Обновляет логику состояния движения каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            _speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude) * 0.5f;
            PlayMoveAnimation(_speed);

            if (_currentPoint == null) return;
            SwitchStateIfPointReached();
        }

        public void SetCurrentScheduleParams(ScheduleParams scheduleParams) => _currentScheduleParams = scheduleParams;

        /// <summary> Проверяет, достиг ли NPC текущей точки, и переключает состояние, если это так. </summary>
        private void SwitchStateIfPointReached()
        {
            bool isInTargetLocation = _currentLocation == _currentPoint.LocationName;
            bool isCloseEnough = _navMeshAgent.remainingDistance <= DistanceToReachPoint;

            if (isInTargetLocation && isCloseEnough) RequestStateChange(typeof(RoutineState));
        }

        /// <summary> Находит ближайшую точку маршрута на основе текущего времени
        /// и задаёт её как цель для NPC. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void FindDestinationPoint(DateTime currentTime)
        {
            var closestPoint = _currentScheduleParams?.GetClosestSchedulePointInPath(currentTime);

            if (closestPoint == null)
            {
                Debug.LogError("Ближайшая точка отсутствует!");
                return;
            }

            if (_currentPoint == closestPoint) return;

            StopCoroutine();

            _currentPoint = closestPoint;

            if (_currentLocation != closestPoint.LocationName)
                HandleWarpTransition(closestPoint);
            else
                _navMeshAgent.SetDestination(closestPoint.Position);
        }

        /// <summary> Обрабатывает переход NPC через варпы между локациями. </summary>
        /// <param name="destination"> Целевая точка расписания. </param>
        private void HandleWarpTransition(SchedulePoint destination)
        {
            var currentWarp = FindClosestWarpInScene(_npcTransform.position, _currentLocation);
            var targetWarp = FindClosestWarpInScene(destination.Position, destination.LocationName);

            if (!currentWarp || !targetWarp)
            {
                Debug.LogError("Не удалось найти начальный или конечный варп!");
                return;
            }

            var path = _warpGraph.FindShortestPath(currentWarp, targetWarp);

            if (path == null)
            {
                Debug.LogError("Путь не найден!");
                return;
            }

            _currentPathCoroutine = _coroutineRunner.StartCoroutine(TraverseWarpPath(path, destination));
        }

        /// <summary> Перемещает NPC по пути через варпы. </summary>
        /// <param name="path"> Список варпов, через которые нужно пройти. </param>
        /// <param name="destination"> Целевая точка расписания. </param>
        private IEnumerator TraverseWarpPath(List<WarpPortal> path, SchedulePoint destination)
        {
            foreach (var currentWarp in path)
                if (currentWarp.ParentLocationName != _currentLocation)
                {
                    _navMeshAgent.Warp(currentWarp.transform.position);
                    _currentLocation = currentWarp.ParentLocationName;
                }
                else
                {
                    _navMeshAgent.SetDestination(currentWarp.transform.position);

                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > DistanceToReachPoint)
                        yield return null;
                }

            // когда прошел через все варпы и оказался на конечной сцене
            _navMeshAgent.SetDestination(destination.Position);

            while (_navMeshAgent.remainingDistance > DistanceToReachPoint)
                yield return null;
        }

        /// <summary> Находит ближайший варп в указанной локации. </summary>
        /// <param name="position"> Позиция для поиска ближайшего варпа. </param>
        /// <param name="location"> Локация, в которой искать варп. </param>
        /// <returns> Ближайший варп или null, если варп не найден. </returns>
        private WarpPortal FindClosestWarpInScene(Vector3 position, LocationName location)
        {
            var closestWarp = _warpGraph.FindClosestWarp(position, location);
            return closestWarp?.SourceWarp;
        }

        /// <summary> Остановить корутину. </summary>
        private void StopCoroutine()
        {
            if (_currentPathCoroutine == null) return;

            _coroutineRunner.StopCoroutine(_currentPathCoroutine);
            _currentPathCoroutine = null;
        }

        /// <summary> Воспроизведение анимации движения. </summary>
        /// <param name="speed"> Скорость движения. </param>
        /// <param name="dampTime"> Время сглаживания перехода анимации. </param>
        private void PlayMoveAnimation(float speed, float dampTime = 0.2f)
        {
            _animator.SetFloat(_speedParameterHash, speed, dampTime, Time.deltaTime);
        }
    }
}