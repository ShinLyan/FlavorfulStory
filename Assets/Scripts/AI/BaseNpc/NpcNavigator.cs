using System;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Навигатор для управления перемещением NPC с поддержкой варп-переходов между локациями. </summary>
    public class NpcNavigator
    {
        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Текущая целевая точка расписания. </summary>
        private NpcDestinationPoint _currentTargetPoint;

        /// <summary> NPC не двигается? </summary>
        protected bool _isNotMoving;

        /// <summary> Transform объекта NPC. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Дистанция, на которой считается что цель достигнута. </summary>
        private const float ArrivalDistance = 0.1f;

        /// <summary> Начальная позиция NPC при создании. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Локация спавна NPC. </summary>
        private readonly LocationName _spawnLocation;

        /// <summary> Скорость агента. </summary>
        private Vector3 _agentSpeed;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Инициализирует новый экземпляр навигатора NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        public NpcNavigator(NavMeshAgent navMeshAgent, Transform transform)
        {
            _navMeshAgent = navMeshAgent;
            _npcTransform = transform;
            _spawnPosition = transform.position;

            _navMeshAgent.autoTraverseOffMeshLink = false;
        }

        /// <summary> Обновляет состояние навигации каждый кадр. </summary>
        /// <remarks> Проверяет достижение цели и обновляет состояние NPC. </remarks>
        public void Update()
        {
            if (_navMeshAgent.isOnOffMeshLink)
            {
                var link = _navMeshAgent.currentOffMeshLinkData;
                _navMeshAgent.Warp(link.endPos);
                _navMeshAgent.CompleteOffMeshLink();
                MoveTo(_currentTargetPoint);
            }

            if (_isNotMoving) return;

            var offset = _npcTransform.position - _currentTargetPoint.Position;
            offset.y = 0;
            float sqrDistance = offset.sqrMagnitude;

            if (sqrDistance <= ArrivalDistance)
            {
                _navMeshAgent.transform.rotation = _currentTargetPoint.Rotation;
                OnDestinationReached?.Invoke();
                Stop();
            }
        }

        /// <summary> Останавливает навигацию NPC. </summary>
        /// <param name="warpToSpawn"> Если true, телепортирует NPC на точку спавна. </param>
        public void Stop(bool warpToSpawn = false)
        {
            _isNotMoving = true;
            StopAgent();

            if (!warpToSpawn) return;

            _navMeshAgent.Warp(_spawnPosition);
            _currentTargetPoint = new NpcDestinationPoint(Vector3.zero, Quaternion.identity);
        }

        public void MoveTo(NpcDestinationPoint point)
        {
            _currentTargetPoint = point;
            _isNotMoving = false;
            ResumeAgent();

            _navMeshAgent.SetDestination(point.Position);
        }

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
        private void ResumeAgent() => ToggleAgentMovement(false);
    }
}