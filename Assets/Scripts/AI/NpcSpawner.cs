using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Random = UnityEngine.Random;

namespace FlavorfulStory.AI
{
    /// <summary> Управляет спавном и деспавном NPC в игре. </summary>
    public class NpcSpawner : MonoBehaviour
    {
        /// <summary> Данные точек спавна NPC. </summary>
        [Header("Spawner Settings")] [SerializeField]
        private NpcSpawnData[] _spawnData;

        /// <summary> Префабы NPC. </summary>
        [SerializeField] private NonInteractableNpc.NonInteractableNpc[] _npcPrefabs;

        /// <summary> Родительский объект для NPC. </summary>
        [SerializeField] private Transform _parentTransform;

        /// <summary> Фабрика для создания NPC. </summary>
        private IPrefabFactory<NonInteractableNpc.NonInteractableNpc> _npcFactory;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Пул объектов NPC. </summary>
        private ObjectPool<NonInteractableNpc.NonInteractableNpc> _npcPool;

        /// <summary> Список активных NPC. </summary>
        private List<NonInteractableNpc.NonInteractableNpc> _activeCharacters;

        /// <summary> Счетчик тиков. </summary>
        private int _tickCounter;

        /// <summary> Максимальное количество NPC. </summary>
        private const int MaxNpcCount = 10;

        /// <summary> Интервал спавна в минутах. </summary>
        private const int SpawnIntervalInMinutes = 30;

        /// <summary> Проверяет возможность спавна нового NPC. </summary>
        /// <returns> True если можно спавнить, иначе False. </returns>
        private bool CanSpawn => _activeCharacters.Count < MaxNpcCount;

        /// <summary> Инициализирует зависимости. </summary>
        /// <param name="npcFactory"> Фабрика NPC. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        [Inject]
        public void Construct(IPrefabFactory<NonInteractableNpc.NonInteractableNpc> npcFactory,
            LocationManager locationManager)
        {
            _npcFactory = npcFactory;
            _locationManager = locationManager;
        }

        /// <summary> Настройка при инициализации. </summary>
        private void Awake()
        {
            if (_spawnData == null || _spawnData.Length < 2)
                Debug.LogError("NpcSpawner: требуется минимум 2 точки спавна/деспавна.");

            _npcPool = new ObjectPool<NonInteractableNpc.NonInteractableNpc>(
                CreateNpc,
                npc => npc.gameObject.SetActive(true),
                npc => npc.gameObject.SetActive(false)
            );

            _activeCharacters = new List<NonInteractableNpc.NonInteractableNpc>();

            WorldTime.OnDayEnded += HandleDayEnded;
            WorldTime.OnTimeTick += OnTimeTickHandler;
        }

        /// <summary> Отписывается от событий при уничтожении. </summary>
        private void OnDestroy()
        {
            WorldTime.OnDayEnded -= HandleDayEnded;
            WorldTime.OnTimeTick -= OnTimeTickHandler;
        }

        /// <summary> Обрабатывает окончание дня. </summary>
        /// <param name="_"> Игнорируемый параметр даты. </param>
        private void HandleDayEnded(DateTime _) => DespawnAllNpc().Forget();

        /// <summary> Обрабатывает тик времени. </summary>
        /// <param name="_"> Игнорируемый параметр даты. </param>
        private void OnTimeTickHandler(DateTime _)
        {
            if (WorldTime.IsPaused) return;

            _tickCounter++;
            int spawnIntervalInTicks = SpawnIntervalInMinutes / (int)WorldTime.TimeScale;

            if (_tickCounter < spawnIntervalInTicks || !CanSpawn) return;

            SpawnNpcFromPool();
            _tickCounter = 0;
        }

        /// <summary> Создает NPC для пула. </summary>
        /// <returns> Созданный NPC. </returns>
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

        /// <summary> Спавнит NPC из пула. </summary>
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

        /// <summary> Деспавнит указанного NPC. </summary>
        /// <param name="npc"> NPC для деспавна. </param>
        private void DespawnNpc(NonInteractableNpc.NonInteractableNpc npc)
        {
            _activeCharacters.Remove(npc);
            _npcPool.Release(npc);
        }

        /// <summary> Настраивает NPC после создания/спавна. </summary>
        /// <param name="npc"> Настраиваемый NPC. </param>
        /// <param name="spawnData"> Данные точки спавна. </param>
        /// <param name="despawnData"> Данные точки деспавна. </param>
        /// <param name="subscribeDespawnHandler"> Нужно ли подписывать обработчик деспавна. </param>
        private void SetupNpc(NonInteractableNpc.NonInteractableNpc npc, NpcSpawnData spawnData,
            NpcSpawnData despawnData, bool subscribeDespawnHandler)
        {
            var agent = npc.GetComponent<NavMeshAgent>();
            agent.Warp(spawnData.SpawnPoint.position);
            npc.transform.rotation = spawnData.SpawnPoint.rotation;

            npc.SetDespawnPoint(new NpcDestinationPoint(despawnData.DespawnPoint.position, Quaternion.identity));
            if (subscribeDespawnHandler) npc.OnReachedDespawnPoint += () => DespawnNpc(npc);
        }

        /// <summary> Устанавливает цель NPC после инициализации. </summary>
        /// <param name="npc"> NPC для установки цели. </param>
        private async UniTask SetDestinationAfterInit(NonInteractableNpc.NonInteractableNpc npc)
        {
            var agent = npc.GetComponent<NavMeshAgent>();

            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning($"NPC {npc.name} could not be placed on NavMesh");
                return;
            }

            await UniTask.Yield();

            if (!npc || !npc.gameObject.activeInHierarchy) return;

            var shopEntryPointPosition = ((ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop))
                .EntryPoint.position;
            npc.SetDestination(new NpcDestinationPoint(shopEntryPointPosition, Quaternion.identity));
        }

        /// <summary> Деспавнит всех активных NPC. </summary>
        private async UniTask DespawnAllNpc()
        {
            for (int i = _activeCharacters.Count - 1; i >= 0; i--)
            {
                DespawnNpc(_activeCharacters[i]);
                await UniTask.Yield();
            }

            _tickCounter = 0;
        }
    }
}