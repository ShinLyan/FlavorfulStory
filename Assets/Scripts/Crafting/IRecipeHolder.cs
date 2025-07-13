namespace FlavorfulStory.Crafting
{
    /// <summary> Интерфейс для предоставления доступа к рецепту крафта. </summary>
    public interface IRecipeHolder
    {
        /// <summary> Получить рецепт крафта, связанный с этим объектом. </summary>
        CraftingRecipe GetRecipe();
    }
}