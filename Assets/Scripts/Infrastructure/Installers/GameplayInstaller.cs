using System.Collections.Generic;
using FlavorfulStory.BuildingRepair.UI;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.DialogueSystem.UI;
using FlavorfulStory.Economy;
using FlavorfulStory.GridSystem;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.EquipmentSystem;
using FlavorfulStory.InventorySystem.ItemUsage;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Notifications;
using FlavorfulStory.Notifications.UI;
using FlavorfulStory.PickupSystem;
using FlavorfulStory.PlacementSystem;
using FlavorfulStory.Player;
using FlavorfulStory.QuestSystem;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.UI;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using FlavorfulStory.Stats;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.TimeManagement.UI;
using FlavorfulStory.Toolbar;
using FlavorfulStory.Toolbar.UI;
using FlavorfulStory.Tools;
using FlavorfulStory.TooltipSystem;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;
using FlavorfulStory.Visuals.Lightning;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Installers
{
    /// <summary> Установщик зависимостей, необходимых для игрового процесса. </summary>
    public class GameplayInstaller : MonoInstaller
    {
        #region Fields

        /// <summary> Префаб отображения требования ресурса. </summary>
        [SerializeField] private ResourceRequirementView _requirementViewPrefab;

        /// <summary> Префаб кнопки в списке квестов. </summary>
        [SerializeField] private QuestListButton _questListButtonPrefab;

        /// <summary> Виртуальная камера при телепорте. </summary>
        /// <remarks> Используется для WarpPortal при смене локаций. </remarks>
        [SerializeField] private CinemachineCamera _teleportVirtualCamera;

        /// <summary> Затемнитель интерфейса HUD. </summary>
        [SerializeField] private CanvasGroupFader _hudFader;

        /// <summary> Префаб всплывающей подсказки для предмета. </summary>
        [SerializeField] private ItemTooltipView _itemTooltipPrefab;

        /// <summary> Инвентарь игрока. </summary>
        [Header("Inventory")]
        [SerializeField] private Inventory _playerInventory;

        /// <summary> Префаб отображения ячейки инвентаря. </summary>
        [SerializeField] private InventorySlotView _inventorySlotViewPrefab;

        /// <summary> Индикатор клетки на гриде. </summary>
        [Header("Placement System")]
        [SerializeField] private GameObject _gridIndicator;

        /// <summary> Родительский контейнер для размещаемых объектов. </summary>
        [SerializeField] private Transform _placeableContainer;

        /// <summary> Сопоставления типов инструментов с их префабами для визуализации в руке игрока. </summary>
        [Header("Tools")]
        [SerializeField] private ToolPrefabMapping[] _toolMappings;

        /// <summary> Слои, по которым производится удар с помощью инструмента. </summary>
        [SerializeField] private LayerMask _hitableLayers;

        #endregion

        /// <summary> Выполнить установку всех зависимостей, необходимых для сцены. </summary>
        public override void InstallBindings()
        {
            DeclareSignals();

            BindBuildingRepair();
            BindDialogueSystem();
            BindEconomy();
            BindGridSystem();
            BindInventorySystem();
            BindNotifications();
            BindPlacementSystem();
            BindPlayer();
            BindQuestSystem();
            BindSceneManagement();
            BindStats();
            BindTimeManagement();
            BindTooltipSystem();
            BindUI();
            BindOther();
        }

        /// <summary> Объявить сигналы, используемые в сцене. </summary>
        private void DeclareSignals()
        {
            Container.DeclareSignal<MidnightStartedSignal>();
            Container.DeclareSignal<ItemCollectedSignal>();
            Container.DeclareSignal<QuestAddedSignal>();
            Container.DeclareSignal<DismantleDeniedSignal>();
            Container.DeclareSignal<ExhaustedSleepSignal>();

            Container.DeclareSignal<ToolbarSlotSelectedSignal>();
            Container.DeclareSignal<ToolbarHotkeyPressedSignal>();
            Container.DeclareSignal<ConsumeSelectedItemSignal>();
            Container.DeclareSignal<ClosestInteractableChangedSignal>();
        }

        /// <summary> Установить зависимости, связанные с ремонтом построек. </summary>
        private void BindBuildingRepair()
        {
            Container.Bind<RepairableBuildingView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPrefabFactory<ResourceRequirementView>>()
                .To<Factories.PrefabFactory<ResourceRequirementView>>().AsSingle()
                .WithArguments(_requirementViewPrefab);
        }

        /// <summary> Установить зависимости, связанные с системой диалогов. </summary>
        private void BindDialogueSystem()
        {
            Container.Bind<PlayerSpeaker>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DialogueModelPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DialogueView>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с экономикой. </summary>
        private void BindEconomy()
        {
            Container.Bind<PlayerWalletView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ICurrencyStorage>().WithId("Player").To<PlayerWallet>().AsSingle();
            Container.Bind<ICurrencyStorage>().WithId("Register").To<CashRegister>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системой грида. </summary>
        private void BindGridSystem()
        {
            Container.Bind<Grid>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GridPositionProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<GridSelectionService>().AsSingle().WithArguments(_gridIndicator);
        }

        /// <summary> Установить зависимости, связанные с системой инвентаря. </summary>
        private void BindInventorySystem()
        {
            Container.Bind<Inventory>().FromInstance(_playerInventory).AsSingle();
            Container.Bind<Equipment>().FromComponentInHierarchy().AsSingle();

            Container.Bind<IPrefabFactory<Pickup>>().To<Factories.PrefabFactory<Pickup>>().AsSingle();

            Container.Bind<IItemDropService>().To<ItemDropService>().AsSingle();
            Container.Bind<ISaveable>().To<ItemDropService>().FromResolve();
            Container.Bind<IInventoryProvider>().To<InventoryProvider>().AsSingle().NonLazy();
            Container.Bind<InventoryTransferService>().AsSingle();

            Container.Bind<IPrefabFactory<InventorySlotView>>().To<Factories.PrefabFactory<InventorySlotView>>()
                .AsSingle()
                .WithArguments(_inventorySlotViewPrefab);
            Container.Bind<InventoryExchangeWindow>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesTo<ToolUseController>().AsSingle();
            Container.BindInterfacesTo<EdibleUseController>().AsSingle();
            Container.BindInterfacesTo<PlaceableUseController>().AsSingle();

            Container.Bind<ToolbarView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<ToolHighlightHandler>().AsSingle();
            Container.Bind<ToolUsageService>().AsSingle().WithArguments(_toolMappings, _hitableLayers);
        }

        /// <summary> Установить зависимости, связанные с системой уведомлений. </summary>
        private void BindNotifications()
        {
            Container.BindInterfacesTo<SignalNotifier<MidnightStartedSignal>>().AsSingle();
            Container.BindInterfacesTo<SignalNotifier<ItemCollectedSignal>>().AsSingle();
            Container.BindInterfacesTo<SignalNotifier<QuestAddedSignal>>().AsSingle();
            Container.BindInterfacesTo<SignalNotifier<DismantleDeniedSignal>>().AsSingle();
            Container.BindInterfacesTo<SignalNotifier<SaveCompletedSignal>>().AsSingle();
            Container.BindInterfacesTo<SignalNotifier<ExhaustedSleepSignal>>().AsSingle();

            Container.Bind<NotificationAnchorLocator>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<NotificationService>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системой размещения объектов. </summary>
        private void BindPlacementSystem()
        {
            Container.Bind<PlacementPreview>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPrefabFactory<PlaceableObject>>().To<Factories.PrefabFactory<PlaceableObject>>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlacementController>().AsSingle().WithArguments(_placeableContainer);

            var placeables = FindObjectsByType<PlaceableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Container.Bind<IPlaceableObjectProvider>().To<PlaceableObjectProvider>().AsSingle()
                .WithArguments(new List<PlaceableObject>(placeables));
        }

        /// <summary> Установить зависимости, связанные с игроком. </summary>
        private void BindPlayer()
        {
            Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPlayerPositionProvider>().To<PlayerPositionProvider>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системой квестов. </summary>
        private void BindQuestSystem()
        {
            Container.Bind<QuestList>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IQuestList>().To<QuestList>().FromResolve();
            Container.Bind<IPrefabFactory<QuestListButton>>().To<Factories.PrefabFactory<QuestListButton>>().AsSingle()
                .WithArguments(_questListButtonPrefab);
            Container.Bind<QuestDescriptionView>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<ObjectiveProgressService>().AsSingle().NonLazy();
            Container.Bind<QuestExecutionContext>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с менеджментом локаций. </summary>
        private void BindSceneManagement()
        {
            Container.Bind<Location>().FromComponentsInHierarchy().AsCached();
            Container.Bind<List<Location>>().FromMethod(ctx =>
                new List<Location>(ctx.Container.ResolveAll<Location>())).AsSingle();
            Container.BindInterfacesAndSelfTo<LocationManager>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные со статами игрока. </summary>
        private void BindStats() =>
            Container.Bind<PlayerStats>().FromComponentInHierarchy().AsSingle();

        /// <summary> Установить зависимости, связанные с системой времени. </summary>
        private void BindTimeManagement()
        {
            Container.Bind<SummaryView>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<DayEndManager>().AsSingle();
            Container.Bind<PlayerSpawnService>().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системой тултипов. </summary>
        private void BindTooltipSystem()
        {
            Container.BindInterfacesAndSelfTo<ActionTooltipController>().AsSingle().NonLazy();
            Container.Bind<IActionTooltipViewSpawner>().To<ActionTooltipViewSpawner>().FromComponentInHierarchy()
                .AsSingle();
            Container.Bind<ItemTooltipView>().FromInstance(_itemTooltipPrefab).AsSingle();
        }

        /// <summary> Установить зависимости, связанные с пользовательским интерфейсом. </summary>
        private void BindUI()
        {
            Container.Bind<CanvasGroupFader>().WithId("HUD").FromInstance(_hudFader).AsSingle();
            Container.Bind<ConfirmationWindowView>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить общие или прочие зависимости. </summary>
        private void BindOther()
        {
            Container.Bind<GlobalLightSystem>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineCamera>().FromInstance(_teleportVirtualCamera).AsSingle();
        }
    }
}