using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    [Serializable]
    public class ScheduleParams
    {
        [Header("Limitations")]
        public Seasons[] Season;
        public WeekDays[] WeekDays;
        [Range(1, 28)] public int[] Dates;
        [Range(0, 14)] public int Hearts;
        public bool IsRain;
        public SchedulePoint[] Path;
    }
}