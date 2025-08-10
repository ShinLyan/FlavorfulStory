using FlavorfulStory.Crafting;
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

            // Инициализацию статики лучше вынести в IInitializable, чтобы не делать Resolve в InstallBindings
            Container.Bind<IInitializable>().To<CraftingBootstrap>().AsSingle();
        }
    }
    
    public sealed class CraftingBootstrap : IInitializable
    {
        private readonly ICraftingRecipeProvider _provider;
        public CraftingBootstrap(ICraftingRecipeProvider provider) => _provider = provider;
        public void Initialize() => CraftingProcessor.Init(_provider);
    }
}