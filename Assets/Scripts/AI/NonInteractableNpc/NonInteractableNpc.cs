using System;
using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Неинтерактивный NPC, который может перемещаться и выполнять последовательности действий. </summary>
    [RequireComponent(typeof(ItemHandler))]
    public class NonInteractableNpc : Npc
    {
        /// <summary> Менеджер локаций для управления переходами между локациями. </summary>
        [Inject] private LocationManager _locationManager;

        [Inject] private TransactionService _transactionService;

        /// <summary> Обработчик предметов для взаимодействия с объектами. </summary>
        private ItemHandler _itemHandler;

        /// <summary> Событие, вызываемое при достижении NPC точки спавна для уничтожения. </summary>
        public Action OnReachedDespawnPoint;

        /// <summary> Инициализирует компоненты неинтерактивного NPC. </summary>
        protected override void Awake()
        {
            _itemHandler = GetComponent<ItemHandler>();
            base.Awake();

            Debug.Log("Неинтерактивный NPC инициализирован: " + _itemHandler);
        }

        protected override void Start() { base.Start(); }

        /// <summary> Создает контроллер движения для неинтерактивного NPC. </summary>
        /// <returns> Экземпляр контроллера движения неинтерактивного NPC. </returns>
        protected override NpcMovementController CreateMovementController()
        {
            return new NonInteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
                _animationController
            );
        }

        /// <summary> Создает контроллер состояний для неинтерактивного NPC. </summary>
        /// <returns> Экземпляр контроллера состояний неинтерактивного NPC. </returns>
        protected override StateController CreateStateController()
        {
            return new NonInteractableNpcStateController(
                _movementController as NonInteractableNpcMovementController,
                _locationManager,
                _animationController,
                _itemHandler,
                _transactionService
            );
        }

        /// <summary> Устанавливает цель для перемещения NPC с автоматическим запуском случайной последовательности по прибытии. </summary>
        /// <param name="destination"> Целевая позиция для перемещения. </param>
        /// <param name="locationName"> Название локации назначения. </param>
        public void SetDestination(Vector3 destination, LocationName locationName)
        {
            var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
            point.Position = destination;
            point.LocationName = locationName;

            ((NonInteractableNpcMovementController)_movementController).SetPoint(point);
            ((NonInteractableNpcStateController)_stateController).ForceSetState(typeof(MovementState).ToString());
            ((NonInteractableNpcMovementController)_movementController).OnDestinationReached += () =>
                (_stateController as NonInteractableNpcStateController)?.StartRandomSequence();
        }

        /// <summary> Устанавливает точку исчезновения для неинтерактивного NPC. </summary>
        /// <param name="destination"> Координаты точки, где NPC должен исчезнуть. </param>
        /// <param name="locationName"> Локация, в которой находится точка исчезновения. По умолчанию RockyIsland. </param>
        public void SetDespawnPoint(Vector3 destination, LocationName locationName = LocationName.RockyIsland) =>
            (_stateController as NonInteractableNpcStateController)?.SetDespawnPoint(destination, locationName);
    }
}