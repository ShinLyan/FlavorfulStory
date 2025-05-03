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
    public class MovementState : CharacterState, IScheduleDependable
    {
        #region Fields

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Граф варпов, используемый для поиска пути между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private readonly Animator _animator;

        /// <summary> Компонент Transform NPC. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Компонент для запуска корутин. </summary>
        private readonly MonoBehaviour _coroutineRunner;

        /// <summary> Позиция спавна NPC. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Локация спавна NPC. </summary>
        private readonly LocationName _spawnLocation;

        /// <summary> Хэш параметра скорости анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        /// <summary> Текущее активное расписание NPC. </summary>
        private ScheduleParams _currentScheduleParams;

        /// <summary> Стек маршрута движения по расписанию. </summary>
        private Stack<SchedulePoint> _currentPath;

        /// <summary> Текущая целевая точка маршрута. </summary>
        private SchedulePoint _currentTargetPoint;

        /// <summary> Текущая локация, в которой находится NPC. </summary>
        private LocationName _currentLocation;

        /// <summary> Текущая корутина движения по пути. </summary>
        private Coroutine _currentPathCoroutine;

        /// <summary> Минимальное расстояние до целевой точки для её достижения. </summary>
        private const float DistanceToReachPoint = 1.0f;

        #endregion

        /// <summary> Инициализация нового состояния движения. </summary>
        /// <param name="navMeshAgent"> Компонент навигации по NavMesh. </param>
        /// <param name="warpGraph"> Граф варпов. </param>
        /// <param name="animator"> Аниматор персонажа. </param>
        /// <param name="npcTransform"> Transform NPC. </param>
        /// <param name="coroutineRunner"> Компонент для запуска корутин. </param>
        public MovementState(NavMeshAgent navMeshAgent, WarpGraph warpGraph,
            Animator animator, Transform npcTransform, MonoBehaviour coroutineRunner)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _animator = animator;
            _npcTransform = npcTransform;
            _coroutineRunner = coroutineRunner;

            _spawnPosition = npcTransform.position;
            _spawnLocation = GetCurrentLocationName();
            _currentLocation = _spawnLocation;

            WorldTime.OnTimePaused += StopMovementWithoutWarp;
            WorldTime.OnTimeUnpaused += MoveToCurrentTarget;
            WorldTime.OnTimeUpdated += UpdateCurrentSchedulePoint;
        }

        /// <summary> Получить название локации, в которой находится NPC. </summary>
        /// <returns> Название текущей локации. </returns>
        private LocationName GetCurrentLocationName()
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location.LocationName;
            return LocationName.RockyIsland;
        }

        /// <summary> Вызывается при входе в состояние движения. </summary>
        public override void Enter() { }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit() => StopMovementWithoutWarp();

        /// <summary> Обновляет состояние движения NPC каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            float speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude) * 0.5f;
            PlayMoveAnimation(speed);

            if (_currentTargetPoint == null) return;

            SwitchStateIfPointReached();
        }

        /// <summary> Сброс состояния движения NPC. </summary>
        public override void Reset() => StopMovementAndWarp();

        /// <summary> Останавливает движение NPC без телепортации. </summary>
        private void StopMovementWithoutWarp()
        {
            StopCurrentCoroutine();
            PlayMoveAnimation(0f, 0f);
            _navMeshAgent.ResetPath();
            if (_currentTargetPoint != null)
                _navMeshAgent.transform.rotation = Quaternion.Euler(_currentTargetPoint.Rotation);
        }

        /// <summary> Останавливает движение NPC с возвратом на точку спавна. </summary>
        private void StopMovementAndWarp()
        {
            StopMovementWithoutWarp();
            _navMeshAgent.Warp(_spawnPosition);
            _currentLocation = _spawnLocation;
            _currentTargetPoint = null;
        }

        /// <summary> Обновляет текущую целевую точку маршрута на основе времени. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void UpdateCurrentSchedulePoint(DateTime currentTime)
        {
            if (_currentPath.Count <= 0) return;

            var nextPoint = _currentPath.Peek();
            if ((int)currentTime.Hour != nextPoint.Hour || (int)currentTime.Minute != nextPoint.Minutes) return;

            StopCurrentCoroutine();
            _currentTargetPoint = _currentPath.Pop();
            MoveToCurrentTarget();
        }

        /// <summary> Начинает движение к текущей целевой точке. </summary>
        private void MoveToCurrentTarget()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation != _currentTargetPoint.LocationName)
                StartWarpTransition(_currentTargetPoint);
            else
                _navMeshAgent.SetDestination(_currentTargetPoint.Position);
        }

        /// <summary> Проверяет достижение целевой точки и переключает состояние. </summary>
        private void SwitchStateIfPointReached()
        {
            bool isInTargetLocation = _currentLocation == _currentTargetPoint.LocationName;
            bool isCloseEnough =
                Vector3.Distance(_currentTargetPoint.Position, _npcTransform.position) <= DistanceToReachPoint;

            if (isInTargetLocation && isCloseEnough) RequestStateChange(typeof(RoutineState));
        }

        /// <summary> Начинает переход через варпы к целевой локации. </summary>
        /// <param name="destination"> Целевая точка назначения. </param>
        private void StartWarpTransition(SchedulePoint destination)
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

        /// <summary> Перемещает NPC по маршруту через варпы. </summary>
        /// <param name="path"> Путь через варпы. </param>
        /// <param name="destination"> Целевая точка назначения. </param>
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

            while (_navMeshAgent.remainingDistance > DistanceToReachPoint) yield return null;
        }

        /// <summary> Находит ближайший варп в указанной локации. </summary>
        /// <param name="position"> Позиция поиска. </param>
        /// <param name="location"> Название локации. </param>
        /// <returns> Ближайший варп или null. </returns>
        private WarpPortal FindClosestWarpInScene(Vector3 position, LocationName location)
        {
            var closestWarp = _warpGraph.FindClosestWarp(position, location);
            return closestWarp?.SourceWarp;
        }

        /// <summary> Останавливает текущую активную корутину. </summary>
        private void StopCurrentCoroutine()
        {
            if (_currentPathCoroutine == null) return;

            _coroutineRunner.StopCoroutine(_currentPathCoroutine);
            _currentPathCoroutine = null;
        }

        /// <summary> Устанавливает новое расписание движения. </summary>
        /// <param name="scheduleParams"> Параметры расписания. </param>
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams)
        {
            _currentScheduleParams = scheduleParams;
            if (_currentScheduleParams == null) return;

            _currentPath = _currentScheduleParams.GetSortedSchedulePointsStack();
        }

        /// <summary> Воспроизводит анимацию движения. </summary>
        /// <param name="speed"> Скорость движения. </param>
        /// <param name="dampTime"> Время сглаживания перехода анимации. </param>
        private void PlayMoveAnimation(float speed, float dampTime = 0.2f) =>
            _animator.SetFloat(_speedParameterHash, speed, dampTime, Time.deltaTime);
    }
}