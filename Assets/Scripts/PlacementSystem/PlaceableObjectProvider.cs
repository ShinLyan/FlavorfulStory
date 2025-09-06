using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Провайдер для управления размещаемыми объектами. </summary>
    public class PlaceableObjectProvider : IPlaceableObjectProvider
    {
        /// <summary> Список всех размещённых объектов. </summary>
        private readonly List<PlaceableObject> _placeables;

        /// <summary> Все текущие размещённые объекты. </summary>
        public IReadOnlyCollection<PlaceableObject> All => _placeables;

        /// <summary> Инициализировать провайдер. </summary>
        /// <param name="placeables"> Список всех размещённых объектов на сцене. </param>
        public PlaceableObjectProvider(List<PlaceableObject> placeables) => _placeables = placeables;

        /// <summary> Зарегистрировать объект. </summary>
        /// <param name="placeableObject"> Объект для регистрации. </param>
        public void Register(PlaceableObject placeableObject)
        {
            if (placeableObject && !_placeables.Contains(placeableObject)) _placeables.Add(placeableObject);
        }

        /// <summary> Отменить регистрацию объекта. </summary>
        /// <param name="placeableObject"> Объект для отмены регистрации. </param>
        public void Unregister(PlaceableObject placeableObject)
        {
            if (_placeables.Contains(placeableObject)) _placeables.Remove(placeableObject);
        }

        /// <summary> Получить все объекты заданного типа компонента. </summary>
        /// <typeparam name="T"> Тип компонента для поиска. </typeparam>
        /// <returns> Перечисление найденных объектов. </returns>
        public IEnumerable<T> GetObjectsOfType<T>() where T : Component =>
            _placeables.Select(placeableObject => placeableObject.GetComponent<T>()).Where(component => component);
    }
}