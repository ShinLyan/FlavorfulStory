using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Класс, представляющий параметры расписания для NPC. </summary>
    [Serializable]
    public class ScheduleParams
    {
        /// <summary> Сезоны, в которые будет выполняться заданное расписание. </summary>
        [Header("Limitations")]
        [Tooltip("По каким сезонам будет выполнятся заданное расписание.")] public Seasons[] Season;

        /// <summary> Дни недели, в которые будет выполняться заданное расписание. </summary>
        [Tooltip("По каким дням недели будет выполнятся заданное расписание.")] public WeekDays[] WeekDays;

        /// <summary> Дни месяца, в которые будет выполняться заданное расписание. </summary>
        [Range(1, 28), Tooltip("По каким дням будет выполнятся заданное расписание.")] public int[] Dates;

        /// <summary> Уровень отношений, начиная с которого будет выполняться данное расписание. </summary>
        [Range(0, 14), Tooltip("С какого уровня взимоотношений будет выполняться данное расписание.")] public int Hearts;

        /// <summary> Указывает, будет ли расписание выполняться при дожде. </summary>
        [Tooltip("Расписание будет выполнятся при дожде.")] public bool IsRaining;

        /// <summary> Массив точек расписания, которые NPC должен посетить. </summary>
        public SchedulePoint[] Path;

        /// <summary>
        /// Возвращает ближайшую точку расписания, которую NPC должен посетить, основываясь на текущем времени.
        /// </summary>
        /// <param name="currentTime"> Текущее время, используемое для поиска ближайшей точки. </param>
        /// <returns> Ближайшая точка расписания или null, если подходящая точка не найдена. </returns>
        public SchedulePoint GetClosestSchedulePointInPath(DateTime currentTime)
        {
            int currentMinutes = currentTime.Hour * 60 + currentTime.Minute;

            int minTimeDifference = int.MaxValue;
            SchedulePoint closestPoint = null;

            foreach (var pathPoint in Path)
            {
                int pathPointMinutes = pathPoint.Hour * 60 + pathPoint.Minutes;

                if (pathPointMinutes <= currentMinutes)
                {
                    int timeDifference = currentMinutes - pathPointMinutes;

                    if (timeDifference < minTimeDifference)
                    {
                        minTimeDifference = timeDifference;
                        closestPoint = pathPoint;
                    }
                }
            }

            return closestPoint;
        }
    }
}