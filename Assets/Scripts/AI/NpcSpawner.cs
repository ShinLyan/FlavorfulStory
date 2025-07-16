using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

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

        /// <summary> Трансформ, к которому будут привязаны созданные NPC. </summary>
        [SerializeField] private Transform _parentTransform;

        /// <summary> Список активных NPC на сцене. </summary>
        private readonly List<NonInteractableNpc.NonInteractableNpc> _activeCharacters = new();

        /// <summary> Флаг, указывающий, активен ли процесс спавна. </summary>
        private readonly bool _isSpawning = true;

        /// <summary> Контейнер для инъекции зависимостей. </summary>
        [Inject] private DiContainer _diContainer;

        /// <summary> Менеджер локаций для управления местоположениями. </summary>
        [Inject] private LocationManager _locationManager;

        private ObjectPool<NonInteractableNpc.NonInteractableNpc> _npcPool;

        private bool _isTimePaused;

        private void Awake()
        {
            WorldTime.OnDayEnded += _ => StartCoroutine(DespawnAllNpcCoroutine());
            WorldTime.OnTimePaused += () => _isTimePaused = true;
            WorldTime.OnTimeUnpaused += () => _isTimePaused = false;
        }


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

            _npcPool = new ObjectPool<NonInteractableNpc.NonInteractableNpc>(
                CreateNpc,
                npc => npc.gameObject.SetActive(true),
                npc => npc.gameObject.SetActive(false),
                actionOnDestroy: npc => Destroy(npc.gameObject),
                defaultCapacity: _maxTotalCharacters,
                maxSize: _maxTotalCharacters
            );

            StartCoroutine(SpawnRoutine());
        }

        private NonInteractableNpc.NonInteractableNpc CreateNpc()
        {
            var npcSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            var pos = npcSpawnPoint.GetRandomPosition();

            NonInteractableNpc.NonInteractableNpc prefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Length)];
            var npc = _diContainer.InstantiatePrefabForComponent<NonInteractableNpc.NonInteractableNpc>(
                prefab, pos, Quaternion.identity, _parentTransform
            );
            return npc;
        }

        private void SpawnNpcFromPool()
        {
            var npc = _npcPool.Get();

            NpcSpawnPoint npcSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            var pos = npcSpawnPoint.GetRandomPosition();
            npc.transform.position = pos;
            npc.transform.rotation = Quaternion.identity;

            _activeCharacters.Add(npc);

            npc.OnReachedDespawnPoint += () => DespawnNpc(npc);
            npc.SetDespawnPoint(GetRandomDespawnPoint(npcSpawnPoint).transform.position);
            StartCoroutine(SetDestinationAfterInit(npc));
        }


        /// <summary> Корутина для периодического спавна NPC. </summary>
        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                yield return new WaitForSeconds(_spawnInterval);

                if (CanSpawn()) SpawnNpcFromPool();
            }
        }

        /// <summary>  Проверяет, можно ли создать нового NPC. </summary>
        /// <returns> True, если количество активных NPC меньше максимального. </returns>
        private bool CanSpawn() => _activeCharacters.Count < _maxTotalCharacters && !_isTimePaused;


        /// <summary> Возвращает NPC в пул и удаляет из списка активных. </summary>
        private void DespawnNpc(NonInteractableNpc.NonInteractableNpc npc)
        {
            if (_activeCharacters.Contains(npc))
            {
                _activeCharacters.Remove(npc);
                _npcPool.Release(npc);
            }
        }

        private IEnumerator DespawnAllNpcCoroutine()
        {
            yield return null;

            foreach (var npc in _activeCharacters.ToArray()) DespawnNpc(npc);
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