using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _placedGameObjects = new();

        public int PlaceObject(GameObject prefab, Vector3 position)
        {
            var newObject = Instantiate(prefab, transform);
            newObject.transform.position = position;
            _placedGameObjects.Add(newObject);

            return _placedGameObjects.Count - 1;
        }

        public void RemoveObjectAt(int gameObjectIndex)
        {
            if (_placedGameObjects.Count <= gameObjectIndex) return;

            Destroy(_placedGameObjects[gameObjectIndex]);
            _placedGameObjects[gameObjectIndex] = null;
        }
    }
}