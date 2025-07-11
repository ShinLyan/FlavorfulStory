namespace FlavorfulStory.Crafting
{
    /// <summary> Результат попытки выполнить крафт. </summary>
    public enum CraftingResult
    {
        /// <summary> Крафт выполнен успешно. </summary>
        Success,
        /// <summary> Недостаточно ресурсов для выполнения крафта. </summary>
        NotEnoughResources,
        /// <summary> Недостаточно места в инвентаре для результата. </summary>
        NotEnoughSpace,
    }
}