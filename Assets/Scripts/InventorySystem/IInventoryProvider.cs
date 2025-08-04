using System.Collections.Generic;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Интерфейс для получения инвентарей по типу и доступа к инвентарю игрока. </summary>
    public interface IInventoryProvider
    {
        /// <summary> Получить все инвентари указанного типа. </summary>
        IEnumerable<Inventory> GetAll(InventoryType type);
        
        /// <summary> Получить инвентарь игрока. </summary>
        Inventory GetPlayerInventory();
    }
}