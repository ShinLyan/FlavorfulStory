using UnityEngine;

namespace FlavorfulStory.Infrastructure.Factories
{
    /// <summary> Интерфейс фабрики для создания и удаления префабов. </summary>
    /// <typeparam name="T"> Тип компонента. </typeparam>
    public interface IPrefabFactory<T> where T : Component
    {
        /// <summary> Создать экземпляр префаба. </summary>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        T Create(Transform parent = null);

        /// <summary> Создать экземпляр префаба. </summary>
        /// <param name="prefab"> Префаб, который нужно создать. </param>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        T Create(T prefab, Transform parent = null);

        /// <summary> Удалить ранее созданный экземпляр префаба. </summary>
        /// <param name="prefab"> Префаб для удаления. </param>
        void Despawn(T prefab);
    }
}