using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Shop
{
    /// <summary> Локация магазина с функциональностью управления полками, мебелью и кассой. </summary>
    public class ShopLocation : Location
    {
        /// <summary> Касса магазина для обслуживания покупателей. </summary>
        [field: SerializeField]
        public CashRegister CashDesk { get; private set; }

        /// <summary> Массив полок в магазине. </summary>
        [SerializeField] private Shelf[] _shelves;

        /// <summary> Массив мебели в магазине. </summary>
        [SerializeField] private Furniture[] _furnitures;

        /// <summary> Минимальное расстояние от объектов магазина при генерации случайных точек. </summary>
        private const float MinDistance = 3f;

        /// <summary> Возвращает случайную доступную мебель. </summary>
        /// <returns> Доступная мебель или null, если все мебель занята. </returns>
        public Furniture GetAvailableFurniture() => GetRandomObject(_furnitures);

        /// <summary> Возвращает случайную доступную полку. </summary>
        /// <returns> Доступная полка или null, если все полки заняты. </returns>
        public Shelf GetAvailableShelf() => GetRandomObject(_shelves);

        /// <summary> Получает массив доступных (незанятых) объектов из переданного массива. </summary>
        /// <typeparam name="T"> Тип объекта, производный от ShopObject. </typeparam>
        /// <param name="objects"> Массив объектов магазина для фильтрации. </param>
        /// <returns> Массив доступных объектов. </returns>
        private static T[] GetAvailableObjects<T>(T[] objects) where T : ShopObject
        {
            var availableObjects = new List<T>();
            foreach (var obj in objects)
                if (!obj.IsOccupied)
                    availableObjects.Add(obj);

            return availableObjects.ToArray();
        }

        /// <summary> Получает случайный свободный объект из массива объектов магазина. </summary>
        /// <typeparam name="T"> Тип объекта, наследующийся от ShopObject. </typeparam>
        /// <param name="objects"> Массив объектов для выбора. </param>
        /// <returns> Случайный свободный объект типа T, или null, если все объекты заняты. </returns>
        private static T GetRandomObject<T>(T[] objects) where T : ShopObject
        {
            var availableObjects = GetAvailableObjects(objects);

            if (availableObjects.Length == 0) return null;

            return availableObjects[Random.Range(0, availableObjects.Length)];
        }

        /// <summary> Проверяет, пусты ли все доступные полки. </summary>
        /// <returns> True, если все доступные полки пусты, иначе false. </returns>
        public bool AreAvailableShelvesEmpty()
        {
            // TODO: Реализация проверки, есть ли на полках товары
            return GetAvailableObjects(_shelves).Length == 0;
        }

        /// <summary> Проверяет, занята ли вся мебель в магазине. </summary>
        /// <returns> True, если вся мебель занята, иначе false. </returns>
        public bool AreAllFurnitureOccupied() => GetAvailableObjects(_furnitures).Length == 0;

        /// <summary> Генерирует случайную точку на навигационной сетке с учетом минимального
        /// расстояния от объектов магазина. </summary>
        /// <param name="maxAttempts"> Максимальное количество попыток генерации точки. </param>
        /// <returns> Случайная позиция на навигационной сетке или Vector3.zero. </returns>
        public override Vector3 GetRandomPointOnNavMesh(int maxAttempts = 20)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                var position = base.GetRandomPointOnNavMesh(maxAttempts);
                if (position == Vector3.zero) continue;

                bool isValidPosition = true;

                foreach (var shelf in _shelves)
                    if (Vector3.Distance(position, shelf.transform.position) < MinDistance)
                    {
                        isValidPosition = false;
                        break;
                    }

                if (!isValidPosition) continue;

                foreach (var furniture in _furnitures)
                    if (Vector3.Distance(position, furniture.transform.position) < MinDistance)
                    {
                        isValidPosition = false;
                        break;
                    }

                if (isValidPosition) return position;
            }

            return Vector3.zero;
        }
    }
}