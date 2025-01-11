namespace FlavorfulStory.Saving
{
    /// <summary> Интерфейс, позволяющий сохранять и загружать данные объекта. </summary>
    public interface ISaveable
    {
        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state);
    }
}