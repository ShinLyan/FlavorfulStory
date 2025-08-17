using FlavorfulStory.Crafting;
using FlavorfulStory.InventorySystem;
using Zenject;

namespace FlavorfulStory
{
    public class CraftingInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CraftingRecipeService>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();

            //TODO: Инициализацию статики лучше вынести в IInitializable, чтобы не делать Resolve в InstallBindings
            Container.Bind<IInitializable>().To<CraftingBootstrap>().AsSingle();
            Container.Bind<RecipeUnlockedNotificationWindow>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<RecipeUnlockedNotificationPresenter>().AsSingle();
        }
    }
    
    public sealed class CraftingBootstrap : IInitializable
    {
        private readonly ICraftingRecipeProvider _provider;
        private readonly IInventoryProvider _inventoryProvider;

        public CraftingBootstrap(
            ICraftingRecipeProvider provider,
            IInventoryProvider inventoryProvider)
        {
            _provider = provider;
            _inventoryProvider = inventoryProvider;
        }

        public void Initialize() => CraftingProcessor.Init(_provider, _inventoryProvider);
    }
}