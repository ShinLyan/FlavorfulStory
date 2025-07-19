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

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Навигатор для управления перемещением NPC с поддержкой варп-переходов между локациями. </summary>
    public class NpcNavigator : INpcNavigator
    {
        #region Fields

        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        protected readonly NavMeshAgent _navMeshAgent;

        /// <summary> Текущая локация NPC. </summary>
        protected LocationName _currentLocation;

        /// <summary> Текущая целевая точка расписания. </summary>
        protected SchedulePoint _currentTargetPoint;

        /// <summary> Флаг, указывающий что NPC не двигается. </summary>
        protected bool _isNotMoving;

        /// <summary> Transform объекта NPC. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Граф варп-точек для навигации между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Дистанция, на которой считается что цель достигнута. </summary>
        private const float ArrivalDistance = 0.05f;

        /// <summary> Начальная позиция NPC при создании. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Локация спавна NPC. </summary>
        private readonly LocationName _spawnLocation;

        /// <summary> Скорость агента. </summary>
        private Vector3 _agentSpeed;
        
        /// <summary> Токен отмены для телепортации между локациями. </summary>
        private CancellationTokenSource _warpCts;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        #endregion

        /// <summary> Инициализирует новый экземпляр навигатора NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="warpGraph"> Граф варп-точек. </param>
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

        #region INpcNavigator

        /// <summary> Обновляет состояние навигации каждый кадр. </summary>
        /// <remarks> Проверяет достижение цели и обновляет состояние NPC. </remarks>
        public virtual void Update()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation == _currentTargetPoint.LocationName && !_isNotMoving)
            {
                Vector3 offset = _npcTransform.position - _currentTargetPoint.Position;
                offset.y = 0;
                float sqrDistance = offset.sqrMagnitude;

                if (sqrDistance <= ArrivalDistance)
                {
                    _navMeshAgent.transform.rotation = Quaternion.Euler(_currentTargetPoint.Rotation);
                    OnDestinationReached?.Invoke();
                    Stop();
                }
            }
        }

        /// <summary> Останавливает навигацию NPC. </summary>
        /// <param name="warpToSpawn"> Если true, телепортирует NPC на точку спавна. </param>
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

        #endregion

        /// <summary> Переключает движение агента. </summary>
        /// <param name="stopAgent"> Если true, останавливает агента, иначе возобновляет движение. </param>
        private void ToggleAgentMovement(bool stopAgent)
        {
            _navMeshAgent.isStopped = stopAgent;
            _agentSpeed = _navMeshAgent.velocity;
            _navMeshAgent.velocity = stopAgent ? Vector3.zero : _agentSpeed;
        }

        /// <summary> Останавливает агента. </summary>
        private void StopAgent() => ToggleAgentMovement(true);

        /// <summary> Возобновляет движение агента. </summary>
        protected void ResumeAgent() => ToggleAgentMovement(false);

        /// <summary> Начинает варп-переход к указанной точке назначения. </summary>
        /// <param name="destination"> Целевая точка для варп-перехода. </param>
        protected void StartWarpTransition(SchedulePoint destination)
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

            _warpCts?.Cancel();
            _warpCts?.Dispose();
            _warpCts = new CancellationTokenSource();

            WarpRoutine(path, destination, _warpCts.Token).Forget();
        }

        /// <summary> Асинхронная рутина для выполнения варп-перехода. </summary>
        /// <param name="path"> Путь варп-порталов. </param>
        /// <param name="destination"> Целевая точка. </param>
        /// <param name="token"> Токен отмены. </param>
        /// <returns> Задача выполнения варп-перехода. </returns>
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
                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > ArrivalDistance)
                    {
                        token.ThrowIfCancellationRequested();
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                    }
                }
            }

            _navMeshAgent.SetDestination(destination.Position);
            while (_navMeshAgent.remainingDistance > ArrivalDistance)
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary> Находит ближайший варп-портал к указанной позиции в локации. </summary>
        /// <param name="pos"> Позиция для поиска. </param>
        /// <param name="loc"> Локация для поиска. </param>
        /// <returns> Ближайший варп-портал или null если не найден. </returns>
        private WarpPortal FindClosestWarp(Vector3 pos, LocationName loc) =>
            _warpGraph.FindClosestWarp(pos, loc)?.SourceWarp;

        /// <summary> Определяет текущую локацию NPC по его позиции. </summary>
        /// <returns> Название текущей локации. </returns>
        //TODO: Удалить. Для игрока используется ровно такая же логика, надо оставить одну. </remarks>
        private LocationName GetCurrentLocationName()
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location.LocationName;

            return LocationName.RockyIsland;
        }
    }
}