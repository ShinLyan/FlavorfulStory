using UnityEngine;

namespace FlavorfulStory.Utils.Factories
{
    /// <summary> Интерфейс фабрики для создания и удаления префабов. </summary>
    /// <typeparam name="T"> Тип компонента. </typeparam>
    public interface IPrefabFactory<T> where T : Component
    {
        /// <summary> Создать экземпляр префаба с параметрами. </summary>
        /// <param name="prefab"> Префаб для создания (если null — используется prefab по умолчанию). </param>
        /// <param name="position"> Позиция (если null — используется Vector3.zero). </param>
        /// <param name="rotation"> Поворот (если null — используется Quaternion.identity). </param>
        /// <param name="parentTransform"> Родительский трансформ (может быть null). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        T Create(T prefab = null, Vector3? position = null, Quaternion? rotation = null,
            Transform parentTransform = null);

        /// <summary> Удалить ранее созданный экземпляр префаба. </summary>
        /// <param name="prefab"> Префаб для удаления. </param>
        void Despawn(T prefab);
    }
}