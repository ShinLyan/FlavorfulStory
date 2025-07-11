using FlavorfulStory.Infrastructure.Factories;
using Zenject;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Фабрика создания отображений требований ресурсов. </summary>
    public class ResourceRequirementViewFactory : GameFactoryBase<ResourceRequirementView>
    {
        /// <summary> Инициализирует фабрику с контейнером зависимостей и префабом отображения. </summary>
        /// <param name="container"> Контейнер зависимостей Zenject. </param>
        /// <param name="prefab"> Префаб отображения требования ресурса. </param>
        public ResourceRequirementViewFactory(DiContainer container, ResourceRequirementView prefab)
            : base(container, prefab)
        {
        }
    }
}