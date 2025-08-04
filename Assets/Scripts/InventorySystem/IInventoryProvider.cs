using System.Collections.Generic;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Интерфейс для получения инвентарей по типу и доступа к инвентарю игрока. </summary>
    public interface IInventoryProvider
    {
        /// <summary> Зарегистрировать инвентарь. </summary>
        void Register(Inventory inventory);

        /// <summary> Отвязать инвентарь. </summary>
        void Unregister(Inventory inventory);
        
        /// <summary> Получить все инвентари указанного типа. </summary>
        IEnumerable<Inventory> GetAll(InventoryType type);
        
        /// <summary> Получить инвентарь игрока. </summary>
        Inventory GetPlayerInventory();
    }
}