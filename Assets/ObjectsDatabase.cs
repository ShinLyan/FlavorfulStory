using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Objects Database")]
    public class ObjectsDatabase : ScriptableObject
    {
        [SerializeField] private List<ObjectData> _objectsData;

        public List<ObjectData> ObjectsData => _objectsData;
    }

    [Serializable]
    public class ObjectData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}