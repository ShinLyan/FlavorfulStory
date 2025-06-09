using System;
using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace FlavorfulStory.AI
{
    public class NpcNavigator
    {
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _npcTransform;
        private readonly WarpGraph _warpGraph;
        private readonly MonoBehaviour _coroutineRunner;

        private Coroutine _currentWarpCoroutine;
        private LocationName _currentLocation;
        private readonly float _arrivalDistance = 1.0f;

        private readonly Vector3 _spawnPosition;
        private readonly LocationName _spawnLocation;
        private SchedulePoint _currentTargetPoint;

        public Action OnDestinationReached;

        public NpcNavigator(NavMeshAgent navMeshAgent, WarpGraph warpGraph, Transform transform,
            MonoBehaviour coroutineRunner)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _npcTransform = transform;
            _coroutineRunner = coroutineRunner;

            _spawnPosition = transform.position;
            _spawnLocation = GetCurrentLocationName();
            _currentLocation = _spawnLocation;
        }

        public void Update()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation == _currentTargetPoint.LocationName &&
                Vector3.Distance(_npcTransform.position, _currentTargetPoint.Position) <= _arrivalDistance)
            {
                _navMeshAgent.transform.rotation = Quaternion.Euler(_currentTargetPoint.Rotation);
                OnDestinationReached?.Invoke();
            }
        }

        public void MoveTo(SchedulePoint point)
        {
            _currentTargetPoint = point;

            if (_currentLocation != point.LocationName)
                StartWarpTransition(point);
            else
                _navMeshAgent.SetDestination(point.Position);
        }

        public void Stop(bool warpToSpawn = false)
        {
            if (_currentWarpCoroutine != null) _coroutineRunner.StopCoroutine(_currentWarpCoroutine);

            _navMeshAgent.ResetPath();

            if (warpToSpawn)
            {
                _navMeshAgent.Warp(_spawnPosition);
                _currentLocation = _spawnLocation;
                _currentTargetPoint = null;
            }
        }


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

            _currentWarpCoroutine = _coroutineRunner.StartCoroutine(WarpRoutine(path, destination));
        }

        private IEnumerator WarpRoutine(List<WarpPortal> path, SchedulePoint destination)
        {
            foreach (var warp in path)
                if (warp.ParentLocationName != _currentLocation)
                {
                    _navMeshAgent.Warp(warp.transform.position);
                    _currentLocation = warp.ParentLocationName;
                }
                else
                {
                    _navMeshAgent.SetDestination(warp.transform.position);
                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _arrivalDistance)
                        yield return null;
                }

            _navMeshAgent.SetDestination(destination.Position);
            while (_navMeshAgent.remainingDistance > _arrivalDistance) yield return null;
        }

        private WarpPortal FindClosestWarp(Vector3 pos, LocationName loc) =>
            _warpGraph.FindClosestWarp(pos, loc)?.SourceWarp;

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