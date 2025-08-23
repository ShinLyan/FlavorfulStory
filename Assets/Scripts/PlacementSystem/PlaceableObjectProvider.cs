using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Провайдер для управления размещаемыми объектами. </summary>
    public class PlaceableObjectProvider
    {
        /// <summary> Список всех размещаемых объектов. </summary>
        private readonly List<PlaceableObject> _objects = new();

        /// <summary> Инициализирует провайдер и находит все существующие объекты. </summary>
        public PlaceableObjectProvider()
        {
            var existingObjects =
                Object.FindObjectsByType<PlaceableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var placeableObject in existingObjects) Register(placeableObject);
        }

        /// <summary> Регистрирует новый предмет в реестре. </summary>
        /// <param name="placeableObject"> Объект для регистрации. </param>
        public void Register(PlaceableObject placeableObject)
        {
            if (placeableObject && !_objects.Contains(placeableObject)) _objects.Add(placeableObject);
        }

        /// <summary> Удаляет предмет из реестра. </summary>
        /// <param name="placeableObject"> Объект для удаления. </param>
        public void Unregister(PlaceableObject placeableObject) => _objects.Remove(placeableObject);

        /// <summary> Возвращает все объекты указанного типа. </summary>
        /// <typeparam name="T"> Тип компонента для поиска. </typeparam>
        /// <returns> Перечисление найденных объектов. </returns>
        public IEnumerable<T> GetObjectsOfType<T>() where T : Component =>
            _objects.Select(placeableObject => placeableObject.GetComponent<T>()).Where(component => component);
    }
}