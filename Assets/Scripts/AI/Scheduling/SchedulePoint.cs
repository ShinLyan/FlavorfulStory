using System;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Точка расписания для NPC, определяющая, куда и когда он должен переместиться. </summary>
    [Serializable]
    public class SchedulePoint
    {
        /// <summary> Час, в который NPC начнет движение в указанную точку. </summary>
        [field: Header("Start Time")]
        [field: Tooltip("Час, в который NPC начнет движение в указанную точку."), SerializeField, Range(0, 23)]
        public int Hour { get; private set; }

        /// <summary> Минута, в которую NPC начнет движение в указанную точку. </summary>
        [field: Tooltip("Минута, в которую NPC начнет движение в указанную точку."), SerializeField, Range(0, 59)]
        public int Minutes { get; private set; }

        /// <summary> Название сцены, в которой находится указанная точка. </summary>
        [field: Header("Scene Settings")]
        [field: Tooltip("Название сцены, в которой находится указанная точка."), SerializeField]
        public LocationName LocationName { get; private set; }

        /// <summary> Координаты, в которые должен прийти NPC. </summary>
        [field: Tooltip("Координаты, в которые должен прийти NPC."), SerializeField]
        public Vector3 Position { get; private set; }

        /// <summary> Угол поворота NPC при достижении точки. </summary>
        [field: Tooltip("Угол поворота NPC при достижении точки."), SerializeField]
        public Vector3 Rotation { get; private set; }

        /// <summary> Название анимации, которая будет воспроизводиться в данной точке. </summary>
        [field: Header("Other Settings")]
        [field: Tooltip("Название анимации, которая будет воспроизводиться в данной точке."), SerializeField]
        public NpcAnimationClipName NpcAnimationClipName { get; private set; }

        /// <summary> Набор диалогов в указанных координатах. </summary>
        [Tooltip("Набор диалогов в указанных координатах.")]
        public string[] DialoguePool { get; private set; } // TODO: в будущем заменить на спец. класс для диалогов
    }
}