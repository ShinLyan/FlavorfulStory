using System;
using FlavorfulStory.TimeManagement;
using GD.MinMaxSlider;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using DayOfWeek = FlavorfulStory.TimeManagement.DayOfWeek;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Параметры расписания для NPC. </summary>
    [Serializable]
    public class ScheduleParams
    {
        /// <summary> Сезоны, в которые будет выполняться расписание. </summary>]
        [field: Header("Limitations")]
        [field: Tooltip("Сезоны, в которые будет выполняться расписание."), SerializeField, EnumButtons]
        public Season Seasons { get; private set; }

        /// <summary> Дни недели, в которые будет выполняться расписание. </summary>
        [field: Tooltip("Дни недели, в которые будет выполняться расписание."), SerializeField]
        public DayOfWeek DayOfWeek { get; private set; }

        /// <summary> Дни месяца, в которые будет выполняться расписание. </summary>
        [field: Tooltip("Дни месяца, в которые будет выполняться расписание."), SerializeField, MinMaxSlider(1, 28)]
        public Vector2Int[] Dates { get; private set; }

        /// <summary> Минимальный уровень отношений, необходимый для выполнения расписания. </summary>
        [field: Tooltip("Минимальный уровень отношений, необходимый для выполнения расписания."),
                Range(0, 14), SerializeField]
        public int Hearts { get; private set; }

        /// <summary> Будет ли расписание выполняться в дождливую погоду. </summary>
        [field: Tooltip("Расписание будет выполнятся при дожде."), SerializeField]
        public bool IsRaining { get; private set; }

        /// <summary> Массив точек, которые NPC должен посетить в рамках расписания. </summary>
        [field: Header("Path")]
        [field: Tooltip("Массив точек, которые NPC должен посетить в рамках расписания."), SerializeField]
        public SchedulePoint[] Path { get; private set; }

        /// <summary> Найти ближайшую точку маршрута, соответствующую текущему времени. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        /// <returns> Ближайшая точка маршрута или <c>null</c>, если подходящая точка не найдена. </returns>
        public SchedulePoint GetClosestSchedulePointInPath(DateTime currentTime)
        {
            var currentMinutes = currentTime.Hour * 60 + currentTime.Minute;
            SchedulePoint closestPoint = null;
            var minTimeDifference = int.MaxValue;

            foreach (var pathPoint in Path)
            {
                var pathPointMinutes = pathPoint.Hour * 60 + pathPoint.Minutes;
                var timeDifference = currentMinutes - pathPointMinutes;

                if (timeDifference < 0 || timeDifference >= minTimeDifference) continue;

                minTimeDifference = timeDifference;
                closestPoint = pathPoint;
            }

            return closestPoint;
        }

        /// <summary> Проверка на подходящие условия</summary>
        /// <param name="currentTime"> Текущее время. </param>
        /// <param name="currentHearts"> Текущие отнощения. </param>
        /// <param name="isRaining"> Идет ли дождь. </param>
        /// <returns></returns>
        public bool AreConditionsSuitable(DateTime currentTime, int currentHearts, bool isRaining)
        {
            if (IsRaining != isRaining)
                return false;
            if (Hearts > 0 && currentHearts < Hearts)
                return false;
            if (Dates.Length > 0 && !IsDateInRanges(currentTime.SeasonDay))
                return false;
            if (DayOfWeek != 0 && (DayOfWeek & currentTime.DayOfWeek) == 0)
                return false;
            if (Seasons != 0 && (Seasons & currentTime.Season) == 0)
                return false;
            return true;
        }

        /// <summary> Проверяет, попадает ли день в заданные диапазоны. </summary>
        private bool IsDateInRanges(int day)
        {
            foreach (var range in Dates)
                if (day >= range.x && day <= range.y)
                    return true;
            return false;
        }
    }
}