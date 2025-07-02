using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace FlavorfulStory.AI
{
    /// <summary> Навигатор NPC, отвечающий за перемещение персонажа
    /// по игровому миру с поддержкой телепортации между локациями. </summary>
    public class NpcNavigator
    {
        /// <summary> Агент NavMesh для навигации по сцене. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Transform NPC для управления позицией и поворотом. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Граф телепортов для перемещения между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Текущая локация, в которой находится NPC. </summary>
        private LocationName _currentLocation;

        /// <summary> Дистанция, на которой считается, что NPC достиг цели. </summary>
        private readonly float _arrivalDistance = 1.0f;

        /// <summary> Начальная позиция NPC при создании. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Начальная локация NPC при создании. </summary>
        private readonly LocationName _spawnLocation;

        /// <summary> Текущая целевая точка расписания. </summary>
        private SchedulePoint _currentTargetPoint;

        /// <summary> Флаг, указывающий, движется ли NPC в данный момент. </summary>
        private bool _isNotMoving;

        /// <summary> События, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Скорость агента. </summary>
        private Vector3 _agentSpeed;

        private CancellationTokenSource _warpCts;

        /// <summary> Инициализирует навигатор NPC с необходимыми компонентами. </summary>
        /// <param name="navMeshAgent"> Агент NavMesh для навигации. </param>
        /// <param name="warpGraph"> Граф телепортов для межлокационных перемещений. </param>
        /// <param name="transform"> Transform NPC. </param>
        public NpcNavigator(NavMeshAgent navMeshAgent, WarpGraph warpGraph, Transform transform)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _npcTransform = transform;

            _spawnPosition = transform.position;
            _spawnLocation = GetCurrentLocationName();
            _currentLocation = _spawnLocation;
        }

        /// <summary> Обновляет логику навигации каждый кадр, проверяя достижение цели. </summary>
        public void Update()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation == _currentTargetPoint.LocationName &&
                Vector3.Distance(_npcTransform.position, _currentTargetPoint.Position) <= _arrivalDistance &&
                !_isNotMoving)
            {
                _navMeshAgent.transform.rotation = Quaternion.Euler(_currentTargetPoint.Rotation);
                OnDestinationReached?.Invoke();
                _isNotMoving = true;
                StopAgent();
            }
        }

        /// <summary> Начинает движение к заданной точке расписания. </summary>
        /// <param name="point"> Целевая точка расписания для перемещения. </param>
        public void MoveTo(SchedulePoint point)
        {
            _currentTargetPoint = point;
            _isNotMoving = false;
            ResumeAgent();

            if (_currentLocation != point.LocationName)
                StartWarpTransition(point);
            else
                _navMeshAgent.SetDestination(point.Position);
        }

        /// <summary> Останавливает движение NPC с возможностью телепортации в начальную позицию. </summary>
        /// <param name="warpToSpawn"> Если true, телепортирует NPC в начальную позицию. </param>
        public void Stop(bool warpToSpawn = false)
        {
            _isNotMoving = true;
            StopAgent();

            _warpCts?.Cancel();
            _warpCts?.Dispose();
            _warpCts = null;

            _navMeshAgent.ResetPath();

            if (!warpToSpawn) return;

            _navMeshAgent.Warp(_spawnPosition);
            _currentLocation = _spawnLocation;
            _currentTargetPoint = null;
        }

        /// <summary> Переключатель передвижения агента. </summary>
        /// <param name="stopAgent"> Оставновить агента. </param>
        private void ToggleAgentMovement(bool stopAgent)
        {
            _navMeshAgent.isStopped = stopAgent;
            _agentSpeed = _navMeshAgent.velocity;
            _navMeshAgent.velocity = stopAgent ? Vector3.zero : _agentSpeed;
        }

        /// <summary> Остановить агента. </summary>
        private void StopAgent() => ToggleAgentMovement(true);

        /// <summary> Запустить агента. </summary>
        private void ResumeAgent() => ToggleAgentMovement(false);

        /// <summary> Начинает процесс телепортации к целевой точке в другой локации. </summary>
        /// <param name="destination"> Целевая точка назначения. </param>
        private void StartWarpTransition(SchedulePoint destination)
        {
            var startWarp = FindClosestWarp(_npcTransform.position, _currentLocation);
            var endWarp = FindClosestWarp(destination.Position, destination.LocationName);

            if (!startWarp || !endWarp)
            {
                Debug.LogError("Warp points not found.");
                return;
            }

            var path = _warpGraph.FindShortestPath(startWarp, endWarp);
            if (path == null)
            {
                Debug.LogError("Warp path not found.");
                return;
            }

            // _currentWarpCoroutine = _coroutineRunner.StartCoroutine(WarpRoutine(path, destination));
            _warpCts?.Cancel();
            _warpCts?.Dispose();
            _warpCts = new CancellationTokenSource();

            WarpRoutine(path, destination, _warpCts.Token).Forget();
        }

        private async UniTask WarpRoutine(List<WarpPortal> path, SchedulePoint destination, CancellationToken token)
        {
            foreach (var warp in path)
            {
                token.ThrowIfCancellationRequested();

                if (warp.ParentLocationName != _currentLocation)
                {
                    _navMeshAgent.Warp(warp.transform.position);
                    _currentLocation = warp.ParentLocationName;
                }
                else
                {
                    _navMeshAgent.SetDestination(warp.transform.position);
                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _arrivalDistance)
                    {
                        token.ThrowIfCancellationRequested();
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                    }
                }
            }

            _navMeshAgent.SetDestination(destination.Position);
            while (_navMeshAgent.remainingDistance > _arrivalDistance)
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary> Находит ближайший телепорт к заданной позиции в указанной локации. </summary>
        /// <param name="pos"> Позиция для поиска. </param>
        /// <param name="loc"> Локация для поиска. </param>
        /// <returns> Ближайший телепорт или null, если не найден. </returns>
        private WarpPortal FindClosestWarp(Vector3 pos, LocationName loc) =>
            _warpGraph.FindClosestWarp(pos, loc)?.SourceWarp;

        /// <summary> Определяет текущую локацию NPC на основе его позиции. </summary>
        /// <returns> Название текущей локации. </returns>
        // TODO: Удалить. Для игрока используется ровно такая же логика, надо оставить одну.
        private LocationName GetCurrentLocationName()
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location.LocationName;
            return LocationName.RockyIsland;
        }

        /// <summary> Обрабатывает изменение точки расписания во время движения. </summary>
        /// <param name="point"> Новая точка расписания. </param>
        public void OnSchedulePointChanged(SchedulePoint point)
        {
            if (_isNotMoving) return;

            Stop();
            MoveTo(point);
        }
    }
}