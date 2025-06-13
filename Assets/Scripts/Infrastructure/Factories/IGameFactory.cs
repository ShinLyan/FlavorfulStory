using UnityEngine;

namespace FlavorfulStory.Infrastructure.Factories
{
    /// <summary> Интерфейс фабрики для создания и удаления компонентов. </summary>
    /// <typeparam name="T"> Тип компонента. </typeparam>
    public interface IGameFactory<T> where T : Component
    {
        /// <summary> Создать экземпляр компонента. </summary>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр компонента. </returns>
        T Create(Transform parent = null);

        /// <summary> Удалить ранее созданный экземпляр компонента. </summary>
        /// <param name="obj"> Объект для удаления. </param>
        void Despawn(T obj);
    }
}