using FlavorfulStory.InventorySystem.UI.Dragging;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Целевой контейнер для предметов, выбрасываемых из инвентаря. </summary>
    public class ItemDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        /// <summary> Провайдер позиции игрока. </summary>
        private IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Выбрасыватель предметов. </summary>
        private IItemDropService _itemDropService;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        /// <param name="itemDropService"> Выбрасыватель предметов. </param>
        [Inject]
        private void Construct(IPlayerPositionProvider playerPositionProvider, IItemDropService itemDropService)
        {
            _playerPositionProvider = playerPositionProvider;
            _itemDropService = itemDropService;
        }

        /// <summary> Получить максимально допустимое количество элементов,
        /// которые можно добавить в это место назначения. </summary>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Максимально допустимое количество элементов. </returns>
        /// <remarks> Если ограничений нет, метод должен возвращать значение <c>Int.MaxValue</c>. </remarks>
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => int.MaxValue;

        /// <summary> Добавить элементы в это место назначения с обновлением UI и данных. </summary>
        /// <param name="item"> Тип добавляемого элемента. </param>
        /// <param name="number"> Количество добавляемых элементов. </param>
        public void AddItems(InventoryItem item, int number) =>
            _itemDropService.Drop(new ItemStack(item, number), _playerPositionProvider.GetPlayerPosition());
    }
}