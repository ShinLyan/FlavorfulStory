using FlavorfulStory.BuildingRepair;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.EquipmentSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.UI;
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

        /// <summary> Выполняет установку всех зависимостей, необходимых для сцены. </summary>
        public override void InstallBindings()
        {
            BindGameplay();
            BindUI();
        }

        /// <summary> Установить зависимости, связанные с игровыми объектами и логикой. </summary>
        private void BindGameplay()
        {
            Container.Bind<Inventory>().FromInstance(_playerInventory).AsSingle();
            Container.Bind<Equipment>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
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
    }
}