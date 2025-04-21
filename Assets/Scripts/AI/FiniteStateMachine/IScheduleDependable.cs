using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Интерфейс для объектов, зависящих от расписания NPC. </summary>
    public interface IScheduleDependable
    {
        /// <summary> Установить текущие параметры расписания. </summary>
        /// <param name="scheduleParams"> Активные параметры расписания, применяемые к объекту. </param>
        void SetCurrentScheduleParams(ScheduleParams scheduleParams);
    }
}