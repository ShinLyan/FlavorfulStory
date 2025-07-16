using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.EditorTools.Attributes;
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
        [SerializeField, SteppedRange(5f, 120f, 5f)]
        private float _spawnInterval = 30f;

        /// <summary> Время одного тика в секундах. </summary>
        private const int TickTime = 5;

        /// <summary> Интервал спавна, переведенный в тики. </summary>
        private int SpawnIntervalInTicks => (int)_spawnInterval / TickTime;

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

        /// <summary> Пул объектов для переиспользования NPC. </summary>
        private ObjectPool<NonInteractableNpc.NonInteractableNpc> _npcPool;

        /// <summary> Флаг, указывающий, приостановлено ли время в игре. </summary>
        private bool _isTimePaused;

        /// <summary> Счетчик тиков для отслеживания интервала спавна. </summary>
        private int _tickCounter;

        /// <summary> Инициализация подписок на события времени. </summary>
        private void Awake()
        {
            WorldTime.OnDayEnded += _ => StartCoroutine(DespawnAllNpcCoroutine());
            WorldTime.OnTimePaused += () => _isTimePaused = true;
            WorldTime.OnTimeUnpaused += () => _isTimePaused = false;
            WorldTime.OnTimeTick += OnTimeTickHandler;
        }

        /// <summary> Проверяет наличие необходимых данных и создает пул объектов для NPC. </summary>
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
        }

        /// <summary> Создает новый экземпляр NPC для пула объектов. </summary>
        /// <returns> Новый экземпляр NPC. </returns>
        private NonInteractableNpc.NonInteractableNpc CreateNpc()
        {
            NpcSpawnPoint npcSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            var pos = npcSpawnPoint.GetRandomPosition();

            NonInteractableNpc.NonInteractableNpc prefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Length)];
            var npc = _diContainer.InstantiatePrefabForComponent<NonInteractableNpc.NonInteractableNpc>(
                prefab, pos, Quaternion.identity, _parentTransform
            );
            return npc;
        }

        /// <summary>  Обработчик события тика времени. Управляет спавном NPC на основе игрового времени. </summary>
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void OnTimeTickHandler(DateTime gameTime)
        {
            if (!_isSpawning || _isTimePaused) return;

            _tickCounter++;

            if (_tickCounter >= SpawnIntervalInTicks && CanSpawn())
            {
                SpawnNpcFromPool();
                _tickCounter = 0;
            }
        }

        /// <summary> Проверяет, можно ли создать нового NPC. </summary>
        /// <returns> True, если количество активных NPC меньше максимального и время не приостановлено. </returns>
        private bool CanSpawn() => _activeCharacters.Count < _maxTotalCharacters && !_isTimePaused;

        /// <summary> Извлекает NPC из пула и настраивает его для спавна. </summary>
        private void SpawnNpcFromPool()
        {
            var npc = _npcPool.Get();
            var npcSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            var pos = npcSpawnPoint.GetRandomPosition();

            npc.transform.position = pos;
            npc.transform.rotation = Quaternion.identity;
            _activeCharacters.Add(npc);

            npc.OnReachedDespawnPoint += () => DespawnNpc(npc);
            npc.SetDespawnPoint(GetRandomDespawnPoint(npcSpawnPoint).transform.position);
            StartCoroutine(SetDestinationAfterInit(npc));
        }

        /// <summary>  Устанавливает пункт назначения для созданного NPC после его инициализации. </summary>
        /// <param name="npc"> NPC, для которого устанавливается пункт назначения. </param>
        /// <returns> Корутина для установки пункта назначения. </returns>
        private IEnumerator SetDestinationAfterInit(NonInteractableNpc.NonInteractableNpc npc)
        {
            yield return null;
            var loc = _locationManager.GetLocationByName(LocationName.NewShop);
            npc.SetDestination(loc.transform.position, LocationName.NewShop);
        }

        /// <summary> Возвращает NPC в пул и удаляет из списка активных. </summary>
        /// <param name="npc"> NPC для деспавна. </param>
        private void DespawnNpc(NonInteractableNpc.NonInteractableNpc npc)
        {
            if (_activeCharacters.Contains(npc))
            {
                _activeCharacters.Remove(npc);
                _npcPool.Release(npc);
            }
        }

        /// <summary> Корутина для деспавна всех активных NPC с пропуском одного кадра.
        /// Используется при окончании дня для очистки мира от NPC. </summary>
        /// <returns> Корутина для выполнения деспавна. </returns>
        private IEnumerator DespawnAllNpcCoroutine()
        {
            yield return null;
            foreach (var npc in _activeCharacters.ToArray()) DespawnNpc(npc);
        }

        /// <summary> Получает случайную точку деспавна, исключая указанную точку. </summary>
        /// <param name="excludePoint"> Точка, которую следует исключить из выбора. </param>
        /// <returns> Случайная точка деспавна. </returns>
        private NpcSpawnPoint GetRandomDespawnPoint(NpcSpawnPoint excludePoint)
        {
            if (_spawnPoints.Length == 1) return _spawnPoints[0];

            NpcSpawnPoint result;
            do
            {
                result = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            } while (result == excludePoint && _spawnPoints.Length > 1);

            return result;
        }
    }
}