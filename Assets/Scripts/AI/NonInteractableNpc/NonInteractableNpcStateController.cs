using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FSM;
using FlavorfulStory.AI.FSM.ShopStates;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using AnimationState = FlavorfulStory.AI.FSM.AnimationState;
using Showcase = FlavorfulStory.Shop.Showcase;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public sealed class NonInteractableNpcStateController : NpcStateController
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NpcMovementController _npcMovementController;

        /// <summary> Менеджер локаций для получения информации о местоположениях. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Сервис для обработки транзакций и торговых операций. </summary>
        private readonly TransactionService _transactionService;

        /// <summary> Индикатор купленного товара у НПС. </summary>
        private readonly NpcPurchaseIndicator _purchaseIndicator;

        /// <summary> Флаг, указывающий, посещал ли NPC мебель после покупки. </summary>
        private bool _hadVisitedFurnitureAfterPurchase;

        /// <summary> Точка, в которой NPC должен исчезнуть. </summary>
        private NpcDestinationPoint _despawnPoint;

        /// <summary> Инициализирует новый экземпляр контроллера состояний для неинтерактивного NPC. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC, отвечающий за перемещение персонажа. </param>
        /// <param name="locationManager"> Менеджер локаций для получения информации о местоположениях. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC для управления анимациями персонажа. </param>
        /// <param name="transactionService"> Сервис для транзакций. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        /// <param name="npcPurchaseIndicator">  </param>
        public NonInteractableNpcStateController(NpcMovementController npcMovementController,
            LocationManager locationManager, NpcAnimationController npcAnimationController,
            TransactionService transactionService, Transform npcTransform, NpcPurchaseIndicator npcPurchaseIndicator)
            : base(npcAnimationController, npcTransform)
        {
            _npcMovementController = npcMovementController;
            _locationManager = locationManager;
            _transactionService = transactionService;
            _purchaseIndicator = npcPurchaseIndicator;

            _hadVisitedFurnitureAfterPurchase = false;
            _despawnPoint = new NpcDestinationPoint();
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeStates();
        }

        /// <summary> Инициализирует все состояния и последовательности NPC. </summary>
        private void InitializeStates()
        {
            CreateStates();
            CreateSequences();
        }

        /// <summary> Создает все базовые состояния NPC. </summary>
        private void CreateStates()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            _nameToCharacterStates.Add(NpcStateName.Movement,
                new MovementState(_npcMovementController, _animationController));
            _nameToCharacterStates.Add(NpcStateName.Animation, new AnimationState(_animationController));
            _nameToCharacterStates.Add(NpcStateName.FurniturePicker,
                new ShopObjectPickerState<Furniture>(() => shopLocation.GetAvailableFurniture()));
            _nameToCharacterStates.Add(NpcStateName.ItemPicker,
                new ItemPickerState(_npcMovementController, shopLocation, _purchaseIndicator));
            _nameToCharacterStates.Add(NpcStateName.Payment,
                new PaymentState(_locationManager, _transactionService, _purchaseIndicator));
            _nameToCharacterStates.Add(NpcStateName.RandomPointPicker,
                new RandomPointPickerState(shopLocation));
            _nameToCharacterStates.Add(NpcStateName.ShowcasePicker,
                new ShopObjectPickerState<Showcase>(() => shopLocation.GetRandomAvailableShowcaseWithItems()));
            _nameToCharacterStates.Add(NpcStateName.ReleaseObject, new ReleaseObjectState());
        }

        /// <summary> Создает все последовательности состояний для различных сценариев поведения NPC. </summary>
        private void CreateSequences()
        {
            _nameToCharacterStates.Add(NpcStateName.RandomPointSequence, new SequenceState(new[]
            {
                _nameToCharacterStates[NpcStateName.RandomPointPicker],
                _nameToCharacterStates[NpcStateName.Movement],
                _nameToCharacterStates[NpcStateName.Animation]
            }));

            _nameToCharacterStates.Add(NpcStateName.FurnitureSequence, new SequenceState(new[]
            {
                _nameToCharacterStates[NpcStateName.FurniturePicker],
                _nameToCharacterStates[NpcStateName.Movement],
                _nameToCharacterStates[NpcStateName.Animation],
                _nameToCharacterStates[NpcStateName.ReleaseObject]
            }));

            _nameToCharacterStates.Add(NpcStateName.BuyItemSequence, new SequenceState(new[]
                {
                    _nameToCharacterStates[NpcStateName.ShowcasePicker],
                    _nameToCharacterStates[NpcStateName.Movement],
                    _nameToCharacterStates[NpcStateName.Animation],
                    _nameToCharacterStates[NpcStateName.ReleaseObject],
                    _nameToCharacterStates[NpcStateName.ItemPicker],
                    _nameToCharacterStates[NpcStateName.Movement],
                    _nameToCharacterStates[NpcStateName.Payment],
                    _nameToCharacterStates[NpcStateName.Animation],
                    _nameToCharacterStates[NpcStateName.ReleaseObject]
                })
            );

            _nameToCharacterStates.Add(NpcStateName.RefuseItemSequence, new SequenceState(new[]
            {
                _nameToCharacterStates[NpcStateName.ShowcasePicker],
                _nameToCharacterStates[NpcStateName.Movement],
                _nameToCharacterStates[NpcStateName.Animation],
                _nameToCharacterStates[NpcStateName.ReleaseObject]
            }));

            ((SequenceState)_nameToCharacterStates[NpcStateName.BuyItemSequence]).OnSequenceEnded +=
                HandleAfterPurchaseTransition;
            ((SequenceState)_nameToCharacterStates[NpcStateName.FurnitureSequence]).OnSequenceEnded +=
                HandleAfterFurnitureSequence;
            ((SequenceState)_nameToCharacterStates[NpcStateName.RefuseItemSequence]).OnSequenceEnded +=
                StartRandomSequence;
            ((SequenceState)_nameToCharacterStates[NpcStateName.RandomPointSequence]).OnSequenceEnded +=
                StartRandomSequence;
        }

        /// <summary> Действия при сбросе контроллера. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        protected override void OnReset(DateTime currentTime) => ResetStates();

        /// <summary> Сбрасывает все состояния в исходное состояние. </summary>
        private void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(NpcStateName.Animation);
        }

        /// <summary> Запускает случайную последовательность действий NPC. </summary>
        public void StartRandomSequence() => SetState(CalculateNextSequence());

        /// <summary> Вычисляет следующую последовательность действий на основе доступных вариантов и весов. </summary>
        /// <returns> Имя выбранной последовательности. </returns>
        private NpcStateName CalculateNextSequence()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            bool areShowcasesAvailable = shopLocation.HasAvailableShowcaseWithItems();
            bool areFurnitureAvailable = shopLocation.HasAvailableFurniture();

            var availableOptions = new List<(NpcStateName sequenceName, float weight)>();

            if (areShowcasesAvailable)
            {
                availableOptions.Add((NpcStateName.RefuseItemSequence, (float)(50 * 0.33)));

                if (shopLocation.CashRegister.HasFreeAccessPoint())
                    availableOptions.Add((NpcStateName.BuyItemSequence, (float)(50 * 0.67)));
            }

            if (areFurnitureAvailable) availableOptions.Add((NpcStateName.FurnitureSequence, 25f));

            availableOptions.Add((NpcStateName.RandomPointSequence, 25f));

            float totalWeight = availableOptions.Sum(x => x.weight);
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var option in availableOptions)
            {
                currentWeight += option.weight;
                if (randomValue <= currentWeight) return option.sequenceName;
            }

            return availableOptions.Last().sequenceName;
        }

        /// <summary> Обрабатывает переход после завершения покупки. </summary>
        private void HandleAfterPurchaseTransition()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);
            bool isFurnitureAvailable = !shopLocation.HasAvailableFurniture();

            if (isFurnitureAvailable && Random.Range(0, 2) == 0 && !_hadVisitedFurnitureAfterPurchase)
            {
                _hadVisitedFurnitureAfterPurchase = true;
                SetState(NpcStateName.FurnitureSequence);
            }
            else
            {
                var context = new StateContext();
                context.Set(FsmContextType.DestinationPoint, _despawnPoint);
                _nameToCharacterStates[NpcStateName.Movement].SetContext(context);
                SetState(NpcStateName.Movement);
            }
        }

        /// <summary> Обрабатывает переход после завершения последовательности с мебелью. </summary>
        private void HandleAfterFurnitureSequence()
        {
            if (_hadVisitedFurnitureAfterPurchase)
                HandleAfterPurchaseTransition();
            else
                StartRandomSequence();
        }

        /// <summary> Устанавливает точку исчезновения для NPC. </summary>
        /// <param name="npcDestination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(NpcDestinationPoint npcDestination) => _despawnPoint = npcDestination;

        /// <summary> Принудительно устанавливает состояние NPC по строковому типу. </summary>
        /// <param name="npcStateName"> Строковое представление типа состояния для установки. </param>
        /// <param name="context"> Контекст для состояния. </param>
        /// <remarks> Обёртка над методом SetState для принудительного изменения состояния. </remarks>
        public void ForceSetState(NpcStateName npcStateName, StateContext context = null)
        {
            _nameToCharacterStates[npcStateName].SetContext(context);
            SetState(npcStateName);
        }
    }
}