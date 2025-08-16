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
    public sealed class NonInteractableNpc : Npc
    {
        /// <summary> Контроллер передвижений для неинтерактивного НПС. </summary>
        private NonInteractableNpcMovementController _movementController;

        /// <summary> Стейт-контроллер для неинтерктивного НПС. </summary>
        private NonInteractableNpcStateController _stateController;

        /// <summary> Менеджер локаций для управления переходами между локациями. </summary>
        private LocationManager _locationManager;

        /// <summary> Сервис для транзакций. </summary>
        private TransactionService _transactionService;

        /// <summary> Возвращает контроллер движения NPC. </summary>
        protected override NpcMovementController MovementController => _movementController;

        /// <summary> Возвращает контроллер состояний NPC. </summary>
        protected override NpcStateController StateController => _stateController;

        /// <summary> Событие, вызываемое при достижении NPC точки спавна для уничтожения. </summary>
        public Action OnReachedDespawnPoint;

        /// <summary> Внедряет зависимости Zenject. </summary>
        /// <param name="locationManager"> Менеджер локаций. </param>
        /// <param name="transactionService"> Сервис транзакций. </param>
        [Inject]
        private void Construct(LocationManager locationManager, TransactionService transactionService)
        {
            _locationManager = locationManager;
            _transactionService = transactionService;
        }

        /// <summary> Выполняет инициализацию компонентов NPC. </summary>
        protected override void Awake()
        {
            base.Awake();

            _movementController = new NonInteractableNpcMovementController(GetComponent<NavMeshAgent>(),
                transform, AnimationController);
            _movementController.Initialize();

            _stateController = new NonInteractableNpcStateController(_movementController, _locationManager,
                AnimationController, _transactionService, transform, GetComponent<NpcPurchaseIndicator>());
        }

        /// <summary> Устанавливает цель для перемещения NPC с автоматическим запуском
        /// случайной последовательности по прибытии. </summary>
        /// <param name="npcDestination"> Целевая позиция для перемещения. </param>
        public void SetDestination(NpcDestinationPoint npcDestination)
        {
            _movementController.SetPoint(npcDestination);
            _stateController.ForceSetState(NpcStateName.Movement);
            _movementController.OnDestinationReached += () =>
            {
                _stateController.ForceSetState(NpcStateName.Idle);
                _stateController.StartRandomSequence();
            };
        }

        /// <summary> Устанавливает точку исчезновения для неинтерактивного NPC. </summary>
        /// <param name="npcDestination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(NpcDestinationPoint npcDestination) =>
            (StateController as NonInteractableNpcStateController)?.SetDespawnPoint(npcDestination);

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (StateController == null) return;

            Gizmos.color = Color.yellow;
            var labelPosition = transform.position + Vector3.up * 2.5f;

            Handles.Label(labelPosition, StateController.CurrentNpcStateName.ToString());
        }

#endif
    }
}