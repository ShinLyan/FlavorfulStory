namespace FlavorfulStory.Saving
{
    /// <summary> Интерфейс для сервисов, поддерживающих сохранение состояния. </summary>
    /// <remarks> Используется системой сохранений для регистрации Zenject-сервисов. </remarks>
    public interface ISaveableService : ISaveable
    {
        /// <summary> Уникальный идентификатор сервиса для сохранения. </summary>
        string UniqueIdentifier => GetType().FullName;
    }
}