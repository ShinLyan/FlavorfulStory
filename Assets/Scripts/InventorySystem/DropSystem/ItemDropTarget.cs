using FlavorfulStory.InventorySystem.UI.Dragging;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Целевой контейнер для предметов, выбрасываемых из инвентаря. </summary>
    public class ItemDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        /// <summary> Коонтроллер игрока. </summary>
        private PlayerController _playerController;
        
        /// <summary> Выбрасыватель предметов. </summary>
        private IItemDropService _itemDropService;

        /// <summary> Внедренеие зависимостей Zenject. </summary>
        /// <param name="itemDropService"> Выбрасыватель предметов. </param>
        [Inject]
        private void Construct(PlayerController playerController, IItemDropService itemDropService)
        {
            _playerController = playerController;
            _itemDropService = itemDropService;
        } 

        /// <summary> Получить максимально допустимое количество элементов,
        /// которые можно добавить в это место назначения. </summary>
        /// <remarks> Если ограничений нет, метод должен возвращать значение <c>Int.MaxValue</c>. </remarks>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => int.MaxValue;

        //TODO: Сделать ревизию.
        //Перевел на IItemDropService - как-будто можно переписать DragItem, чтобы тут адекватно выбрасывать
        /// <summary> Добавить элементы в это место назначения с обновлением UI и данных. </summary>
        /// <param name="item"> Тип добавляемого элемента. </param>
        /// <param name="number"> Количество добавляемых элементов. </param>
        public void AddItems(InventoryItem item, int number)
        {
            _itemDropService.Drop(item, number, _playerController.transform.position); 
        }
    }
}