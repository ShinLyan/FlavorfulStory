using System.Collections.Generic;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Интерфейс провайдера для получения инвентарей по типу и доступа к инвентарю игрока. </summary>
    public interface IInventoryProvider
    {
        /// <summary> Зарегистрировать новый инвентарь в системе. </summary>
        /// <param name="inventory"> Инвентарь, который нужно зарегистрировать. </param>
        void Register(Inventory inventory);

        /// <summary> Отвязать инвентарь из системы. </summary>
        /// <param name="inventory"> Инвентарь, который нужно отвязать. </param>
        void Unregister(Inventory inventory);

        /// <summary> Получить все инвентари указанного типа. </summary>
        /// <param name="type"> Тип инвентаря (например, игрок, прилавок, сундук). </param>
        /// <returns> Коллекция инвентарей указанного типа. </returns>
        IEnumerable<Inventory> GetAll(InventoryType type);

        /// <summary> Получить инвентарь игрока. </summary>
        /// <returns> Инвентарь, принадлежащий игроку. </returns>
        Inventory GetPlayerInventory();
    }
}