using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Базовый класс для всех NPC. </summary>
    [RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent), typeof(Animator))]
    public abstract class Npc : MonoBehaviour
    {
        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        protected NpcAnimationController _animationController;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        protected NpcStateController _stateController;

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        protected NpcMovementController _movementController;

        /// <summary> Вызывается при создании объекта, может быть переопределен в наследниках </summary>
        protected virtual void Awake()
        {
            _animationController = CreateAnimationController();
            _movementController = CreateMovementController();
            _stateController = CreateStateController();
        }

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        protected virtual void OnDestroy()
        {
            _animationController.Dispose();
            _stateController.Dispose();
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        protected void Update()
        {
            _movementController.UpdateMovement();
            _stateController.Update();
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
        protected abstract NpcStateController CreateStateController();
    }
}