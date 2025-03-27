using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Расписание NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/NPC Schedule")]
    public class NpcSchedule : ScriptableObject
    {
        /// <summary> Параметры расписания NPC. </summary>
        [field: Tooltip("Параметры расписания NPC."), SerializeField]
        public ScheduleParams[] Params { get; private set; }

        /// <summary> Получить отсортированные параметры расписания по текущим условиям. </summary>
        public IEnumerable<ScheduleParams> GetSortedScheduleParams()
        {
            if (Params == null) return Enumerable.Empty<ScheduleParams>();

            return Params
                .OrderByDescending(p => p.IsRaining)
                .ThenByDescending(p => p.Hearts)
                .ThenByDescending(p => p.Dates.Length > 0 ? p.Dates[0].x : 0)
                .ThenByDescending(p => p.DayOfWeek)
                .ThenByDescending(p => p.Seasons);
        }
    }
}