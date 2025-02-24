namespace FlavorfulStory.SceneManagement
{
    /// <summary> Перечисление, представляющее доступные игровые сцены. </summary>
    /// <remarks> Используется для определения загружаемых сцен. 
    /// В сценах могут быть различные игровые зоны. </remarks>
    public enum SceneType
    {
        /// <summary> Главное меню. </summary>
        MainMenu,

        /// <summary> Каменистый остров. </summary>
        RockyIsland,

        /// <summary> Ресторан. </summary>
        Restaurant,
        
        /// <summary> Главная сцена. </summary>
        MainScene,
    }
}