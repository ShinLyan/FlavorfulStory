using System.Collections.Generic;
using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.FiniteStateMachine.InShopStates;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.SceneManagement.ShopLocation;
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

        private SequenceState _randomPointSequence;
        private SequenceState _furnitureSequence;
        private SequenceState _buyItemSequence;
        private SequenceState _refuseItemSequence;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="itemHandler"> Обработчик предмета. </param>
        public NonInteractableNpcStateController(NonInteractableNpcMovementController npcMovementController,
            LocationManager locationManager,
            NpcAnimationController npcAnimationController,
            ItemHandler itemHandler)
            : base(npcAnimationController)
        {
            _npcMovementController = npcMovementController;
            _locationManager = locationManager;
            _itemHandler = itemHandler;
            Initialize();
        }

        protected override void InitializeStates()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            _movementState = new MovementState(_npcMovementController);
            _animationState = new AnimationState();
            _furniturePickerState = new FurniturePickerState(_npcMovementController, shopLocation);
            _itemPickerState =
                new ItemPickerState(_npcMovementController, shopLocation, _itemHandler);
            _paymentState = new PaymentState();
            _randomPointPickerState = new RandomPointPickerState(_npcMovementController, shopLocation);
            _shelfPickerState = new ShelfPickerState(_npcMovementController, shopLocation);

            _randomPointSequence = new SequenceState(this,
                new CharacterState[] { _randomPointPickerState, _movementState, _animationState });

            _furnitureSequence = new SequenceState(this,
                new CharacterState[] { _furniturePickerState, _movementState, _animationState });

            _buyItemSequence = new SequenceState(this,
                new CharacterState[]
                {
                    _shelfPickerState, _movementState, _itemPickerState, _movementState, _paymentState
                });

            _refuseItemSequence = new SequenceState(this,
                new CharacterState[] { _shelfPickerState, _movementState, _animationState });

            var states = new CharacterState[]
            {
                _movementState, _animationState, _furniturePickerState, _itemPickerState, _paymentState,
                _randomPointPickerState, _shelfPickerState, _randomPointSequence, _furnitureSequence,
                _buyItemSequence, _refuseItemSequence
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;
            }
        }

        protected override void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(MovementState)); //TODO: think about initial state
        }

        /// <summary> Запускает последовательность состояний. </summary>
        public override void StartSequence(IEnumerable<CharacterState> states)
        {
            var sequence = new SequenceState(this, states);
            // _stateStack.Push(_currentState); //TODO: rework
            SetState(sequence.GetType());
        }

        /// <summary> Возвращает управление после завершения последовательности. </summary>
        public override void ReturnFromSequence()
        {
            // if (_stateStack.Count > 0)
            // {
            //     var previousState = _stateStack.Pop(); // TODO: rework
            //     SetState(previousState.GetType());
            // }
        }


        public override void Update() { base.Update(); }
    }
}