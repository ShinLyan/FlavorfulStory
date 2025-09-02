using System.Collections.Generic;
using System.Linq;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Провайдер инвентарей. Хранит все инвентари в списке. </summary>
    public class InventoryProvider : IInventoryProvider
    {
        /// <summary> Все инвентари в игре. </summary>
        private readonly Dictionary<InventoryType, List<Inventory>> _inventories = new();

        /// <summary> Зарегистрировать новый инвентарь в системе. </summary>
        /// <param name="inventory"> Инвентарь, который нужно зарегистрировать. </param>
        public void Register(Inventory inventory)
        {
            if (!_inventories.TryGetValue(inventory.Type, out var list))
            {
                list = new List<Inventory>();
                _inventories[inventory.Type] = list;
            }

            list.Add(inventory);
        }

        /// <summary> Отвязать инвентарь из системы. </summary>
        /// <param name="inventory"> Инвентарь, который нужно отвязать. </param>
        public void Unregister(Inventory inventory)
        {
            if (!_inventories.TryGetValue(inventory.Type, out var list)) return;

            list.Remove(inventory);
        }

        /// <summary> Получить все инвентари указанного типа. </summary>
        /// <param name="type"> Тип инвентаря (например, игрок, прилавок, сундук). </param>
        /// <returns> Коллекция инвентарей указанного типа. </returns>
        public IEnumerable<Inventory> GetAllOfType(InventoryType type) =>
            _inventories.TryGetValue(type, out var list) ? list : Enumerable.Empty<Inventory>();

        /// <summary> Получить инвентарь игрока. </summary>
        /// <returns> Инвентарь, принадлежащий игроку. </returns>
        public Inventory GetPlayerInventory() =>
            _inventories.TryGetValue(InventoryType.Player, out var list) ? list.FirstOrDefault() : null;
    }
}