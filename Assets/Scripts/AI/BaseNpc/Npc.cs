using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Базовый класс для всех NPC. </summary>
    [RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent), typeof(Animator))]
    public abstract class Npc : MonoBehaviour
    {
        /// <summary> Контроллер анимации NPC для воспроизведения состояний анимации. </summary>
        protected NpcAnimationController AnimationController { get; private set; }

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        protected abstract NpcStateController StateController { get; }

        /// <summary> Контроллер движения NPC для управления навигацией и перемещением. </summary>
        protected abstract NpcMovementController MovementController { get; }

        /// <summary> Вызывается при создании объекта, может быть переопределен в наследниках </summary>
        protected virtual void Awake()
        {
            AnimationController = new NpcAnimationController(GetComponent<Animator>());
            AnimationController.Initialize();
        }

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        protected virtual void OnDestroy()
        {
            AnimationController?.Dispose();
            StateController?.Dispose();
        }

        /// <summary> Обновляет логику состояний и движения NPC каждый кадр. </summary>
        protected virtual void Update()
        {
            MovementController?.Update();
            StateController?.Update();
        }
    }
}