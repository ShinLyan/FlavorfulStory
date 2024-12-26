using System;
using UnityEngine;

namespace FlavorfulStory.AI.Movement
{
    [Serializable]
    public class Schedule
    {
        public string StartTime;
        public string EndTime;
        public Transform Position;
    }
}