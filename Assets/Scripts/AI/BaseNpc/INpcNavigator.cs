namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Интерфейс для навигации NPC персонажей. </summary>
    public interface INpcNavigator
    {
        /// <summary> Обновляет состояние навигации NPC. </summary>
        void Update();

        /// <summary> Останавливает навигацию NPC. </summary>
        /// <param name="warpToSpawn"> Если true, телепортирует NPC на точку спавна. </param>
        void Stop(bool warpToSpawn = false);
    }
}