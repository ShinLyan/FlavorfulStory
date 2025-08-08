using FlavorfulStory.PlacementSystem;
using FlavorfulStory.TimeManagement;
using Zenject;

namespace FlavorfulStory.InventorySystem.ItemUsage
{
    /// <summary> Контроллер использования предметов, которые можно размещать в игровом мире. </summary>
    public class PlaceableUseController : ItemUseController<PlaceableItem>
    {
        /// <summary> Контроллер, управляющий процессом размещения объектов. </summary>
        private readonly PlacementController _placementController;

        /// <summary> Создаёт контроллер для управления режимом размещения. </summary>
        /// <param name="signalBus"> Шина сигналов для подписки на смену предмета. </param>
        /// <param name="placementControllerController"> Контроллер, управляющий процессом размещения объектов. </param>
        public PlaceableUseController(SignalBus signalBus, PlacementController placementControllerController)
            : base(signalBus) => _placementController = placementControllerController;

        /// <summary> Вызывается при смене выбранного предмета на панели быстрого доступа. </summary>
        /// <param name="previous"> Предыдущий выбранный предмет для размещения. </param>
        /// <param name="current"> Новый выбранный предмет для размещения. </param>
        protected override void OnSelectedChanged(PlaceableItem previous, PlaceableItem current)
        {
            if (WorldTime.IsPaused)
            {
                _placementController.ExitCurrentMode();
                return;
            }

            if (current)
                _placementController.EnterPlacementMode(PlacementModeType.Place, current.Prefab);
            else
                _placementController.ExitCurrentMode();
        }
    }
}