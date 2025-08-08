using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI
{
    /// <summary> Отвечает за спавн и деспавн NPC в игре. </summary>
    public class NpcSpawner : MonoBehaviour
    {
        /// <summary> Данные точек спавна NPC. </summary>
        [SerializeField] private NpcSpawnData[] _spawnData;

        /// <summary> Префабы NPC. </summary>
        [SerializeField] private NonInteractableNpc.NonInteractableNpc[] _npcPrefabs;

        /// <summary> Родительский объект для NPC. </summary>
        [SerializeField] private Transform _parentTransform;

        /// <summary> Максимальное количество NPC. </summary>
        [SerializeField] private int _maxNpcCount = 5;

        /// <summary> Интервал спавна в минутах. </summary>
        [SerializeField] private int _spawnIntervalInMinutes = 60;

        /// <summary> Количество минут в одном тике времени. </summary>
        private const int MinutesInTick = 5;

        /// <summary> DI контейнер. </summary>
        private DiContainer _diContainer;

        /// <summary> Пул объектов NPC. </summary>
        private ObjectPool<NonInteractableNpc.NonInteractableNpc> _npcPool;

        /// <summary> Список активных NPC. </summary>
        private readonly List<NonInteractableNpc.NonInteractableNpc> _activeCharacters = new();

        /// <summary> Интервал спавна в тиках. </summary>
        private int _spawnIntervalInTicks;

        /// <summary> Счетчик тиков. </summary>
        private int _tickCounter;

        /// <summary> Флаг паузы времени. </summary>
        private bool _isTimePaused;

        /// <summary> Флаг активности спавна. </summary>
        private bool _isSpawning = true;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Внедрение зависимостей. </summary>
        /// <param name="diContainer"> DI контейнер. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        [Inject]
        public void Construct(DiContainer diContainer, LocationManager locationManager)
        {
            _diContainer = diContainer;
            _locationManager = locationManager;
        }

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _spawnIntervalInTicks = _spawnIntervalInMinutes / MinutesInTick;

            _npcPool = new ObjectPool<NonInteractableNpc.NonInteractableNpc>(
                CreateNpc,
                npc => npc.gameObject.SetActive(true),
                npc => npc.gameObject.SetActive(false)
            );

            WorldTime.OnDayEnded += _ => DespawnAllNpcCoroutine().Forget();
            WorldTime.OnTimePaused += () => _isTimePaused = true;
            WorldTime.OnTimeUnpaused += () => _isTimePaused = false;
            WorldTime.OnTimeTick += OnTimeTickHandler;
        }

        /// <summary> Создает экземпляр NPC. </summary>
        /// <returns> Созданный NPC. </returns>
        private NonInteractableNpc.NonInteractableNpc CreateNpc()
        {
            var prefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Length)];
            var npcInstance = _diContainer.InstantiatePrefabForComponent<NonInteractableNpc.NonInteractableNpc>(
                prefab, _spawnData[0].spawnPoint.position, Quaternion.identity, _parentTransform
            );
            npcInstance.gameObject.SetActive(false);
            return npcInstance;
        }

        /// <summary> Обрабатывает тик времени. </summary>
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void OnTimeTickHandler(DateTime gameTime)
        {
            if (!_isSpawning || _isTimePaused) return;

            _tickCounter++;

            if (_tickCounter >= _spawnIntervalInTicks && CanSpawn())
            {
                SpawnNpcFromPool();
                _tickCounter = 0;
            }
        }

        /// <summary> Проверяет возможность спавна нового NPC. </summary>
        /// <returns> True если можно спавнить, иначе False. </returns>
        private bool CanSpawn() => _activeCharacters.Count < _maxNpcCount;

        /// <summary> Спавнит NPC из пула. </summary>
        private void SpawnNpcFromPool()
        {
            int spawnIndex = Random.Range(0, _spawnData.Length);
            var spawnData = _spawnData[spawnIndex];

            int despawnIndex;
            do
            {
                despawnIndex = Random.Range(0, _spawnData.Length);
            } while (despawnIndex == spawnIndex);

            var despawnData = _spawnData[despawnIndex];

            var npc = _npcPool.Get();

            npc.GetComponent<NavMeshAgent>().Warp(spawnData.spawnPoint.position);
            npc.transform.rotation = spawnData.spawnPoint.rotation;
            _activeCharacters.Add(npc);

            npc.OnReachedDespawnPoint += () => DespawnNpc(npc);
            npc.SetDespawnPoint(despawnData.despawnPoint.position);

            SetDestinationAfterInit(npc).Forget();
        }

        /// <summary> Деспавнит указанного NPC. </summary>
        /// <param name="npc"> NPC для деспавна. </param>
        private void DespawnNpc(NonInteractableNpc.NonInteractableNpc npc)
        {
            _activeCharacters.Remove(npc);
            _npcPool.Release(npc);
        }

        /// <summary> Устанавливает цель NPC после инициализации. </summary>
        /// <param name="npc"> NPC для установки цели. </param>
        private async UniTaskVoid SetDestinationAfterInit(NonInteractableNpc.NonInteractableNpc npc)
        {
            var agent = npc.GetComponent<NavMeshAgent>();

            while (agent == null || !agent.isOnNavMesh) await UniTask.Yield();

            var loc = _locationManager.GetLocationByName(LocationName.NewShop);
            npc.SetDestination(loc.transform.position);
        }

        /// <summary> Деспавнит всех активных NPC. </summary>
        private async UniTaskVoid DespawnAllNpcCoroutine()
        {
            _isSpawning = false;

            foreach (var npc in new List<NonInteractableNpc.NonInteractableNpc>(_activeCharacters))
            {
                DespawnNpc(npc);
                await UniTask.Yield();
            }

            _isSpawning = true;
            _tickCounter = 0;
        }
    }
}