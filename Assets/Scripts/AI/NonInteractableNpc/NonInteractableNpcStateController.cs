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
        #region Fields

        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _npcMovementController;

        /// <summary> Менеджер локаций для получения информации о местоположениях. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Обработчик предметов для работы с игровыми объектами. </summary>
        private readonly ItemHandler _itemHandler;

        /// <summary> Состояние перемещения NPC. </summary>
        private MovementState _movementState;

        /// <summary> Состояние анимации NPC. </summary>
        private AnimationState _animationState;

        /// <summary> Состояние выбора мебели. </summary>
        private FurniturePickerState _furniturePickerState;

        /// <summary> Состояние выбора предмета. </summary>
        private ItemPickerState _itemPickerState;

        /// <summary> Состояние оплаты. </summary>
        private PaymentState _paymentState;

        /// <summary> Состояние выбора случайной точки. </summary>
        private RandomPointPickerState _randomPointPickerState;

        /// <summary> Состояние выбора полки. </summary>
        private ShelfPickerState _shelfPickerState;

        /// <summary> Состояние отказа от предмета. </summary>
        private RefuseItemState _refuseItemState;

        /// <summary> Состояние ожидания. </summary>
        private WaitingState _waitingState;

        /// <summary> Последовательность перемещения к случайной точке. </summary>
        private SequenceState _randomPointSequence;

        /// <summary> Последовательность взаимодействия с мебелью. </summary>
        private SequenceState _furnitureSequence;

        /// <summary> Последовательность покупки предмета. </summary>
        private SequenceState _buyItemSequence;

        /// <summary> Последовательность отказа от предмета. </summary>
        private SequenceState _refuseItemSequence;

        /// <summary> Контроллер игрока для взаимодействия с игроком. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Transform компонент NPC для получения информации о позиции и трансформации. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Флаг, указывающий, посещал ли NPC мебель после покупки. </summary>
        private bool _hadVisitedFurnitureAfterPurchase;

        private SchedulePoint _despawnPoint;

        #endregion

        /// <summary> Инициализирует новый экземпляр контроллера состояний для неинтерактивного NPC. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC, отвечающий за перемещение персонажа. </param>
        /// <param name="locationManager"> Менеджер локаций для получения информации о местоположениях. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC для управления анимациями персонажа. </param>
        /// <param name="itemHandler"> Обработчик предметов для работы с игровыми объектами. </param>
        /// <param name="playerController"> Контроллер игрока для взаимодействия с игроком. </param>
        /// <param name="npcTransform"> Transform компонент NPC для получения информации о позиции и трансформации. </param>
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
            _despawnPoint = null;

            Initialize();
        }

        /// <summary> Инициализирует все состояния и последовательности NPC. </summary>
        protected override void InitializeStates()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            CreateStates(shopLocation);
            CreateSequences();

            var states = new CharacterState[]
            {
                _movementState, _animationState, _furniturePickerState, _itemPickerState, _paymentState,
                _randomPointPickerState, _shelfPickerState, _refuseItemState, _waitingState
            };

            foreach (var state in states)
            {
                _nameToCharacterStates.Add(state.GetType().ToString(), state);
                state.OnStateChangeRequested += SetState;
            }
        }

        /// <summary> Создает все базовые состояния NPC. </summary>
        /// <param name="shopLocation"> Локация магазина для инициализации состояний. </param>
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

        /// <summary> Создает все последовательности состояний для различных сценариев поведения NPC. </summary>
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

        /// <summary> Сбрасывает все состояния в исходное состояние. </summary>
        protected override void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(typeof(WaitingState).ToString()); //TODO: think about initial state
        }

        /// <summary> Запускает случайную последовательность действий NPC. </summary>
        public void StartRandomSequence()
        {
            string selectedSequence = CalculateNextSequence();
            SetState(selectedSequence);
        }

        /// <summary> Вычисляет следующую последовательность действий на основе доступных вариантов и весов. </summary>
        /// <returns> Имя выбранной последовательности. </returns>
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

        /// <summary> Обрабатывает переход после завершения покупки. </summary>
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
                GoToDespawnPoint();
            }
        }

        /// <summary> Отправляет NPC в точку деспавна. </summary>
        private void GoToDespawnPoint()
        {
            //TODO: переделать после удаление WarpGraph
            _npcMovementController.SetPoint(_despawnPoint);
            SetState(typeof(MovementState).ToString());
        }

        /// <summary> Обрабатывает переход после завершения последовательности с мебелью. </summary>
        private void HandleAfterFurnitureSequence()
        {
            if (_hadVisitedFurnitureAfterPurchase)
                HandleAfterPurchaseTransition();
            else
                StartRandomSequence();
        }

        public void SetDespawnPoint(Vector3 destination, LocationName locationName = LocationName.RockyIsland)
        {
            var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
            point.Position = destination;
            point.LocationName = locationName;

            _despawnPoint = point;
        }

        public void ForceSetState(string type) => SetState(type);
    }
}