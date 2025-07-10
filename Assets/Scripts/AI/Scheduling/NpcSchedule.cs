using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Расписание NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/NPC/Schedule")]
    public class NpcSchedule : ScriptableObject
    {
        /// <summary> Параметры расписания NPC. </summary>
        [field: Tooltip("Параметры расписания NPC."), SerializeField]
        public ScheduleParams[] Params { get; set; }

        /// <summary> Получить отсортированные параметры расписания по текущим условиям. </summary>
        /// <remarks> Приоритеты: 1. IsRaining; 2. Max Hearts; 3. По дате; 4. По DayOfWeek; 5. По Seasons </remarks>
        public IEnumerable<ScheduleParams> GetSortedScheduleParams() => Params?
            .OrderByDescending(p => p.IsRaining)
            .ThenByDescending(p => p.Hearts)
            .ThenByDescending(p => p.Dates.Length > 0 ? p.Dates[0].x : 0)
            .ThenByDescending(p => p.DayOfWeek)
            .ThenByDescending(p => p.Seasons);
    }
}