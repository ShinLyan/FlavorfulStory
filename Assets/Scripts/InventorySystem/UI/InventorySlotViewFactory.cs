using FlavorfulStory.Infrastructure.Factories;
using Zenject;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Фабрика создания отображений ячеек инвентаря. </summary>
    public class InventorySlotViewFactory : GameFactoryBase<InventorySlotView>
    {
        /// <summary> Инициализирует фабрику с контейнером зависимостей и префабом отображения ячейки. </summary>
        /// <param name="container"> Контейнер зависимостей Zenject. </param>
        /// <param name="prefab"> Префаб отображения ячейки инвентаря. </param>
        public InventorySlotViewFactory(DiContainer container, InventorySlotView prefab) : base(container, prefab)
        {
        }
    }
}