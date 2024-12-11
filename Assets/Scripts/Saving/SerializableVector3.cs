using UnityEngine;

namespace FlavorfulStory.Saving
{
    /// <summary> Сериализуемый Unity Vector3.</summary>
    /// <remarks> Стандартный Unity Vector3 нельзя сериализовать. Для сериализации сделана эта надстройка.</remarks>
    [System.Serializable]
    public class SerializableVector3
    {
        /// <summary> Координата вектора.</summary>
        private float x, y, z;

        /// <summary> Конструктор, который по стандартному Vector3 создает сериализуемый вектор.</summary>
        /// <param name="vector"> Стандартный Unity Vector3</param>
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        /// <summary> Приведение сериализуемого Vector3 к стандартному из Unity.</summary>
        /// <returns> Возвращает Unity Vector3.</returns>
        public Vector3 ToVector() => new(x, y, z);
    }
}