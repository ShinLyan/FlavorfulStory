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

        /// <summary> Получить подходящие параметры расписания по текущим условиям </summary>
        public IEnumerable<ScheduleParams> GetSortedScheduleParams()
        {
            if (Params == null) return Enumerable.Empty<ScheduleParams>();

            return Params
                .OrderByDescending(p => p.IsRaining)
                .ThenByDescending(p => p.Hearts)
                .ThenByDescending(p => p.Seasons != 0);
        }
    }
}