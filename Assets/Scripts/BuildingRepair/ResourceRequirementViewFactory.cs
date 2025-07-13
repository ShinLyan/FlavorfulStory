using FlavorfulStory.Infrastructure.Factories;
using Zenject;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Фабрика создания отображений требований ресурсов. </summary>
    public class ResourceRequirementViewFactory : GameFactoryBase<ItemRequirementView>
    {
        /// <summary> Инициализирует фабрику с контейнером зависимостей и префабом отображения. </summary>
        /// <param name="container"> Контейнер зависимостей Zenject. </param>
        /// <param name="prefab"> Префаб отображения требования ресурса. </param>
        public ResourceRequirementViewFactory(DiContainer container, ItemRequirementView prefab)
            : base(container, prefab)
        {
        }
    }
}