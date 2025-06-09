using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI
{
    public class NpcScheduleHandler : IScheduleDependable
    {
        private Stack<SchedulePoint> _currentPath;

        public void SetCurrentScheduleParams(ScheduleParams scheduleParams)
        {
            _currentPath = scheduleParams?.GetSortedSchedulePointsStack();
        }

        public SchedulePoint GetNextSchedulePoint()
        {
            if (_currentPath == null || _currentPath.Count == 0) return null;

            return _currentPath.Peek();
        }

        public SchedulePoint PopNextSchedulePoint()
        {
            if (_currentPath == null || _currentPath.Count == 0) return null;

            return _currentPath.Pop();
        }
    }
}