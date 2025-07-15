using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.AI
{
    /// <summary>
    /// Компонент для спавна неинтерактивных NPC в игровом мире.
    /// Управляет созданием случайных NPC из списка префабов в заданных точках спавна.
    /// </summary>
    public class NpcSpawner : MonoBehaviour
    {
        /// <summary> Массив префабов NPC для случайного выбора при спавне. </summary>
        [SerializeField] private NonInteractableNpc.NonInteractableNpc[] _npcPrefabs;

        /// <summary> Точки спавна, в которых могут появляться NPC. </summary>
        [SerializeField] private NpcSpawnPoint[] _spawnPoints;

        /// <summary> Интервал между спавнами NPC в секундах. </summary>
        [SerializeField] private float _spawnInterval = 10f;

        /// <summary> Максимальное количество одновременно активных NPC. </summary>
        [SerializeField] private int _maxTotalCharacters = 10;

        /// <summary> Список активных NPC на сцене. </summary>
        private readonly List<NonInteractableNpc.NonInteractableNpc> _activeCharacters = new();

        /// <summary> Флаг, указывающий, активен ли процесс спавна. </summary>
        private readonly bool _isSpawning = true;

        /// <summary> Контейнер для инъекции зависимостей. </summary>
        [Inject] private DiContainer _diContainer;

        /// <summary> Менеджер локаций для управления местоположениями. </summary>
        [Inject] private LocationManager _locationManager;

        /// <summary> Инициализация компонента. </summary>
        private void Start()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned!", this);
                return;
            }

            if (_npcPrefabs == null || _npcPrefabs.Length == 0)
            {
                Debug.LogError("No NPC prefabs assigned!", this);
                return;
            }

            StartCoroutine(SpawnRoutine());
        }

        /// <summary> Корутина для периодического спавна NPC. </summary>
        /// <returns> Корутина для периодического спавна. </returns>
        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                yield return new WaitForSeconds(_spawnInterval);

                if (CanSpawn()) SpawnNpc();
            }
        }

        /// <summary>  Проверяет, можно ли создать нового NPC. </summary>
        /// <returns> True, если количество активных NPC меньше максимального. </returns>
        private bool CanSpawn() => _activeCharacters.Count < _maxTotalCharacters;

        /// <summary> Создает нового NPC в случайной точке спавна, используя случайный префаб. </summary>
        private void SpawnNpc()
        {
            NpcSpawnPoint npcSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            NonInteractableNpc.NonInteractableNpc randomPrefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Length)];

            var npc = _diContainer.InstantiatePrefabForComponent<NonInteractableNpc.NonInteractableNpc>(
                randomPrefab,
                npcSpawnPoint.GetRandomPosition(),
                Quaternion.identity,
                null);

            _activeCharacters.Add(npc);

            StartCoroutine(SetDestinationAfterInit(npc));
        }

        /// <summary> Устанавливает пункт назначения для созданного NPC после его инициализации. </summary>
        /// <param name="npc"> NPC, для которого устанавливается пункт назначения. </param>
        /// <returns> Корутина для установки пункта назначения. </returns>
        private IEnumerator SetDestinationAfterInit(NonInteractableNpc.NonInteractableNpc npc)
        {
            yield return null;

            //TODO: переделать после удаления WarpGraph 
            var loc = _locationManager.GetLocationByName(LocationName.NewShop);
            var pos = loc.transform.position;

            npc.SetDestination(pos, LocationName.NewShop);
        }

        /// <summary>  Получает случайную точку деспавна, исключая указанную точку. </summary>
        /// <param name="excludePoint"> Точка, которую следует исключить из выбора. </param>
        /// <returns> Случайная точка деспавна. </returns>
        private NpcSpawnPoint GetRandomDespawnPoint(NpcSpawnPoint excludePoint)
        {
            if (_spawnPoints.Length == 1) return _spawnPoints[0]; // Если только одна точка, возвращаем её

            NpcSpawnPoint result;
            do
            {
                result = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            } while (result == excludePoint && _spawnPoints.Length > 1);

            return result;
        }
    }
}