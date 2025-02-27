using System.Collections.Generic;
using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectPrefabs;

    private List<GameObject> _spawnedObjects;

    private void Awake()
    {
        _spawnedObjects = new List<GameObject>(_objectPrefabs.Length + 1) { transform.GetChild(0).gameObject };
        foreach (var prefab in _objectPrefabs)
        {
            var spawnedObject = Instantiate(prefab, transform, false);
            spawnedObject.SetActive(false);
            _spawnedObjects.Add(spawnedObject);
        }
    }

    public void SwitchToGameobject(int index)
    {
        for (int i = 0; i < _spawnedObjects.Count; i++)
            _spawnedObjects[i].SetActive(i == index);
    }
}