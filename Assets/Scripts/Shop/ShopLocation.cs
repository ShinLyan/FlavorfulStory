using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.PlacementSystem;
using FlavorfulStory.SceneManagement;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Shop
{
    /// <summary> Локация магазина с функциональностью управления полками, мебелью и кассой. </summary>
    public class ShopLocation : Location
    {
        [field: SerializeField] public Transform EntryPoint { get; private set; }

        /// <summary> Касса магазина для обслуживания покупателей. </summary>
        [field: SerializeField]
        public CashRegister CashRegister { get; private set; }

        /// <summary> Массив полок в магазине. </summary>
        private List<Showcase> Showcases => _placeableObjectProvider.GetObjectsOfType<Showcase>().ToList();

        /// <summary> Массив мебели в магазине. </summary>
        private List<Furniture> Furnitures => _placeableObjectProvider.GetObjectsOfType<Furniture>().ToList();

        /// <summary> Поверхность NavMesh для данной локации. </summary> 
        [SerializeField] private NavMeshSurface _navMeshSurface;

        /// <summary> Провайдер для получения размещаемых объектов в локации магазина. </summary>
        private IPlaceableObjectProvider _placeableObjectProvider;

        /// <summary> Минимальное расстояние от объектов магазина при генерации случайных точек. </summary>
        private const float MinDistance = 3f;

        [Inject]
        private void Construct(IPlaceableObjectProvider placeableObjectProvider) =>
            _placeableObjectProvider = placeableObjectProvider;


        /// <summary> Возвращает случайную доступную мебель. </summary>
        /// <returns> Доступная мебель или null, если все мебель занята. </returns>
        public Furniture GetAvailableFurniture() => GetRandomAvailableObjectOfType<Furniture>();

        /// <summary> Получает массив доступных (незанятых) объектов из переданного массива. </summary>
        /// <typeparam name="T"> Тип объекта, производный от ShopObject. </typeparam>
        /// <param name="objects"> Массив объектов магазина для фильтрации. </param>
        /// <returns> Массив доступных объектов. </returns>
        private static List<T> GetAvailableObjects<T>(List<T> objects) where T : ShopObject
        {
            var availableObjects = new List<T>();
            foreach (var obj in objects)
                if (!obj.IsOccupied)
                    availableObjects.Add(obj);
            
            return availableObjects;
        }

        /// <summary> Получает случайный свободный объект из массива объектов магазина. </summary>
        /// <typeparam name="T"> Тип объекта, наследующийся от ShopObject. </typeparam>
        /// <returns> Случайный свободный объект типа T, или null, если все объекты заняты. </returns>
        private T GetRandomAvailableObjectOfType<T>() where T : ShopObject
        {
            var placedObjects = _placeableObjectProvider.GetObjectsOfType<T>().ToList();
            var availableObjects = GetAvailableObjects(placedObjects);
            return availableObjects.Count == 0 ? null : availableObjects[Random.Range(0, availableObjects.Count)];
        }

        /// <summary> Проверяет наличие доступных прилавков с товарами. </summary>
        /// <returns> True, если есть прилавки с товарами, иначе False. </returns>
        public bool HasAvailableShowcaseWithItems()
        {
            var availableShowcases = GetAvailableObjects(Showcases);

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
            var availableShowcases = GetAvailableObjects(Showcases).Where(showcase =>
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
        public bool HasAvailableFurniture() => GetAvailableObjects(Furnitures).Count > 0;

        /// <summary> Проверяет, находится ли позиция на допустимом расстоянии от мебели и витрин. </summary>
        private bool IsValidPosition(Vector3 position) =>
            Showcases.All(showcase => Vector3.Distance(position, showcase.transform.position) >= MinDistance) &&
            Furnitures.All(furniture => Vector3.Distance(position, furniture.transform.position) >= MinDistance) &&
            Vector3.Distance(position, CashRegister.transform.position) >= MinDistance;

        /// <summary> Пытается найти случайную точку на NavMesh в пределах указанного числа попыток. </summary>
        private static bool TryGetRandomPoint(Bounds bounds, out Vector3 result, int maxAttempts = 20)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                var randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.center.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (NavMesh.SamplePosition(randomPoint, out var hit, 2f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }

        /// <summary> Генерирует случайную точку на NavMesh с учётом минимального расстояния от объектов магазина. </summary>
        public NpcDestinationPoint? GetRandomPointOnNavMesh(int maxAttempts = 20)
        {
            if (!_navMeshSurface) return null;

            var bounds = new Bounds(_navMeshSurface.transform.position, _navMeshSurface.size);

            for (int i = 0; i < maxAttempts; i++)
            {
                if (!TryGetRandomPoint(bounds, out var point, 1)) continue;

                if (!IsValidPosition(point)) continue;

                var randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                return new NpcDestinationPoint(point, randomRotation);
            }

            return null;
        }
    }
}