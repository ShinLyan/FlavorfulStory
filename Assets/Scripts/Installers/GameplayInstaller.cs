using System.Collections.Generic;
using FlavorfulStory.BuildingRepair;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.EquipmentSystem;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Lightning;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
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

        /// <summary> Виртуальная камера при телепорте. </summary>
        /// <remarks> Используется для WarpPortal, когда отключаем и включаем камеру
        /// при переходе между локациями. </remarks>
        [SerializeField] private CinemachineCamera _teleportVirtualCamera;

        /// <summary> Выполняет установку всех зависимостей, необходимых для сцены. </summary>
        public override void InstallBindings()
        {
            BindGameplay();
            BindUI();
            BindSystems();
        }

        /// <summary> Установить зависимости, связанные с игровыми объектами и логикой. </summary>
        private void BindGameplay()
        {
            Container.Bind<Inventory>().FromInstance(_playerInventory).AsSingle();
            Container.Bind<PickupNotificationManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Equipment>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();

            Container.Bind<PickupFactory>().AsSingle();
            Container.Bind<ItemDropper>().FromComponentsInHierarchy().AsCached();
            Container.Bind<PickupSpawner>().FromComponentsInHierarchy().AsCached();

            Container.Bind<BuildingRepairView>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с пользовательским интерфейсом. </summary>
        private void BindUI()
        {
            Container.Bind<IGameFactory<InventorySlotView>>().To<InventorySlotViewFactory>().AsSingle()
                .WithArguments(_inventorySlotViewPrefab);
            Container.Bind<IGameFactory<ResourceRequirementView>>().To<ResourceRequirementViewFactory>().AsSingle()
                .WithArguments(_requirementViewPrefab);
            Container.Bind<ConfirmationWindowView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SummaryView>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary> Установить зависимости, связанные с не игровыми системами и логикой. </summary>
        private void BindSystems()
        {
            Container.Bind<GlobalLightSystem>().FromComponentInHierarchy().AsSingle();

            Container.Bind<Location>().FromComponentsInHierarchy().AsCached();
            Container.Bind<List<Location>>().FromMethod(ctx =>
                new List<Location>(ctx.Container.ResolveAll<Location>())).AsSingle();
            Container.BindInterfacesAndSelfTo<LocationManager>().AsSingle();

            Container.Bind<CinemachineCamera>().FromInstance(_teleportVirtualCamera).AsSingle();
        }
    }
}