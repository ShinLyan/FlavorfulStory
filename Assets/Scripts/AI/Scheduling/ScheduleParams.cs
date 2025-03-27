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

        public bool AreConditionsMet(DateTime currentTime, int currentHearts, bool isRaining)
        {
            // 1. Проверка погоды
            if (IsRaining != isRaining)
                return false;
            Debug.Log("weather done");
            // 2. Проверка уровня отношений
            if (Hearts > 0 && currentHearts < Hearts)
                return false;
            Debug.Log("hearts done");
            // 3. Проверка дней месяца
            if (Dates.Length > 0 && !IsDateInRanges(currentTime.SeasonDay))
                return false;
            Debug.Log("day done");
            // 4. Проверка дня недели
            if (DayOfWeek != 0 && (DayOfWeek & currentTime.DayOfWeek) == 0)
                return false;
            Debug.Log("weekday done");
            // 5. Проверка сезона
            if (Seasons != 0 && (Seasons & currentTime.Season) == 0)
                return false;
            Debug.Log("season done");
            Debug.Log("==========");
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