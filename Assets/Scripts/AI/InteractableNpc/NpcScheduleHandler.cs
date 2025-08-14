using System;
using System.Collections.Generic;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Обработчик расписания NPC, который управляет временными точками расписания персонажа. </summary>
    public class NpcScheduleHandler : IDisposable
    {
        /// <summary> Стек текущих точек расписания, отсортированных по времени. </summary>
        private Stack<NpcSchedulePoint> _currentPath;

        /// <summary> Текущая активная точка расписания NPC. </summary>
        public NpcSchedulePoint CurrentPoint { get; private set; }

        /// <summary> Событие, вызываемое при изменении точки расписания. </summary>
        public event Action<NpcSchedulePoint> OnSchedulePointChanged;

        /// <summary> Инициализирует новый экземпляр обработчика расписания и подписывается на события времени. </summary>
        public NpcScheduleHandler()
        {
            WorldTime.OnTimeTick += UpdateSchedulePoint;
            WorldTime.OnDayEnded += Reset;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            WorldTime.OnTimeTick -= UpdateSchedulePoint;
            WorldTime.OnDayEnded -= Reset;
        }

        /// <summary> Устанавливает параметры текущего расписания для NPC. </summary>
        /// <param name="npcScheduleParams"> Параметры расписания для установки. </param>
        public void SetCurrentScheduleParams(NpcScheduleParams npcScheduleParams) =>
            _currentPath = npcScheduleParams?.GetSortedSchedulePointsStack();

        /// <summary> Получает следующую точку расписания без удаления её из стека. </summary>
        /// <returns> Следующая точка расписания или null, если стек пуст. </returns>
        private NpcSchedulePoint GetNextSchedulePoint() =>
            _currentPath == null || _currentPath.Count == 0 ? null : _currentPath.Peek();

        /// <summary> Извлекает следующую точку расписания из стека. </summary>
        /// <returns> Следующая точка расписания или null, если стек пуст. </returns>
        private NpcSchedulePoint PopNextSchedulePoint() =>
            _currentPath == null || _currentPath.Count == 0 ? null : _currentPath.Pop();

        /// <summary> Обновляет текущую точку расписания при изменении игрового времени. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void UpdateSchedulePoint(DateTime currentTime)
        {
            var nextSchedulePoint = GetNextSchedulePoint();
            if (nextSchedulePoint == null) return;

            if ((int)currentTime.Hour == nextSchedulePoint.Hour && (int)currentTime.Minute == nextSchedulePoint.Minutes)
            {
                CurrentPoint = PopNextSchedulePoint();
                OnSchedulePointChanged?.Invoke(CurrentPoint);
            }
        }

        /// <summary> Сбросить состояние. </summary>
        /// <param name="time"> Текущее время. </param>
        private void Reset(DateTime time) => CurrentPoint = null;
    }
}