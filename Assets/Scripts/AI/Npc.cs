using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Npc : MonoBehaviour
    {
        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Инициализация контроллера состояний. </summary>
        private void Awake()
        {
            _stateController = new StateController(
                _npcSchedule,
                GetComponent<Animator>(),
                transform,
                GetComponent<NavMeshAgent>(),
                this
            );
        }

        /// <summary> Обновление логики состояний каждый кадр. </summary>
        private void Update() => _stateController.Update(Time.deltaTime);
    }
}