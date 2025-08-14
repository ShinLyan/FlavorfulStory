using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.Player;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> NPC с возможностью взаимодействия с игроком. </summary>
    /// <remarks> Требует наличия компонента NpcCollisionHandler для обработки столкновений. </remarks>
    [RequireComponent(typeof(CapsuleCollider), typeof(NpcSpeaker))]
    public class InteractableNpc : Npc
    {
        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        protected NpcSchedule _npcSchedule;

        /// <summary> Информация о персонаже. </summary>
        [field: Tooltip("Информация о персонаже."), SerializeField]
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Обработчик расписания NPC </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Обработчик столкновений для взаимодействия с игроком. </summary>
        private NpcCollisionHandler _collisionHandler;

        /// <summary> Контроллер игрока, используемый для взаимодействия NPC с игроком. </summary>
        [Inject] protected PlayerController _playerController;

        /// <summary> Инициализирует компонент обработчика столкновений. </summary>
        protected override void Awake()
        {
            _npcScheduleHandler = new NpcScheduleHandler();
            base.Awake();
            _collisionHandler = new NpcCollisionHandler(_stateController as ICharacterCollisionHandler);
        }

        /// <summary> Отписка от событий при уничтожении. </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _npcScheduleHandler.Dispose();
        }

        /// <summary> Создает контроллер движения для интерактивного NPC. </summary>
        /// <returns> Новый экземпляр InteractableNpcMovementController. </returns>
        protected override NpcMovementController CreateMovementController() => new InteractableNpcMovementController(
            GetComponent<NavMeshAgent>(), transform, _animationController, _npcScheduleHandler);

        /// <summary> Создает контроллер состояний для интерактивного NPC. </summary>
        /// <returns> Новый экземпляр StateControllerInteractableNpc. </returns>
        protected override NpcStateController CreateStateController() => new InteractableNpcStateController(
            _npcSchedule, _movementController as InteractableNpcMovementController, _animationController,
            _npcScheduleHandler, transform, _playerController);

        /// <summary> Обрабатывает вход другого объекта в триггер коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        private void OnTriggerEnter(Collider other) => _collisionHandler?.HandleTriggerEnter(other);

        /// <summary> Обрабатывает выход другого объекта из триггера коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        private void OnTriggerExit(Collider other) => _collisionHandler?.HandleTriggerExit(other);
    }
}