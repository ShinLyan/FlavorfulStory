using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class PlaceableObjectProvider
    {
        private readonly List<PlaceableObject> _objects = new();

        /// <summary>
        /// Регистрирует новый предмет в реестре.
        /// </summary>
        public void Register(PlaceableObject obj)
        {
            if (obj && !_objects.Contains(obj)) _objects.Add(obj);
        }

        /// <summary>
        /// Удаляет предмет из реестра.
        /// </summary>
        public void Unregister(PlaceableObject obj) { _objects.Remove(obj); }

        /// <summary>
        /// Возвращает все объекты, содержащие компонент T.
        /// </summary>
        public IEnumerable<T> GetObjectsOfType<T>() where T : Component
        {
            return _objects
                .Select(o => o.GetComponent<T>())
                .Where(c => c);
        }
    }
}