using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Данные для воспроизведения звуковых эффектов (SFX). </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Audio/SfxData")]
    public class SfxData : ScriptableObject
    {
        /// <summary> Тип звукового эффекта. </summary>
        [field: Tooltip("Тип звукового эффекта."), SerializeField]
        public SfxType Type { get; private set; }

        /// <summary> Список звуковых клипов, соответствующих данному типу. </summary>
        [field: Tooltip("Список звуковых клипов, соответствующих данному типу."), SerializeField]
        public List<AudioClip> Clips { get; private set; }
    }
}