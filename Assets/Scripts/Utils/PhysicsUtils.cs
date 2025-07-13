using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Утилита для работы с физикой и коллизиями. </summary>
    public static class PhysicsUtils
    {
        /// <summary> Радиус сферы для SphereCast. </summary>
        private const float SphereCastRadius = 0.1f;
        
        /// <summary> Максимальное количество хитов в нон-аллок версии. </summary>
        private const int MaxHits = 32;

        /// <summary> Буфер под результаты SphereCastAllNonAlloc. </summary>
        private static readonly RaycastHit[] _nonAllocHits = new RaycastHit[MaxHits];
        
        /// <summary> Выполняет SphereCast и возвращает все попадания, отсортированные по расстоянию. </summary>
        /// <param name="ray"> Луч, от которого производится проверка. </param>
        /// <returns> Массив попаданий, отсортированный по расстоянию. </returns>
        public static RaycastHit[] SphereCastAllSorted(Ray ray)
        {
            var hits = Physics.SphereCastAll(ray, SphereCastRadius);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            return hits;
        }
        
        /// <summary> Нон-аллок версия SphereCast. Возвращает количество попаданий, отсортированных по расстоянию. </summary>
        /// <param name="ray"> Луч для проверки. </param>
        /// <param name="results"> Выходной буфер с попаданиями. </param>
        /// <returns> Количество попаданий. </returns>
        public static int SphereCastAllSortedNonAlloc(Ray ray, out RaycastHit[] results)
        {
            int count = Physics.SphereCastNonAlloc(ray, SphereCastRadius, _nonAllocHits);
            Array.Sort(_nonAllocHits, 0, count, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));
            results = _nonAllocHits;
            return count;
        }
    }
}