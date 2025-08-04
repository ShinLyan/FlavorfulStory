namespace FlavorfulStory.InventorySystem
{
    /// <summary> Тип инвентаря — используется для определения назначения и поведения. </summary>
    public enum InventoryType
    {
        /// <summary> Инвентарь игрока. </summary>
        Player,
        /// <summary> Сундук — интерактивный инвентарь с обменом. </summary>
        Chest,
        /// <summary> Полка магазина — только отображение, без взаимодействия. </summary>
        ShopShelf,
    }
}