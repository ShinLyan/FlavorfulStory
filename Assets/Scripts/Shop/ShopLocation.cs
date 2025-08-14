using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Shop
{
    /// <summary> Локация магазина с функциональностью управления полками, мебелью и кассой. </summary>
    public class ShopLocation : Location
    {
        /// <summary> Касса магазина для обслуживания покупателей. </summary>
        [field: SerializeField] public CashRegister CashRegister { get; private set; }

        /// <summary> Массив полок в магазине. </summary>
        [SerializeField] private Showcase[] _showcases;

        /// <summary> Массив мебели в магазине. </summary>
        [SerializeField] private Furniture[] _furnitures;

        /// <summary> Минимальное расстояние от объектов магазина при генерации случайных точек. </summary>
        private const float MinDistance = 3f;

        /// <summary> Возвращает случайную доступную мебель. </summary>
        /// <returns> Доступная мебель или null, если все мебель занята. </returns>
        public Furniture GetAvailableFurniture() => GetRandomAvailableObject(_furnitures);

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
        private static T GetRandomAvailableObject<T>(T[] objects) where T : ShopObject
        {
            var availableObjects = GetAvailableObjects(objects);

            if (availableObjects.Length == 0) return null;

            return availableObjects[Random.Range(0, availableObjects.Length)];
        }

        /// <summary> Проверяет наличие доступных прилавков с товарами. </summary>
        /// <returns> True, если есть прилавки с товарами, иначе False. </returns>
        public bool HasAvailableShowcaseWithItems()
        {
            var availableShowcases = GetAvailableObjects(_showcases);

            foreach (var showcase in availableShowcases)
                for (int i = 0; i < showcase.Inventory.InventorySize; i++)
                    if (showcase.Inventory.GetItemInSlot(i))
                        return true;

            return false;
        }

        /// <summary> Получает случайный прилавок с товарами. </summary>
        /// <returns> Случайный прилавок с товарами или null, если таких нет. </returns>
        public Showcase GetRandomAvailableShowcaseWithItems()
        {
            var availableShowcases = GetAvailableObjects(_showcases).Where(showcase =>
            {
                if (!showcase.Inventory) return false;

                for (int i = 0; i < showcase.Inventory.InventorySize; i++)
                    if (showcase.Inventory.GetItemInSlot(i))
                        return true;
                return false;
            }).ToArray();

            return availableShowcases.Length == 0
                ? null
                : availableShowcases[Random.Range(0, availableShowcases.Length)];
        }

        /// <summary> Проверяет, занята ли вся мебель в магазине. </summary>
        /// <returns> True, если вся мебель занята, иначе false. </returns>
        public bool AreAllFurnitureOccupied() => GetAvailableObjects(_furnitures).Length == 0;

        /// <summary> Генерирует случайную точку на навигационной сетке с учетом минимального
        /// расстояния от объектов магазина. </summary>
        /// <param name="maxAttempts"> Максимальное количество попыток генерации точки. </param>
        /// <returns> Случайная позиция на навигационной сетке или Vector3.zero. </returns>
        public override NpcDestinationPoint GetRandomPointOnNavMesh(int maxAttempts = 20)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                var pointOnNavMesh = base.GetRandomPointOnNavMesh(maxAttempts);
                if (pointOnNavMesh.Position == Vector3.zero) continue;

                bool isValidPosition = _showcases.All(showcase =>
                    !(Vector3.Distance(pointOnNavMesh.Position, showcase.transform.position) < MinDistance));

                if (!isValidPosition) continue;

                if (_furnitures.All(furniture =>
                        Vector3.Distance(pointOnNavMesh.Position, furniture.transform.position) < MinDistance))
                    isValidPosition = false;

                if (isValidPosition)
                    return new NpcDestinationPoint(pointOnNavMesh.Position,
                        Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }

            return new NpcDestinationPoint();
        }
    }
}