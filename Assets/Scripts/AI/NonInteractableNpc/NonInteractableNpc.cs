using System;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FSM;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Неинтерактивный NPC, который может перемещаться и выполнять последовательности действий. </summary>
    [RequireComponent(typeof(NpcPurchaseIndicator))]
    public class NonInteractableNpc : Npc
    {
        /// <summary> Контроллер передвижений для неинтерктивного НПС. </summary>
        private NonInteractableNpcMovementController _nonInteractableNpcMovementController;

        /// <summary> Стейт-контроллер для неинтерктивного НПС. </summary>
        private NonInteractableNpcStateController _nonInteractableNpcStateController;

        /// <summary> Менеджер локаций для управления переходами между локациями. </summary>
        [Inject] private LocationManager _locationManager;

        /// <summary> Сервис для транзакций. </summary>
        [Inject] private TransactionService _transactionService;

        /// <summary> Событие, вызываемое при достижении NPC точки спавна для уничтожения. </summary>
        public Action OnReachedDespawnPoint;

        /// <summary> Создает контроллер движения для неинтерактивного NPC. </summary>
        /// <returns> Экземпляр контроллера движения неинтерактивного NPC. </returns>
        protected override NpcMovementController CreateMovementController()
        {
            _nonInteractableNpcMovementController = new NonInteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
                transform,
                _animationController
            );
            return _nonInteractableNpcMovementController;
        }

        /// <summary> Создает контроллер состояний для неинтерактивного NPC. </summary>
        /// <returns> Экземпляр контроллера состояний неинтерактивного NPC. </returns>
        protected override NpcStateController CreateStateController()
        {
            _nonInteractableNpcStateController = new NonInteractableNpcStateController(
                _movementController as NonInteractableNpcMovementController,
                _locationManager,
                _animationController,
                _transactionService,
                transform,
                GetComponent<NpcPurchaseIndicator>()
            );
            return _nonInteractableNpcStateController;
        }

        /// <summary> Устанавливает цель для перемещения NPC с автоматическим запуском
        /// случайной последовательности по прибытии. </summary>
        /// <param name="npcDestination"> Целевая позиция для перемещения. </param>
        public void SetDestination(NpcDestinationPoint npcDestination)
        {
            _nonInteractableNpcMovementController.SetPoint(npcDestination);
            _nonInteractableNpcStateController.ForceSetState(StateName.Movement);
            _nonInteractableNpcMovementController.OnDestinationReached += () =>
            {
                _nonInteractableNpcStateController.ForceSetState(StateName.Idle);
                _nonInteractableNpcStateController.StartRandomSequence();
            };
        }

        /// <summary> Устанавливает точку исчезновения для неинтерактивного NPC. </summary>
        /// <param name="npcDestination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(NpcDestinationPoint npcDestination) =>
            (_stateController as NonInteractableNpcStateController)?.SetDespawnPoint(npcDestination);

#if UNITY_EDITOR

        protected void OnDrawGizmosSelected()
        {
            if (_stateController == null) return;

            Gizmos.color = Color.yellow;
            var labelPosition = transform.position + Vector3.up * 2.5f;

            Handles.Label(labelPosition, _stateController.CurrentStateName.ToString());
        }

#endif
    }
}