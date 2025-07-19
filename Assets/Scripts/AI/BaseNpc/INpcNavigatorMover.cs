namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Интерфейс для перемещения NPC к заданной цели. </summary>
    /// <typeparam name="T"> Тип целевого объекта для перемещения. </typeparam>
    public interface INpcNavigatorMover<in T>
    {
        /// <summary> Перемещает NPC к указанной цели. </summary>
        /// <param name="target"> Целевой объект для перемещения. </param>
        void MoveTo(T target);
    }
}