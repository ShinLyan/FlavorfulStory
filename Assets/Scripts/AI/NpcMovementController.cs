using System;
using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Object = UnityEngine.Object;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер перемещения NPC, отвечающий за навигацию и перемещение между точками </summary>
    public class NpcMovementController : IScheduleDependable
    {
        #region Fields

        private readonly NavMeshAgent _navMeshAgent;
        private readonly WarpGraph _warpGraph;
        private readonly Animator _animator;
        private readonly Transform _npcTransform;
        private readonly MonoBehaviour _coroutineRunner;

        private readonly Vector3 _spawnPosition;
        private readonly LocationName _spawnLocation;
        private readonly int _speedParameterHash = Animator.StringToHash("Speed");

        private SchedulePoint _currentTargetPoint;
        private LocationName _currentLocation;
        private Coroutine _currentPathCoroutine;
        private Stack<SchedulePoint> _currentPath;

        private const float DistanceToReachPoint = 1.0f;

        public Action OnDestinationReached;

        #endregion

        /// <summary> Инициализация контроллера перемещения </summary>
        public NpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Animator animator,
            Transform npcTransform,
            MonoBehaviour coroutineRunner)
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

        /// <summary> Обновить логику перемещения каждый кадр </summary>
        public void UpdateMovement()
        {
            if (_currentTargetPoint == null) return;

            float speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude) * 0.5f;
            PlayMoveAnimation(speed);

            if (IsDestinationReached()) OnDestinationReached?.Invoke();
        }

        /// <summary> Проверить достигнута ли текущая точка </summary>
        private bool IsDestinationReached()
        {
            if (_currentTargetPoint == null) return false;

            bool isInTargetLocation = _currentLocation == _currentTargetPoint.LocationName;
            bool isCloseEnough =
                Vector3.Distance(_currentTargetPoint.Position, _npcTransform.position) <= DistanceToReachPoint;

            return isInTargetLocation && isCloseEnough;
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
        

        /// <summary> Установить текущий путь перемещения </summary>
        private void SetPath(Stack<SchedulePoint> path) => _currentPath = path;

        /// <summary> Начать движение к следующей точке в расписании </summary>
        public void MoveToNextPoint()
        {
            if (_currentPath == null || _currentPath.Count == 0) return;
            MoveToCurrentTarget();
        }

        /// <summary> Остановить движение без телепортации </summary>
        public void StopMovementWithoutWarp()
        {
            StopCurrentCoroutine();
            PlayMoveAnimation(0f, 0f);
            _navMeshAgent.ResetPath();
            if (_currentTargetPoint != null)
                _navMeshAgent.transform.rotation = Quaternion.Euler(_currentTargetPoint.Rotation);
        }

        /// <summary> Остановить движение с возвратом на спавн </summary>
        public void StopMovementAndWarp()
        {
            StopMovementWithoutWarp();
            _navMeshAgent.Warp(_spawnPosition);
            _currentLocation = _spawnLocation;
            _currentTargetPoint = null;
        }

        /// <summary> Останавливает текущую активную корутину. </summary>
        private void StopCurrentCoroutine()
        {
            if (_currentPathCoroutine == null) return;

            _coroutineRunner.StopCoroutine(_currentPathCoroutine);
            _currentPathCoroutine = null;
        }

        private void UpdateCurrentSchedulePoint(DateTime currentTime)
        {
            if (_currentPath == null || _currentPath.Count <= 0) return;

            var nextPoint = _currentPath.Peek();
            if ((int)currentTime.Hour != nextPoint.Hour || (int)currentTime.Minute != nextPoint.Minutes) return;

            StopCurrentCoroutine();
            _currentTargetPoint = _currentPath.Pop();
            MoveToCurrentTarget();
        }

        private void MoveToCurrentTarget()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation != _currentTargetPoint.LocationName)
                StartWarpTransition(_currentTargetPoint);
            else
                _navMeshAgent.SetDestination(_currentTargetPoint.Position);
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

        /// <summary> Воспроизводит анимацию движения. </summary>
        /// <param name="speed"> Скорость движения. </param>
        /// <param name="dampTime"> Время сглаживания перехода анимации. </param>
        private void PlayMoveAnimation(float speed, float dampTime = 0.2f) =>
            _animator.SetFloat(_speedParameterHash, speed, dampTime, Time.deltaTime);
        
        #region IScheduleDependable
        
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams)
        {
            SetPath(scheduleParams?.GetSortedSchedulePointsStack());
        }
        
        #endregion
    }
}