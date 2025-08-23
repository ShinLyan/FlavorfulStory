using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> NPC с возможностью взаимодействия с игроком. </summary>
    [RequireComponent(typeof(CapsuleCollider), typeof(NpcSpeaker))]
    public sealed class InteractableNpc : Npc
    {
        /// <summary> Информация о персонаже. </summary>
        [field: Tooltip("Информация о персонаже."), SerializeField]
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Обработчик столкновений для взаимодействия с игроком. </summary>
        private NpcCollisionHandler _collisionHandler;

        /// <summary> Провайдер позиции игрока. </summary>
        private IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Контроллер состояний персонажа. </summary>
        private InteractableNpcStateController _stateController;

        /// <summary> Возвращает контроллер состояний для базового NPC. </summary>
        protected override NpcStateController StateController => _stateController;

        /// <summary> Внедряет зависимости Zenject. </summary>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        [Inject]
        private void Construct(IPlayerPositionProvider playerPositionProvider) =>
            _playerPositionProvider = playerPositionProvider;

        /// <summary> Выполняет инициализацию компонентов NPC. </summary>
        protected override void Awake()
        {
            base.Awake();

            _stateController = new InteractableNpcStateController(_npcSchedule, MovementController,
                AnimationController, transform, _playerPositionProvider);
            _collisionHandler = new NpcCollisionHandler(_stateController);
        }


        /// <summary> Обрабатывает вход другого объекта в триггер коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        private void OnTriggerEnter(Collider other) => _collisionHandler?.HandleTriggerEnter(other);

        /// <summary> Обрабатывает выход другого объекта из триггера коллизии NPC. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        private void OnTriggerExit(Collider other) => _collisionHandler?.HandleTriggerExit(other);
    }
}