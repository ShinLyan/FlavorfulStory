using System;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Класс, представляющий точку расписания для NPC. </summary>
    [Serializable]
    public class SchedulePoint
    {
        /// <summary> Час, в который NPC отправится на указанную точку. </summary>
        [Header("Start Time")]
        [Range(0, 23), Tooltip("Час, в который NPC отправится на указанную точку.")] public int Hour;

        /// <summary> Минута, в которую NPC отправится на указанную точку. </summary>
        [Range(0, 59), Tooltip("Минута, в которую NPC отправится на указанную точку.")] public int Minutes;

        /// <summary> Название сцены, на которой действительны указанные координаты. </summary>
        [FormerlySerializedAs("_locationName")]
        [FormerlySerializedAs("SceneName")]
        [Header("Scene Settings")]
        [Tooltip("Название сцены, на которой действительны указанные координаты.")] public LocationType LocationName;

        /// <summary> Координаты, в которые должен прийти NPC. </summary>
        [Tooltip("Координаты, в которые должен прийти NPC.")] public Vector3 Position;

        /// <summary> Стандартный угол поворота NPC, при достижении указанных координат. </summary>
        [Tooltip("Стандартный угол поворота NPC, при достижении указанных координат.")] public Vector3 Rotation;

        /// <summary> Название анимационного стейта в аниматоре. </summary>
        [FormerlySerializedAs("_npcAnimationClipName")]
        [FormerlySerializedAs("AnimationClipName")]
        [Header("Other Settings")]
        [Tooltip("Название анимационного стейта в аниматоре.")] public NpcAnimationClipName NpcAnimationClipName;

        /// <summary> Набор диалогов в указанных координатах. </summary>
        [Tooltip("Набор диалогов в указанных координатах.")] public string[] DialoguePool; // TODO: в будущем заменить на спец. класс для диалогов
    }
}