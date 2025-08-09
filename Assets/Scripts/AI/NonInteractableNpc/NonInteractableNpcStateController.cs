using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.FiniteStateMachine.ShopStates;
using FlavorfulStory.Economy;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using UnityEngine;
using AnimationState = FlavorfulStory.AI.FiniteStateMachine.ShopStates.AnimationState;

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

        /// <summary> Флаг, указывающий, посещал ли NPC мебель после покупки. </summary>
        private bool _hadVisitedFurnitureAfterPurchase;

        /// <summary> Точка, в которой NPC должен исчезнуть. </summary>
        private Vector3 _despawnPoint;

        /// <summary> Сервис для обработки транзакций и торговых операций. </summary>
        private readonly TransactionService _transactionService;

        private readonly NpcSpriteIndicator _spriteIndicator;

        #endregion

        /// <summary> Инициализирует новый экземпляр контроллера состояний для неинтерактивного NPC. </summary>
        /// <param name="npcMovementController"> Контроллер движения NPC, отвечающий за перемещение персонажа. </param>
        /// <param name="locationManager"> Менеджер локаций для получения информации о местоположениях. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC для управления анимациями персонажа. </param>
        /// <param name="transactionService"> Сервис для транзакций. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        /// <param name="npcSpriteIndicator">  </param>
        public NonInteractableNpcStateController(NonInteractableNpcMovementController npcMovementController,
            LocationManager locationManager, NpcAnimationController npcAnimationController,
            TransactionService transactionService, Transform npcTransform,
            NpcSpriteIndicator npcSpriteIndicator)
            : base(npcAnimationController, npcTransform)
        {
            _npcMovementController = npcMovementController;
            _locationManager = locationManager;
            _transactionService = transactionService;
            _spriteIndicator = npcSpriteIndicator;

            _hadVisitedFurnitureAfterPurchase = false;
            _despawnPoint = Vector3.zero;

            Initialize();
        }

        /// <summary> Инициализирует все состояния и последовательности NPC. </summary>
        protected override void InitializeStates()
        {
            CreateStates();
            CreateSequences();

            foreach (var state in _nameToCharacterStates.Values) state.OnStateChangeRequested += SetState;
        }

        /// <summary> Создает все базовые состояния NPC. </summary>
        private void CreateStates()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            _nameToCharacterStates.Add(StateName.Idle, new IdleState());
            _nameToCharacterStates.Add(StateName.Movement, new MovementState(_npcMovementController));
            _nameToCharacterStates.Add(StateName.Animation, new AnimationState(_animationController));
            _nameToCharacterStates.Add(StateName.FurniturePicker,
                new FurniturePickerState(_npcMovementController, shopLocation));
            _nameToCharacterStates.Add(StateName.ItemPicker,
                new ItemPickerState(_npcMovementController, shopLocation, _spriteIndicator));
            _nameToCharacterStates.Add(StateName.Payment,
                new PaymentState(_locationManager, _transactionService, _spriteIndicator));
            _nameToCharacterStates.Add(StateName.RandomPointPicker,
                new RandomPointPickerState(_npcMovementController, shopLocation));
            _nameToCharacterStates.Add(StateName.ShowcasePicker,
                new ShowcasePickerState(_npcMovementController, shopLocation));
            _nameToCharacterStates.Add(StateName.ReleaseObject, new ReleaseObjectState(shopLocation));
        }

        /// <summary> Создает все последовательности состояний для различных сценариев поведения NPC. </summary>
        private void CreateSequences()
        {
            _nameToCharacterStates.Add(StateName.RandomPointSequence, new SequenceState(
                new[]
                {
                    _nameToCharacterStates[StateName.RandomPointPicker],
                    _nameToCharacterStates[StateName.Movement],
                    _nameToCharacterStates[StateName.Animation]
                }));

            _nameToCharacterStates.Add(StateName.FurnitureSequence, new SequenceState(
                new[]
                {
                    _nameToCharacterStates[StateName.FurniturePicker],
                    _nameToCharacterStates[StateName.Movement],
                    _nameToCharacterStates[StateName.Animation],
                    _nameToCharacterStates[StateName.ReleaseObject]
                }));


            _nameToCharacterStates.Add(StateName.BuyItemSequence, new SequenceState(
                new[]
                {
                    _nameToCharacterStates[StateName.ShowcasePicker],
                    _nameToCharacterStates[StateName.Movement],
                    _nameToCharacterStates[StateName.Animation],
                    _nameToCharacterStates[StateName.ReleaseObject],
                    _nameToCharacterStates[StateName.ItemPicker],
                    _nameToCharacterStates[StateName.Movement],
                    _nameToCharacterStates[StateName.Payment],
                    _nameToCharacterStates[StateName.Animation],
                    _nameToCharacterStates[StateName.ReleaseObject]
                })
            );
            _nameToCharacterStates.Add(StateName.RefuseItemSequence, new SequenceState(
                new[]
                {
                    _nameToCharacterStates[StateName.ShowcasePicker],
                    _nameToCharacterStates[StateName.Movement],
                    _nameToCharacterStates[StateName.Animation],
                    _nameToCharacterStates[StateName.ReleaseObject]
                }));


            ((SequenceState)_nameToCharacterStates[StateName.BuyItemSequence]).OnSequenceEnded +=
                HandleAfterPurchaseTransition;
            ((SequenceState)_nameToCharacterStates[StateName.FurnitureSequence]).OnSequenceEnded +=
                HandleAfterFurnitureSequence;
            ((SequenceState)_nameToCharacterStates[StateName.RefuseItemSequence]).OnSequenceEnded +=
                StartRandomSequence;
            ((SequenceState)_nameToCharacterStates[StateName.RandomPointSequence]).OnSequenceEnded +=
                StartRandomSequence;
        }

        /// <summary> Сбрасывает все состояния в исходное состояние. </summary>
        protected override void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(StateName.Idle);
        }

        /// <summary> Запускает случайную последовательность действий NPC. </summary>
        public void StartRandomSequence() => SetState(CalculateNextSequence());

        /// <summary> Вычисляет следующую последовательность действий на основе доступных вариантов и весов. </summary>
        /// <returns> Имя выбранной последовательности. </returns>
        private StateName CalculateNextSequence()
        {
            var shopLocation = (ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop);

            bool areShowcasesAvailable = shopLocation.HasAvailableShowcaseWithItems();
            bool areFurnitureAvailable = !shopLocation.AreAllFurnitureOccupied();

            var availableOptions = new List<(StateName sequenceName, float weight)>();

            if (areShowcasesAvailable)
            {
                availableOptions.Add((StateName.BuyItemSequence, (float)(50 * 0.67)));
                availableOptions.Add((StateName.RefuseItemSequence, (float)(50 * 0.33)));
            }

            if (areFurnitureAvailable) availableOptions.Add((StateName.FurnitureSequence, 25f));

            availableOptions.Add((StateName.RandomPointSequence, 25f));

            if (availableOptions.Count == 0) return StateName.Idle;

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
                SetState(StateName.FurnitureSequence);
            }
            else
            {
                GoToDespawnPoint();
            }
        }

        /// <summary> Отправляет NPC в точку деспавна. </summary>
        private void GoToDespawnPoint()
        {
            _npcMovementController.SetPoint(_despawnPoint);
            SetState(StateName.Movement);
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
        /// <param name="destination"> Координаты точки, где NPC должен исчезнуть. </param>
        public void SetDespawnPoint(Vector3 destination) => _despawnPoint = destination;

        /// <summary> Принудительно устанавливает состояние NPC по строковому типу. </summary>
        /// <param name="stateName"> Строковое представление типа состояния для установки. </param>
        /// <remarks> Обёртка над методом SetState для принудительного изменения состояния. </remarks>
        public void ForceSetState(StateName stateName) => SetState(stateName);
    }
}