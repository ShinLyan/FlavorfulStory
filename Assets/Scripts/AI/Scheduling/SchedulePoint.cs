using System;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.Scheduling
{
    [Serializable]
    public class SchedulePoint
    {
        [Header("Start Time")]
        [Range(0, 24)] public int Hour;
        [Range(0, 60)] public int Minutes;

        [Header("Scene Settings")]
        public string SceneName;
        public Transform Position;
        public float X_Rotation;
        public float Y_Rotation;

        [Header("Other Settings")] 
        public Animation Animation;
        public string[] DialoguePool;
    }
}