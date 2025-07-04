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
    public class NpcNavigator : INpcNavigator
    {
        protected readonly NavMeshAgent _navMeshAgent;
        protected readonly Transform _npcTransform;
        protected readonly WarpGraph _warpGraph;

        protected LocationName _currentLocation;
        protected readonly float _arrivalDistance = 1.0f;
        protected readonly Vector3 _spawnPosition;
        protected readonly LocationName _spawnLocation;

        protected SchedulePoint _currentTargetPoint;
        protected bool _isNotMoving;
        protected Vector3 _agentSpeed;

        protected CancellationTokenSource _warpCts;

        public Action OnDestinationReached;

        public NpcNavigator(NavMeshAgent navMeshAgent, WarpGraph warpGraph, Transform transform)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _npcTransform = transform;

            _spawnPosition = transform.position;
            _spawnLocation = GetCurrentLocationName();
            _currentLocation = _spawnLocation;
        }

        public virtual void Update()
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

        private void ToggleAgentMovement(bool stopAgent)
        {
            _navMeshAgent.isStopped = stopAgent;
            _agentSpeed = _navMeshAgent.velocity;
            _navMeshAgent.velocity = stopAgent ? Vector3.zero : _agentSpeed;
        }

        protected void StopAgent() => ToggleAgentMovement(true);
        protected void ResumeAgent() => ToggleAgentMovement(false);

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

        protected async UniTask WarpRoutine(List<WarpPortal> path, SchedulePoint destination, CancellationToken token)
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

        protected WarpPortal FindClosestWarp(Vector3 pos, LocationName loc) =>
            _warpGraph.FindClosestWarp(pos, loc)?.SourceWarp;

        // TODO: Удалить. Для игрока используется ровно такая же логика, надо оставить одну.
        protected LocationName GetCurrentLocationName()
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location.LocationName;

            return LocationName.RockyIsland;
        }
    }
}