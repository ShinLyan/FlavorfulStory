using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> NPC с возможностью взаимодействия с игроком. </summary>
    /// <remarks> Требует наличия компонента NpcCollisionHandler для обработки столкновений. </remarks>
    public class InteractableNpc : Npc
    {
        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        protected NpcSchedule _npcSchedule;

        /// <summary> Обработчик расписания NPC </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Инициализирует компонент обработчика столкновений. </summary>
        protected override void Awake()
        {
            _npcScheduleHandler = new NpcScheduleHandler();
            base.Awake();
        }

        /// <summary> Отписка от событий при уничтожении. </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _npcScheduleHandler.Dispose();
        }

        /// <summary> Создает контроллер движения для интерактивного NPC. </summary>
        /// <returns> Новый экземпляр InteractableNpcMovementController. </returns>
        protected override NpcMovementController CreateMovementController()
        {
            return new InteractableNpcMovementController(
                GetComponent<NavMeshAgent>(),
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
                transform,
                _playerController
            );
        }
    }
}