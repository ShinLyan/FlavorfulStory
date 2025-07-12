using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.FiniteStateMachine.InShopStates;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;
using AnimationState = FlavorfulStory.AI.FiniteStateMachine.InShopStates.AnimationState;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class NonInteractableNpcStateController : StateController
    {
        private readonly NonInteractableNpcMovementController _npcMovementController;
        private readonly LocationManager _locationManager;
        private readonly ItemHandler _itemHandler;

        private MovementState _movementState;
        private AnimationState _animationState;
        private FurniturePickerState _furniturePickerState;
        private ItemPickerState _itemPickerState;
        private PaymentState _paymentState;
        private RandomPointPickerState _randomPointPickerState;
        private ShelfPickerState _shelfPickerState;
        private WaitingState _waitingState;
        private RefuseItemState _refuseItemState;

        private SequenceState _randomPointSequence;
        private SequenceState _furnitureSequence;
        private SequenceState _buyItemSequence;
        private SequenceState _refuseItemSequence;

        private readonly PlayerController _playerController;
        private readonly Transform _npcTransform;

        private bool _hadVisitedFurnitureAfterPurchase;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="itemHandler"> Обработчик предмета. </param>
        public NonInteractableNpcStateController(NonInteractableNpcMovementController npcMovementController,
            LocationManager locationManager,
            NpcAnimationController npcAnimationController,
            ItemHandler itemHandler,
            PlayerController playerController, Transform npcTransform)
            : base(npcAnimationController)
        {
            _npcMovementController = npcMovementController;
            _locationManager = locationManager;
            _itemHandler = itemHandler;
            _playerController = playerController;
            _npcTransform = npcTransform;

            _hadVisitedFurnitureAfterPurchase = false;

            Initialize();
        }

        protected override void InitializeStates()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            CreateStates(shopLocation);
            CreateSequences();

            var states = new CharacterState[]
            {
                _movementState, _animationState, _furniturePickerState, _itemPickerState, _paymentState,
                _randomPointPickerState, _shelfPickerState, _waitingState
            };

            foreach (var state in states)
            {
                _nameToCharacterStates.Add(state.GetType().ToString(), state);
                state.OnStateChangeRequested += SetState;
            }
        }

        private void CreateStates(ShopLocation shopLocation)
        {
            _movementState = new MovementState(_npcMovementController);
            _animationState = new AnimationState();
            _furniturePickerState = new FurniturePickerState(_npcMovementController, shopLocation);
            _itemPickerState =
                new ItemPickerState(_npcMovementController, shopLocation, _itemHandler);
            _paymentState = new PaymentState(shopLocation, _itemHandler);
            _randomPointPickerState = new RandomPointPickerState(_npcMovementController, shopLocation);
            _shelfPickerState = new ShelfPickerState(_npcMovementController, shopLocation);
            _waitingState = new WaitingState(_playerController, _npcTransform);
            _refuseItemState = new RefuseItemState(shopLocation);
        }

        private void CreateSequences()
        {
            _randomPointSequence = new SequenceState(this,
                new CharacterState[] { _randomPointPickerState, _movementState, _animationState });

            _furnitureSequence = new SequenceState(this,
                new CharacterState[] { _furniturePickerState, _movementState, _animationState });

            _buyItemSequence = new SequenceState(this,
                new CharacterState[]
                {
                    _shelfPickerState, _movementState, _animationState, _itemPickerState, _movementState,
                    _animationState, _paymentState
                });

            _refuseItemSequence = new SequenceState(this,
                new CharacterState[] { _shelfPickerState, _movementState, _refuseItemState, _animationState });

            _nameToCharacterStates.Add("_randomPointSequence", _randomPointSequence);
            _nameToCharacterStates.Add("_furnitureSequence", _furnitureSequence);
            _nameToCharacterStates.Add("_buyItemSequence", _buyItemSequence);
            _nameToCharacterStates.Add("_refuseItemSequence", _refuseItemSequence);

            _buyItemSequence.OnSequenceEnded += HandleAfterPurchaseTransition;
            _furnitureSequence.OnSequenceEnded += HandleAfterFurnitureSequence;
            _refuseItemSequence.OnSequenceEnded += StartRandomSequence;
            _randomPointSequence.OnSequenceEnded += StartRandomSequence;
        }

        protected override void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(typeof(WaitingState).ToString()); //TODO: think about initial state
        }

        /// <summary> Возвращает управление после завершения последовательности. </summary>
        public override void ReturnFromSequence() { SetState(typeof(WaitingState).ToString()); }


        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space)) StartRandomSequence();
        }

        private void StartRandomSequence()
        {
            string selectedSequence = CalculateNextSequence();
            SetState(selectedSequence);
        }

        private string CalculateNextSequence()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            bool areShelvesAvailable = !shopLocation.AreAvailableShelvesEmpty();
            bool areFurnitureAvailable = !shopLocation.AreAllFurnitureOccupied();

            var availableOptions = new List<(string sequenceName, float weight)>();

            if (areShelvesAvailable)
            {
                availableOptions.Add(("_buyItemSequence", (float)(50 * 0.67)));
                availableOptions.Add(("_refuseItemSequence", (float)(50 * 0.33)));
            }

            if (areFurnitureAvailable) availableOptions.Add(("_furnitureSequence", 25f));

            availableOptions.Add(("_randomPointSequence", 25f));

            if (availableOptions.Count == 0) return typeof(WaitingState).ToString();

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

        private void HandleAfterPurchaseTransition()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);
            bool isFurnitureAvailable = !shopLocation.AreAllFurnitureOccupied();

            if (isFurnitureAvailable && Random.Range(0, 2) == 0 && !_hadVisitedFurnitureAfterPurchase)
            {
                _hadVisitedFurnitureAfterPurchase = true;
                SetState("_furnitureSequence");
            }
            else
            {
                //TODO: отправить в точку деспавна
                var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
                point.Position = _playerController.transform.position;
                point.LocationName = LocationName.RockyIsland;

                _npcMovementController.SetPoint(point);
                SetState(typeof(MovementState).ToString());
            }
        }

        private void HandleAfterFurnitureSequence()
        {
            if (_hadVisitedFurnitureAfterPurchase)
                HandleAfterPurchaseTransition();
            else
                StartRandomSequence();
        }
    }
}