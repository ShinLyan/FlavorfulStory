using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Random = UnityEngine.Random;

namespace FlavorfulStory.AI
{
    /// <summary> Управляет спавном и деспавном NPC. </summary>
    public class NpcSpawner : MonoBehaviour
    {
        [Header("Спавн")] [SerializeField] private NpcSpawnData[] _spawnData;

        /// <summary> Префабы NPC. </summary>
        [SerializeField] private NonInteractableNpc.NonInteractableNpc[] _npcPrefabs;

        /// <summary> Родительский объект для NPC. </summary>
        [SerializeField] private Transform _parentTransform;

        [Header("Ограничения")] [SerializeField]
        private int _maxNpcCount = 5;

        /// <summary> Интервал спавна в минутах. </summary>
        [SerializeField] private int _spawnIntervalInMinutes = 60;

        /// <summary> Количество минут в одном тике времени. </summary>
        private const int MinutesInTick = 5;

        private IPrefabFactory<NonInteractableNpc.NonInteractableNpc> _npcFactory;
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

        [Inject]
        public void Construct(IPrefabFactory<NonInteractableNpc.NonInteractableNpc> npcFactory,
            LocationManager locationManager)
        {
            _npcFactory = npcFactory;
            _locationManager = locationManager;
        }

        private void Awake()
        {
            if (_spawnData == null || _spawnData.Length < 2)
            {
                Debug.LogError("NpcSpawner: требуется минимум 2 точки спавна/деспавна.");
                _isSpawning = false;
            }

            _spawnIntervalInTicks = Mathf.Max(1, _spawnIntervalInMinutes / MinutesInTick);

            _npcPool = new ObjectPool<NonInteractableNpc.NonInteractableNpc>(
                CreateNpc,
                npc => npc.gameObject.SetActive(true),
                npc => npc.gameObject.SetActive(false)
            );

            WorldTime.OnDayEnded += HandleDayEnded;
            WorldTime.OnTimePaused += HandlePause;
            WorldTime.OnTimeUnpaused += HandleUnpause;
            WorldTime.OnTimeTick += OnTimeTickHandler;
        }

        private void OnDestroy()
        {
            WorldTime.OnDayEnded -= HandleDayEnded;
            WorldTime.OnTimePaused -= HandlePause;
            WorldTime.OnTimeUnpaused -= HandleUnpause;
            WorldTime.OnTimeTick -= OnTimeTickHandler;
        }

        private void HandleDayEnded(DateTime _) => DespawnAllNpcCoroutine().Forget();
        private void HandlePause() => _isTimePaused = true;
        private void HandleUnpause() => _isTimePaused = false;

        private void OnTimeTickHandler(DateTime _)
        {
            if (!_isSpawning || _isTimePaused) return;

            _tickCounter++;

            if (_tickCounter >= _spawnIntervalInTicks && CanSpawn())
            {
                SpawnNpcFromPool();
                _tickCounter = 0;
            }
        }

        private bool CanSpawn() => _activeCharacters.Count < _maxNpcCount;

        /// <summary>
        /// Создание объекта для пула: создаём сразу в одной из точек спавна (не в нуле) и без подписки на деспавн.
        /// </summary>
        private NonInteractableNpc.NonInteractableNpc CreateNpc()
        {
            int spawnIndex = Random.Range(0, _spawnData.Length);
            int despawnIndex;
            do
            {
                despawnIndex = Random.Range(0, _spawnData.Length);
            } while (despawnIndex == spawnIndex);

            var spawnData = _spawnData[spawnIndex];
            var despawnData = _spawnData[despawnIndex];

            var prefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Length)];

            var npcInstance = _npcFactory.Create(prefab, spawnData.SpawnPoint.position, spawnData.SpawnPoint.rotation,
                _parentTransform);

            SetupNpc(npcInstance, spawnData, despawnData, true);

            return npcInstance;
        }

        private void SpawnNpcFromPool()
        {
            int spawnIndex = Random.Range(0, _spawnData.Length);
            int despawnIndex;
            do
            {
                despawnIndex = Random.Range(0, _spawnData.Length);
            } while (despawnIndex == spawnIndex);

            var spawnData = _spawnData[spawnIndex];
            var despawnData = _spawnData[despawnIndex];

            var npc = _npcPool.Get();

            SetupNpc(npc, spawnData, despawnData, false);

            _activeCharacters.Add(npc);

            SetDestinationAfterInit(npc).Forget();
        }

        private void DespawnNpc(NonInteractableNpc.NonInteractableNpc npc)
        {
            _activeCharacters.Remove(npc);
            _npcPool.Release(npc);
        }

        /// <summary>
        /// Общий метод: назначает точки спавна/деспавна и безопасно подписывает обработчик,
        /// может вызываться и для только что созданного, и для взятого из пула NPC.
        /// </summary>
        private void SetupNpc(NonInteractableNpc.NonInteractableNpc npc, NpcSpawnData spawnData,
            NpcSpawnData despawnData, bool subscribeDespawnHandler)
        {
            var agent = npc.GetComponent<NavMeshAgent>();
            agent.Warp(spawnData.SpawnPoint.position);
            npc.transform.rotation = spawnData.SpawnPoint.rotation;

            npc.SetDespawnPoint(new NpcDestinationPoint(despawnData.DespawnPoint.position, Quaternion.identity));
            if (subscribeDespawnHandler) npc.OnReachedDespawnPoint += () => DespawnNpc(npc);
        }

        private async UniTask SetDestinationAfterInit(NonInteractableNpc.NonInteractableNpc npc)
        {
            var agent = npc.GetComponent<NavMeshAgent>();

            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning($"NPC {npc.name} could not be placed on NavMesh");
                return;
            }

            await UniTask.Yield();

            var loc = _locationManager.GetLocationByName(LocationName.NewShop);
            npc.SetDestination(new NpcDestinationPoint(loc.transform.position, Quaternion.identity));
        }

        private async UniTask DespawnAllNpcCoroutine()
        {
            _isSpawning = false;

            for (int i = _activeCharacters.Count - 1; i >= 0; i--)
            {
                DespawnNpc(_activeCharacters[i]);
                await UniTask.Yield();
            }

            _isSpawning = true;
            _tickCounter = 0;
        }
    }
}