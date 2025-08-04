using System.Collections.Generic;
using System.Linq;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Реализация провайдера инвентарей. Хранит все инвентари в списке. </summary>
    public class InventoryProvider : IInventoryProvider
    {
        /// <summary> Список всех инвентарей в игре. </summary>
        private readonly List<Inventory> _inventories;

        /// <summary> Создать провайдер с переданным списком инвентарей. </summary>
        public InventoryProvider(List<Inventory> inventories) => _inventories = inventories;

        /// <summary> Получить все инвентари указанного типа. </summary>
        public IEnumerable<Inventory> GetAll(InventoryType type) =>
            _inventories.Where(i => i.Type == type);

        /// <summary> Получить инвентарь игрока (первый с типом Player). </summary> 
        public Inventory GetPlayerInventory() =>
            _inventories.FirstOrDefault(i => i.Type == InventoryType.Player);
    }
}