using FlavorfulStory.AI.Scheduling;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Интерфейс для объектов, которые зависят от текущей точки расписания
    /// и могут реагировать на её изменения. </summary>
    public interface ICurrentSchedulePointDependable
    {
        /// <summary> Устанавливает новую текущую точку расписания для объекта. </summary>
        /// <param name="newCurrentPont"> Новая текущая точка расписания. </param>
        void SetNewCurrentPont(SchedulePoint newCurrentPont);
    }
}