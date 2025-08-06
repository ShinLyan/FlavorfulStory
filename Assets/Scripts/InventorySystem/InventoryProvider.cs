using System.Collections.Generic;
using System.Linq;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Провайдер инвентарей. Хранит все инвентари в списке. </summary>
    public class InventoryProvider : IInventoryProvider
    {
        /// <summary> Список всех инвентарей в игре. </summary>
        private readonly List<Inventory> _inventories;

        /// <summary> Создать провайдер с переданным списком инвентарей. </summary>
        public InventoryProvider(List<Inventory> inventories) => _inventories = inventories;

        /// <summary> Зарегистрировать новый инвентарь в системе. </summary>
        /// <param name="inventory"> Инвентарь, который нужно зарегистрировать. </param>
        public void Register(Inventory inventory)
        {
            if (!_inventories.Contains(inventory)) _inventories.Add(inventory);
        }

        /// <summary> Отвязать инвентарь из системы. </summary>
        /// <param name="inventory"> Инвентарь, который нужно отвязать. </param>
        public void Unregister(Inventory inventory)
        {
            if (_inventories.Contains(inventory)) _inventories.Remove(inventory);
        }

        /// <summary> Получить все инвентари указанного типа. </summary>
        /// <param name="type"> Тип инвентаря (например, игрок, прилавок, сундук). </param>
        /// <returns> Коллекция инвентарей указанного типа. </returns>
        public IEnumerable<Inventory> GetAll(InventoryType type) =>
            _inventories.Where(inventory => inventory.Type == type);

        /// <summary> Получить инвентарь игрока. </summary>
        /// <returns> Инвентарь, принадлежащий игроку. </returns>
        public Inventory GetPlayerInventory() =>
            _inventories.FirstOrDefault(inventory => inventory.Type == InventoryType.Player);
    }
}