namespace FlavorfulStory.SceneManagement
{
    /// <summary> Список всех сцен игры. </summary>
    /// <remarks> Необходимо указывать точное название сцены. 
    /// В дальнейшем от этих названий будут загружаться сцены. </remarks>
    public enum SceneType
    {
        /// <summary> Главное меню. </summary>
        MainMenu,

        /// <summary> Каменный остров. </summary>
        RockyIsland,

        /// <summary> Ресторан. </summary>
        Restaurant
    }
}