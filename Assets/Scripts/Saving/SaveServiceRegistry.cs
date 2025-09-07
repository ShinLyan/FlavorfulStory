using System.Collections.Generic;

namespace FlavorfulStory.Saving
{
    /// <summary> Реестр сохраняемых сервисов, участвующих в системе сохранений. </summary>
    public static class SaveServiceRegistry
    {
        /// <summary> Хранилище всех зарегистрированных сервисов по их уникальному идентификатору. </summary>
        private static readonly Dictionary<string, ISaveableService> _services = new();

        /// <summary> Регистрирует сохраняемый сервис. </summary>
        /// <param name="service"> Сервис, который нужно зарегистрировать. </param>
        public static void Register(ISaveableService service) => _services[service.UniqueIdentifier] = service;

        /// <summary> Удаляет сервис из реестра. </summary>
        /// <param name="service"> Сервис, который нужно удалить. </param>
        public static void Unregister(ISaveableService service) => _services.Remove(service.UniqueIdentifier);

        /// <summary> Все зарегистрированные сохраняемые сервисы. </summary>
        public static IReadOnlyCollection<ISaveableService> All => _services.Values;
    }
}