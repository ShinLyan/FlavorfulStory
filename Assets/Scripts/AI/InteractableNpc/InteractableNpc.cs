using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> NPC с возможностью взаимодействия с игроком. </summary>
    /// <remarks> Требует наличия компонента NpcCollisionHandler для обработки столкновений. </remarks>
    public class InteractableNpc : Npc
    {
        /// <summary> Информация о персонаже. </summary>
        [field: SerializeField]
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        protected NpcSchedule _npcSchedule;

        /// <summary> Обработчик столкновений для взаимодействия с игроком. </summary>
        private NpcCollisionHandler _collisionHandler;

        /// <summary> Обработчик расписания NPC </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Инициализирует компонент обработчика столкновений. </summary>
        protected override void Awake()
        {
            _npcScheduleHandler = new NpcScheduleHandler();
            base.Awake();
            _collisionHandler = new NpcCollisionHandler(_stateController as InteractableNpcStateController);
        }

        /// <summary> Создает контроллер движения для интерактивного NPC. </summary>
        /// <returns> Новый экземпляр InteractableNpcMovementController. </returns>
        protected override NpcMovementController CreateMovementController()
        {
            return new InteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
                WarpGraph.Build(FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None)),
                transform,
                _animationController,
                _npcScheduleHandler
            );
        }

        /// <summary> Создает контроллер состояний для интерактивного NPC. </summary>
        /// <returns> Новый экземпляр StateControllerInteractableNpc. </returns>
        protected override StateController CreateStateController()
        {
            return new InteractableNpcStateController(
                _npcSchedule,
                _movementController as InteractableNpcMovementController,
                _animationController,
                _npcScheduleHandler,
                _playerController,
                transform
            );
        }

        /// <summary> Обрабатывает вход другого объекта в триггер коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        private void OnTriggerEnter(Collider other) => _collisionHandler?.HandleTriggerEnter(other);

        /// <summary> Обрабатывает выход другого объекта из триггера коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        private void OnTriggerExit(Collider other) => _collisionHandler?.HandleTriggerExit(other);
    }
}