using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI
{
    public interface IScheduleDependable
    {
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams);
    }
}