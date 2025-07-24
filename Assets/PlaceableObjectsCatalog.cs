using System.Collections.Generic;
using FlavorfulStory.PlacementSystem;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Каталог объектов, которые можно размещать в игровом мире. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Placeable Objects Catalog")]
    public class PlaceableObjectsCatalog : ScriptableObject
    {
        /// <summary> Список размещаемых объектов, определённых в редакторе. </summary>
        [SerializeField] private List<PlaceableObject> _placeables;

        /// <summary> Получить объект из каталога по индексу. </summary>
        /// <param name="index"> Индекс объекта в списке. </param>
        /// <returns> Объект, который можно разместить. </returns>
        public PlaceableObject GetByIndex(int index) => _placeables[index];
    }
}