using System;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    [Serializable]
    public class SchedulePoint
    {
        [Header("Start Time")]
        [Range(0, 24)] public int Hour;
        [Range(0, 60)] public int Minutes;

        [Header("Scene Settings")]
        public SceneType SceneName;
        public Vector3 Position;
        public Vector3 Rotation;

        [Header("Other Settings")] 
        public string AnimationClipName;
        public string[] DialoguePool; // TODO: в будущем заменить на спец. класс для диалогов
    }
}