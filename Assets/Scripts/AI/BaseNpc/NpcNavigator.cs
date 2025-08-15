using System;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Навигатор для управления перемещением NPC с поддержкой варп-переходов между локациями. </summary>
    public sealed class NpcNavigator
    {
        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        private readonly NavMeshAgent _agent;

        /// <summary> Transform объекта NPC. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Начальная позиция NPC при создании. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Текущая целевая точка расписания. </summary>
        private NpcDestinationPoint _currentTargetPoint;

        /// <summary> Дистанция, на которой считается что цель достигнута. </summary>
        private const float ArrivalDistance = 0.1f;

        /// <summary> NPC двигается? </summary>
        public bool IsMoving { get; private set; }

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Инициализирует компонент навигации для NPC. </summary>
        /// <param name="agent"> NavMeshAgent, управляющий движением. </param>
        /// <param name="transform"> Transform объекта NPC. </param>
        public NpcNavigator(NavMeshAgent agent, Transform transform)
        {
            _agent = agent;
            _npcTransform = transform;
            _spawnPosition = transform.position;

            _agent.autoTraverseOffMeshLink = false;
        }

        /// <summary> Обновляет состояние навигации каждый кадр. </summary>
        /// <remarks> Проверяет достижение цели и обновляет состояние NPC. </remarks>
        public void Update()
        {
            HandleOffMeshLink();
            if (!IsMoving) return;
            HandleArrival();
        }

        /// <summary> Обрабатывает прохождение OffMeshLink. </summary>
        private void HandleOffMeshLink()
        {
            if (!_agent.isOnOffMeshLink) return;

            var link = _agent.currentOffMeshLinkData;
            _agent.Warp(link.endPos);
            _agent.CompleteOffMeshLink();
            MoveTo(_currentTargetPoint);
        }

        /// <summary> Запускает перемещение NPC к заданной точке. </summary>
        /// <param name="point"> Целевая точка назначения. </param>
        public void MoveTo(NpcDestinationPoint point)
        {
            _currentTargetPoint = point;
            IsMoving = true;
            SetAgentStopped(false);
            _agent.SetDestination(point.Position);
        }

        /// <summary> Проверяет достижение цели и завершает движение NPC. </summary>
        private void HandleArrival()
        {
            var offset = _npcTransform.position - _currentTargetPoint.Position;
            offset.y = 0;
            if (!(offset.sqrMagnitude <= ArrivalDistance)) return;

            _agent.transform.rotation = _currentTargetPoint.Rotation;
            OnDestinationReached?.Invoke();
            Stop();
        }

        /// <summary> Останавливает навигацию NPC. </summary>
        /// <param name="warpToSpawn"> Если true, телепортирует NPC на точку спавна. </param>
        public void Stop(bool warpToSpawn = false)
        {
            IsMoving = false;
            SetAgentStopped(true);

            if (!warpToSpawn) return;

            _agent.Warp(_spawnPosition);
            _currentTargetPoint = new NpcDestinationPoint();
        }

        /// <summary> Включает или отключает движение агента. </summary>
        /// <param name="isStopped"> True — остановить агента; False — продолжить движение. </param>
        private void SetAgentStopped(bool isStopped)
        {
            _agent.isStopped = isStopped;
            _agent.velocity = isStopped ? Vector3.zero : _agent.velocity;
        }
    }
}