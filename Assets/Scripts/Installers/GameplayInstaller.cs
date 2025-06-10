using FlavorfulStory.BuildingRepair;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.EquipmentSystem;
using FlavorfulStory.InventorySystem.UI;
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

        /// <summary> Установить привязки зависимостей для игровых компонентов. </summary>
        public override void InstallBindings()
        {
            Container.Bind<Inventory>().FromInstance(_playerInventory).AsSingle();

            Container.Bind<IGameFactory<InventorySlotView>>()
                .To<InventorySlotViewFactory>()
                .AsSingle()
                .WithArguments(_inventorySlotViewPrefab);

            Container.Bind<IGameFactory<ResourceRequirementView>>()
                .To<ResourceRequirementViewFactory>()
                .AsSingle()
                .WithArguments(_requirementViewPrefab);

            Container.Bind<Equipment>().FromComponentInHierarchy().AsSingle();
        }
    }
}