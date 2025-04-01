using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер NPC, управляющий состояниями и поведением персонажа. </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Npc : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Расписание NPC, определяющее его поведение и маршруты. </summary>
        [Tooltip("Расписание NPC, определяющее его поведение и маршруты."), SerializeField]
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        #endregion


        /// <summary> Инициализация контроллера состояний. </summary>
        private void Start()
        {
            _stateController = new StateController(
                GetComponent<NavMeshAgent>(),
                FindObjectsByType<WarpPortal>(FindObjectsInactive.Include, FindObjectsSortMode.None),
                GetComponent<Animator>(),
                _npcSchedule,
                transform,
                this,
                FindObjectsByType<Location>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            );
        }

        /// <summary> Обновление логики состояний каждый кадр. </summary>
        private void Update() => _stateController.Update(Time.deltaTime);
    }
}