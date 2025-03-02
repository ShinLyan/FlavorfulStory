using System.Collections.Generic;
using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    /// <summary> Префабы версий одного объекта. </summary>
    /// <remarks> Версия по умолчанию(первый ребенок в иерархии) добавляется автоматически. </remarks>
    [SerializeField] private GameObject[] _objectPrefabs;

    private List<GameObject> _spawnedObjects;

    public int GetObjectsCount() => _objectPrefabs.Length + 1;
    
    public void Initialize()
    {
        if (_spawnedObjects != null) return;
        
        _spawnedObjects = new List<GameObject>(_objectPrefabs.Length + 1) { transform.GetChild(0).gameObject };
        foreach (var prefab in _objectPrefabs)
        {
            var spawnedObject = Instantiate(prefab, transform, false);
            spawnedObject.SetActive(false);
            _spawnedObjects.Add(spawnedObject);
        }
    }

    public void SwitchTo(int index)
    {
        for (int i = 0; i < _spawnedObjects.Count; i++)
            _spawnedObjects[i].SetActive(i == index);
    }
}