using System;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Неинтерактивный NPC, который может перемещаться и выполнять последовательности действий. </summary>
    [RequireComponent(typeof(NpcSpriteIndicator))]
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
        protected override StateController CreateStateController()
        {
            _nonInteractableNpcStateController = new NonInteractableNpcStateController(
                _movementController as NonInteractableNpcMovementController,
                _locationManager,
                _animationController,
                _transactionService,
                transform,
                GetComponent<NpcSpriteIndicator>()
            );
            return _nonInteractableNpcStateController;
        }

        /// <summary> Устанавливает цель для перемещения NPC с автоматическим запуском случайной последовательности по прибытии. </summary>
        /// <param name="destination"> Целевая позиция для перемещения. </param>
        public void SetDestination(Vector3 destination)
        {
            _nonInteractableNpcMovementController.SetPoint(destination);
            _nonInteractableNpcStateController.ForceSetState(StateName.Movement);
            _nonInteractableNpcMovementController.OnDestinationReached += () =>
            {
                _nonInteractableNpcStateController.ForceSetState(StateName.Idle);
                _nonInteractableNpcStateController.StartRandomSequence();
            };
        }

        /// <summary> Устанавливает точку исчезновения для неинтерактивного NPC. </summary>
        /// <param name="destination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(Vector3 destination) =>
            (_stateController as NonInteractableNpcStateController)?.SetDespawnPoint(destination);

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (_stateController != null)
            {
                Gizmos.color = Color.yellow;
                var labelPosition = transform.position + Vector3.up * 2.5f;

                Handles.Label(labelPosition, _stateController.CurrentStateName.ToString());
            }
        }
#endif
    }
}