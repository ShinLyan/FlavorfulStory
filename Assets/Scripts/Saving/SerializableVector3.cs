using System;
using UnityEngine;

namespace FlavorfulStory.Saving
{
    /// <summary> Сериализуемый Unity Vector3. </summary>
    /// <remarks> Стандартный Unity Vector3 нельзя сериализовать. Для сериализации сделана эта надстройка. </remarks>
    [Serializable]
    public readonly struct SerializableVector3
    {
        /// <summary> X координата вектора. </summary>
        private readonly float x;

        /// <summary> Y координата вектора. </summary>
        private readonly float y;

        /// <summary> Z координата вектора. </summary>
        private readonly float z;

        /// <summary> Конструктор, который по Unity.Vector3 значению создает сериализуемый вектор. </summary>
        /// <param name="vector"> Unity.Vector3 значение. </param>
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        /// <summary> Приведение сериализуемого Vector3 к Vector3 из Unity. </summary>
        /// <returns> Возвращает Unity.Vector3 значение. </returns>
        public Vector3 ToVector() => new(x, y, z);
    }
}