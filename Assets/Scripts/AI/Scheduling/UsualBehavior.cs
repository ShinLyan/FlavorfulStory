using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    [CreateAssetMenu(fileName = "UsualBehavior", menuName = "FlavorfulStory/AI Behavior/Usual Behavior")]
    public class UsualBehavior : ScriptableObject
    {
        public SchedulePoint[] MondaySchedule;
        public SchedulePoint[] TuesdaySchedule;
        public SchedulePoint[] WednesdaySchedule;
        public SchedulePoint[] ThursdaySchedule;
        public SchedulePoint[] FridaySchedule;
        public SchedulePoint[] SaturdaySchedule;
        public SchedulePoint[] SundaySchedule;
    }
}