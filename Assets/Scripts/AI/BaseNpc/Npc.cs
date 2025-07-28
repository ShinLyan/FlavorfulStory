using FlavorfulStory.Player;
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
        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        protected StateController _stateController;

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        protected NpcMovementController _movementController;

        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        protected NpcAnimationController _animationController;

        /// <summary> Контроллер игрока, используемый для взаимодействия NPC с игроком. </summary>
        [Inject] protected PlayerController _playerController;

        /// <summary> Вызывается при создании объекта, может быть переопределен в наследниках </summary>
        protected virtual void Awake()
        {
            _animationController = CreateAnimationController();
            _movementController = CreateMovementController();
            _stateController = CreateStateController();
        }

        protected virtual void OnDestroy()
        {
            _animationController.Dispose();
            _stateController.Dispose();
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        protected void Update()
        {
            _stateController.Update();
            _movementController.UpdateMovement();
        }

        /// <summary> Создает контроллер анимации для NPC. </summary>
        /// <returns> Новый экземпляр NpcAnimationController. </returns>
        private NpcAnimationController CreateAnimationController() => new(GetComponent<Animator>());

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