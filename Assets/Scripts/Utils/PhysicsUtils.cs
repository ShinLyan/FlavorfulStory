using System;
using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Утилита для работы с физикой и коллизиями. </summary>
    public static class PhysicsUtils
    {
        /// <summary> Радиус сферы для SphereCast. </summary>
        private const float SphereCastRadius = 0.1f;

        /// <summary> Выполняет SphereCast и возвращает все попадания, отсортированные по расстоянию. </summary>
        /// <param name="ray"> Луч, от которого производится проверка. </param>
        /// <returns> Массив попаданий, отсортированный по расстоянию. </returns>
        public static RaycastHit[] SphereCastAllSorted(Ray ray)
        {
            var hits = Physics.SphereCastAll(ray, SphereCastRadius);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            return hits;
        }
    }
}