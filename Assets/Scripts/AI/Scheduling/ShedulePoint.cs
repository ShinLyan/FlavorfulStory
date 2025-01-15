using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.Scheduling
{
    [Serializable]
    public class SchedulePoint
    {
        [SerializeField, Range(1, 28)] private int _date;
        [SerializeField] private Seasons _season;
        [SerializeField, Range(1, 99)] private int _year;
        [SerializeField, Range(0, 24)] private int _hour;
        [SerializeField, Range(0, 60)] private int _minutes;
        
        [HideInInspector] public DateTime StartTime;   
        public Transform Position;

        private void OnValidate()
        {
            StartTime = new DateTime(_year, _season, _date, _hour, _minutes);
        }
    }
}