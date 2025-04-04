using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Audio/SfxData")]
    public class SfxData : ScriptableObject
    {
        [field: Tooltip("Тип sfx"), SerializeField]
        public SfxType Type { get; private set; }

        [field: Tooltip("Тип sfx"), SerializeField]
        public List<AudioClip> Clips { get; private set; }
    }
}