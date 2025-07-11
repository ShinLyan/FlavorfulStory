using System.Collections.Generic;
using FlavorfulStory.BuildingRepair.UI;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.DialogueSystem.UI;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.EquipmentSystem;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.QuestSystem;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.TooltipSystem;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;
using FlavorfulStory.Visuals.Lightning;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Installers
{
    /// <summary> Установщик зависимостей, необходимых для игрового процесса. </summary>
    public class GameplayInstaller : MonoInstaller
    {
        /// <summary> Инвентарь игрока. </summary>
        [SerializeField] private Inventory _playerInventory;

        /// <summary> Префаб отображения ячейки инвентаря. </summary>
        [SerializeField] private InventorySlotView _inventorySlotViewPrefab;

        /// <summary> Префаб отображения требования ресурса. </summary>
        [SerializeField] private ResourceRequirementView _requirementViewPrefab;

        /// <summary> Префаб кнопки в списке квестов. </summary>
        [SerializeField] private QuestListButton _questListButtonPrefab;

        /// <summary> Виртуальная камера при телепорте. </summary>
        /// <remarks> Используется для WarpPortal, когда отключаем и включаем камеру
        /// при переходе между локациями. </remarks>
        [SerializeField] private CinemachineCamera _teleportVirtualCamera;

        /// <summary> Затемнитель интерфейса HUD. </summary>
        [SerializeField] private CanvasGroupFader _hudFader;

        /// <summary> Выполняет установку всех зависимостей, необходимых для сцены. </summary>
        public override void InstallBindings()
        {
            BindPlayer();
            BindInventory();
            BindDialogue();
            BindQuests();
            BindUI();
            BindSystems();
        }

        /// <summary> Установить зависимости, связанные с игроком. </summary>
        private void BindPlayer()
        {
            Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Equipment>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с инвентарем. </summary>
        private void BindInventory()
        {
            Container.Bind<Inventory>().FromInstance(_playerInventory).AsSingle();
            Container.Bind<PickupNotificationManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PickupFactory>().AsSingle();
            Container.Bind<ItemDropper>().FromComponentsInHierarchy().AsCached();
            Container.Bind<PickupSpawner>().FromComponentsInHierarchy().AsCached();

            Container.Bind<IGameFactory<InventorySlotView>>().To<InventorySlotViewFactory>().AsSingle()
                .WithArguments(_inventorySlotViewPrefab);
            Container.Bind<IGameFactory<ResourceRequirementView>>().To<ResourceRequirementViewFactory>().AsSingle()
                .WithArguments(_requirementViewPrefab);
        }

        /// <summary> Установить зависимости, связанные с системой диалогов. </summary>
        private void BindDialogue()
        {
            Container.Bind<PlayerSpeaker>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DialogueModelPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DialogueView>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системой квестов. </summary>
        private void BindQuests()
        {
            Container.Bind<QuestList>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IGameFactory<QuestListButton>>().To<QuestListButtonFactory>().AsSingle()
                .WithArguments(_questListButtonPrefab);
            Container.Bind<QuestDescriptionView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ObjectiveProgressService>().AsSingle().NonLazy();
        }

        /// <summary> Установить зависимости, связанные с пользовательским интерфейсом. </summary>
        private void BindUI()
        {
            Container.Bind<ConfirmationWindowView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SummaryView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<RepairableBuildingView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IActionTooltipShower>().To<ActionTooltipShower>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CanvasGroupFader>().WithId("HUD").FromInstance(_hudFader).AsSingle();
        }

        /// <summary> Установить зависимости, связанные с системами и логикой. </summary>
        private void BindSystems()
        {
            Container.Bind<GlobalLightSystem>().FromComponentInHierarchy().AsSingle();

            Container.Bind<Location>().FromComponentsInHierarchy().AsCached();
            Container.Bind<List<Location>>().FromMethod(ctx =>
                new List<Location>(ctx.Container.ResolveAll<Location>())).AsSingle();
            Container.BindInterfacesAndSelfTo<LocationManager>().AsSingle();

            Container.Bind<CinemachineCamera>().FromInstance(_teleportVirtualCamera).AsSingle();

            Container.Bind<SleepTrigger>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<DayEndManager>().AsSingle();
        }
    }
}