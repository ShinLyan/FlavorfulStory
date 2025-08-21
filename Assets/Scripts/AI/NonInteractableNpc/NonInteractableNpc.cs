using System;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FSM;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Неинтерактивный NPC, который может перемещаться и выполнять последовательности действий. </summary>
    [RequireComponent(typeof(NpcPurchaseIndicator))]
    public sealed class NonInteractableNpc : Npc
    {
        /// <summary> Стейт-контроллер для неинтерктивного НПС. </summary>
        private NonInteractableNpcStateController _stateController;

        /// <summary> Менеджер локаций для управления переходами между локациями. </summary>
        private LocationManager _locationManager;

        /// <summary> Сервис для транзакций. </summary>
        private TransactionService _transactionService;

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

            _stateController = new NonInteractableNpcStateController(MovementController, _locationManager,
                AnimationController, _transactionService, transform, GetComponent<NpcPurchaseIndicator>());
        }

        /// <summary> Устанавливает цель для перемещения NPC с автоматическим запуском
        /// случайной последовательности по прибытии. </summary>
        /// <param name="npcDestination"> Целевая позиция для перемещения. </param>
        public void SetDestination(NpcDestinationPoint npcDestination)
        {
            var context = new StateContext();
            context.Set(FsmContextType.DestinationPoint, npcDestination);

            _stateController.ForceSetState(NpcStateName.Movement, context);
            MovementController.OnDestinationReached += () => _stateController.StartRandomSequence();
        }

        /// <summary> Устанавливает точку исчезновения для неинтерактивного NPC. </summary>
        /// <param name="npcDestination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(NpcDestinationPoint npcDestination)
        {
            _stateController.SetDespawnPoint(npcDestination);
            OnReachedDespawnPoint += () => _stateController.ForceSetState(NpcStateName.Animation);
        }
    }
}