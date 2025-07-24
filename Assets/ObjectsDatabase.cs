using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Objects Database")]
    public class ObjectsDatabase : ScriptableObject
    {
        [SerializeField] private List<PlaceableObject> _placeables;

        public IEnumerable<PlaceableObject> Placeables => _placeables;

        /// <summary> Получить объект по индексу. </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PlaceableObject GetByIndex(int index) => _placeables[index];

        /// <summary> Количество объектов. </summary>
        public int Count => _placeables.Count;

        /// <summary> Найти индекс объекта. </summary>
        /// <param name="placeable"></param>
        /// <returns></returns>
        public int IndexOf(PlaceableObject placeable) => _placeables.IndexOf(placeable);
    }
}