using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> ScriptableObject, представляющий расписание NPC. </summary>
    [CreateAssetMenu(fileName = "NpcSchedule", menuName = "FlavorfulStory/AI Behavior/NpcSchedule", order = 0)]
    public class NpcSchedule : ScriptableObject
    {
        /// <summary> Массив параметров расписаний для NPC. </summary>
        public ScheduleParams[] Schedules;
    }
}