using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    [CreateAssetMenu(fileName = "NpcSchedule", menuName = "FlavorfulStory/AI Behavior/NpcSchedule", order = 0)]
    public class NpcSchedule : ScriptableObject
    {
        public ScheduleParams[] Schedules;
    }
}