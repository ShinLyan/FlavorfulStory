using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Структура данных для одного типа SFX. </summary>
    [Serializable]
    public struct SfxData
    {
        /// <summary> Тип звукового эффекта. </summary>
        [field: Tooltip("Тип звукового эффекта."), SerializeField]
        public SfxType Type { get; private set; }

        /// <summary> Список звуковых клипов, соответствующих данному типу. </summary>
        [field: Tooltip("Список звуковых клипов, соответствующих данному типу."), SerializeField]
        public List<AudioClip> Clips { get; private set; }
    }
}