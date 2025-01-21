using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.Core
{
    /// <summary> Класс создает префаб только один раз для всех сцен. </summary>
    /// <remarks> Поместите в инспекторе префаб, который будет присутствовать в каждой сцене и содержать все игровые объекты, являющиеся Singleton.
    /// Класс создаст префаб только один раз и настроит его на сохранение между сценами. </remarks>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        /// <summary> Префаб, который создается только один раз и сохраняется между сценами. </summary>
        [Tooltip("Префаб, который создается только один раз и сохраняется между сценами.")]
        [SerializeField] private GameObject _persistentObjectsPrefab;

        /// <summary> Был ли заспавнен?</summary>
        private bool _hasSpawned;

        /// <summary> Создание постоянного объекта. </summary>
        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObject();
            _hasSpawned = true;
            
            InputWrapper.Initialize();
        }

        /// <summary> Создание постоянного объекта. </summary>
        private void SpawnPersistentObject()
        {
            var persistentObject = Instantiate(_persistentObjectsPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
