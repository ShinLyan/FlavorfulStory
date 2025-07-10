using FlavorfulStory.Actions;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.FiniteStateMachine.InShopStates;
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

        private SequenceState _randomPointSequence;
        private SequenceState _furnitureSequence;
        private SequenceState _buyItemSequence;
        private SequenceState _refuseItemSequence;

        private readonly PlayerController _playerController;
        private readonly Transform _npcTransform;

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
            _waitingState = new WaitingState(_playerController, _npcTransform);

            _randomPointSequence = new SequenceState(this,
                new CharacterState[] { _randomPointPickerState, _movementState, _animationState });
            _typeToCharacterStates.Add("_randomPointSequence", _randomPointSequence);

            _furnitureSequence = new SequenceState(this,
                new CharacterState[] { _furniturePickerState, _movementState, _animationState });
            _typeToCharacterStates.Add("_furnitureSequence", _furnitureSequence);

            _buyItemSequence = new SequenceState(this,
                new CharacterState[]
                {
                    _shelfPickerState, _movementState, _itemPickerState, _movementState, _paymentState
                });
            _typeToCharacterStates.Add("_buyItemSequence", _buyItemSequence);

            _refuseItemSequence = new SequenceState(this,
                new CharacterState[] { _shelfPickerState, _movementState, _animationState });
            _typeToCharacterStates.Add("_refuseItemSequence", _refuseItemSequence);

            var states = new CharacterState[]
            {
                _movementState, _animationState, _furniturePickerState, _itemPickerState, _paymentState,
                _randomPointPickerState, _shelfPickerState, _waitingState
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType().ToString(), state);
                state.OnStateChangeRequested += SetState;
            }
        }

        protected override void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(WaitingState).ToString()); //TODO: think about initial state
        }

        /// <summary> Запускает последовательность состояний. </summary>
        public override void StartSequence(string sequenceName)
        {
            // _stateStack.Push(_currentState); //TODO: rework
            SetState(sequenceName);
        }

        /// <summary> Возвращает управление после завершения последовательности. </summary>
        public override void ReturnFromSequence()
        {
            SetState(typeof(WaitingState).ToString());
            // if (_stateStack.Count > 0)
            // {
            //     var previousState = _stateStack.Pop(); // TODO: rework
            //     SetState(previousState.GetType());
            // }
        }


        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space)) StartRandomSequence();
        }

        private readonly string[] _availableSequences =
        {
            "_randomPointSequence", "_furnitureSequence", "_buyItemSequence", "_refuseItemSequence"
        };

        private void StartRandomSequence()
        {
            int randomIndex = Random.Range(0, _availableSequences.Length);
            string randomSequence = _availableSequences[randomIndex];

            // Debug.Log("==================================");
            // Debug.Log(_npcTransform.name + " Starting random sequence: " + randomSequence);
            StartSequence("_buyItemSequence");
        }
    }
}