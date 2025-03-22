using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Core
{
    /// <summary> Создает объект-префаб только один раз и сохраняет его между сценами. </summary>
    /// <remarks> Поместите в инспекторе префаб, содержащий объекты Singleton,
    /// который будет сохраняться между сценами. </remarks>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        /// <summary> Префаб, создаваемый один раз и сохраняемый между сценами. </summary>
        [Tooltip("Префаб, который создается только один раз и сохраняется между сценами."), SerializeField]
        private GameObject _persistentObjectsPrefab;

        /// <summary> Был ли префаб уже создан? </summary>
        private bool _hasSpawned;

        /// <summary> Проверка и создание объекта при загрузке сцены. </summary>
        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObject();
            InputWrapper.Initialize();
            InputWrapper.UnblockAllInput();
            ItemDatabase.Initialize();
        }

        /// <summary> Создает постоянный объект и устанавливает его сохранение между сценами. </summary>
        private void SpawnPersistentObject()
        {
            var persistentObject = Instantiate(_persistentObjectsPrefab);
            DontDestroyOnLoad(persistentObject);

            _hasSpawned = true;
        }
    }
}