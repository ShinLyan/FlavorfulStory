using System;
using System.Linq;
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
        public Season Seasons { get; set; }

        /// <summary> Дни недели, в которые будет выполняться расписание. </summary>
        [field: Tooltip("Дни недели, в которые будет выполняться расписание."), SerializeField]
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary> Диапазоны дней месяца (1–28), в которые будет выполняться расписание. </summary>
        [field: Tooltip("Диапазоны дней месяца (1–28), в которые будет выполняться расписание."),
                SerializeField, MinMaxSlider(1, 28)]
        public Vector2Int[] Dates { get; set; }

        /// <summary> Минимальный уровень отношений, необходимый для выполнения расписания. </summary>
        [field: Tooltip("Минимальный уровень отношений, необходимый для выполнения расписания."),
                Range(0, 14), SerializeField]
        public int Hearts { get; set; }

        /// <summary> Должен ли идти дождь для активации расписания? </summary>
        [field: Tooltip("Должен ли идти дождь для активации расписания?"), SerializeField]
        public bool IsRaining { get; set; }

        /// <summary> Массив точек маршрута, которые NPC должен посетить в рамках расписания. </summary>
        [field: Header("Path")]
        [field: Tooltip("Массив точек маршрута, которые NPC должен посетить в рамках расписания."), SerializeField]
        public SchedulePoint[] Path { get; set; }

        public ScheduleParams()
        {
            Seasons = new Season();
            DayOfWeek = new DayOfWeek();
            Dates = Array.Empty<Vector2Int>();
            Hearts = 0;
            IsRaining = false;
            Path = Array.Empty<SchedulePoint>();
        }

        /// <summary> Найти ближайшую точку маршрута, соответствующую текущему времени. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        /// <returns> Ближайшая точка маршрута или <c>null</c>, если подходящая точка не найдена. </returns>
        public SchedulePoint GetClosestSchedulePointInPath(DateTime currentTime)
        {
            int currentMinutes = (int)currentTime.Hour * 60 + (int)currentTime.Minute;
            SchedulePoint closestPoint = null;
            int minTimeDifference = int.MaxValue;
            foreach (var pathPoint in Path)
            {
                int pathPointMinutes = pathPoint.Hour * 60 + pathPoint.Minutes;
                int timeDifference = currentMinutes - pathPointMinutes;

                if (timeDifference < 0 || timeDifference >= minTimeDifference) continue;

                minTimeDifference = timeDifference;
                closestPoint = pathPoint;
            }

            return closestPoint;
        }

        /// <summary> Удовлетворяют ли текущие условия требованиям расписания? </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        /// <param name="currentHearts"> Текущий уровень отношений. </param>
        /// <param name="isRaining"> Идёт ли в данный момент дождь? </param>
        /// <returns> <c>true</c>, если все условия выполняются; иначе — <c>false</c>. </returns>
        public bool AreConditionsSuitable(DateTime currentTime, int currentHearts, bool isRaining) =>
            IsRaining == isRaining &&
            (Hearts == 0 || currentHearts >= Hearts) &&
            (Dates.Length == 0 || IsDateInRanges((int)currentTime.SeasonDay)) &&
            (DayOfWeek == 0 || (DayOfWeek & currentTime.DayOfWeek) != 0) &&
            (Seasons == 0 || (Seasons & currentTime.Season) != 0);

        /// <summary> Входит ли указанный день в хотя бы один из заданных диапазонов дат? </summary>
        /// <param name="day"> Номер дня месяца (1–28). </param>
        /// <returns> <c>true</c>, если день попадает в один из диапазонов; иначе — <c>false</c>. </returns>
        private bool IsDateInRanges(int day) => Dates.Any(range => day >= range.x && day <= range.y);
    }
}