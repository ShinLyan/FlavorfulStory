using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Базовый абстрактный класс для всех NPC персонажей в игре </summary>
    /// <remarks> Требует наличия компонентов NavMeshAgent и Animator на GameObject </remarks>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class Npc : MonoBehaviour
    {
        /// <summary> Информация о персонаже. </summary>
        [field: SerializeField]
        public NpcInfo NpcInfo { get; private set; }

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        protected NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        protected StateController _stateController;

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        protected NpcMovementController _movementController;

        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        protected NpcAnimationController _animationController;

        /// <summary> Контроллер игрока, используемый для взаимодействия NPC с игроком. </summary>
        [Inject] protected PlayerController _playerController;

        /// <summary> Вызывается при создании объекта, может быть переопределен в наследниках </summary>
        protected virtual void Awake() { }

        /// <summary> Выполняет инициализацию всех контроллеров и систем NPC при запуске. </summary>
        protected virtual void Start()
        {
            _animationController = CreateAnimationController();
            _movementController = CreateMovementController();
            _stateController = CreateStateController();

            WorldTime.OnTimePaused += _animationController.PauseAnimation;
            WorldTime.OnTimeUnpaused += _animationController.ContinueAnimation;
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        protected virtual void Update()
        {
            _stateController.Update();
            _movementController.UpdateMovement();
        }

        /// <summary> Создает контроллер анимации для NPC. </summary>
        /// <returns> Новый экземпляр NpcAnimationController. </returns>
        protected virtual NpcAnimationController CreateAnimationController()
        {
            return new NpcAnimationController(GetComponent<Animator>());
        }

        /// <summary> Создает контроллер движения для NPC. </summary>
        /// <returns> Новый экземпляр NpcMovementController. </returns>
        /// <remarks> Должен быть реализован в наследниках. </remarks>
        protected abstract NpcMovementController CreateMovementController();

        /// <summary> Создает контроллер состояний для NPC. </summary>
        /// <returns> Новый экземпляр StateController. </returns>
        /// <remarks> Должен быть реализован в наследниках. </remarks>
        protected abstract StateController CreateStateController();
    }
}