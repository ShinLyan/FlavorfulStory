using System;
using Zenject;

namespace FlavorfulStory.Saving
{
    /// <summary> Связывает Zenject-сервис, поддерживающий сохранение, с реестром сохранений. </summary>
    /// <typeparam name="T"> Тип сохраняемого сервиса. </typeparam>
    public class SaveableServiceBinder<T> : IInitializable, IDisposable where T : ISaveableService
    {
        /// <summary> Сервис, реализующий <see cref="ISaveableService"/>. </summary>
        private readonly T _service;

        /// <summary> Создаёт новый биндер для сохраняемого сервиса. </summary>
        /// <param name="service"> Сервис для регистрации. </param>
        public SaveableServiceBinder(T service) => _service = service;

        /// <summary> Регистрирует сервис в <see cref="SaveServiceRegistry"/> при инициализации. </summary>
        public void Initialize() => SaveServiceRegistry.Register(_service);

        /// <summary> Удаляет сервис из <see cref="SaveServiceRegistry"/> при уничтожении. </summary>
        public void Dispose() => SaveServiceRegistry.Unregister(_service);
    }
}