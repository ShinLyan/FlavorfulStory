using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Расписание NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/NPC Schedule")]
    public class NpcSchedule : ScriptableObject
    {
        /// <summary> Имя NPC, для которого будет выполняться данное расписание. </summary>
        [field: Tooltip("Имя NPC, для которого будет выполняться данное расписание."), SerializeField]
        public NpcName NpcName { get; private set; }

        /// <summary> Параметры расписания NPC. </summary>
        [field: Tooltip("Параметры расписания NPC."), SerializeField]
        public ScheduleParams[] Params { get; private set; }
    }
}