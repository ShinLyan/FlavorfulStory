using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> NPC с возможностью взаимодействия с игроком. </summary>
    /// <remarks> Требует наличия компонента NpcCollisionHandler для обработки столкновений. </remarks>
    [RequireComponent(typeof(NpcCollisionHandler))]
    public class InteractableNpc : Npc
    {
        /// <summary> Обработчик столкновений для взаимодействия с игроком. </summary>
        private NpcCollisionHandler _collisionHandler;

        /// <summary> Обработчик расписания NPC </summary>
        private NpcScheduleHandler _npcScheduleHandler;

        /// <summary> Инициализирует компонент обработчика столкновений. </summary>
        protected override void Awake() => _collisionHandler = GetComponent<NpcCollisionHandler>();

        /// <summary> Выполняет инициализацию всех систем и компонентов InteractableNpc. </summary>
        protected override void Start()
        {
            _npcScheduleHandler = new NpcScheduleHandler();
            base.Start();
            _collisionHandler.Initialize(_stateController as ICharacterCollisionHandler);
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
            return new StateControllerInteractableNpc(
                _npcSchedule,
                _movementController as InteractableNpcMovementController,
                _animationController,
                _npcScheduleHandler,
                _playerController,
                transform
            );
        }
    }
}