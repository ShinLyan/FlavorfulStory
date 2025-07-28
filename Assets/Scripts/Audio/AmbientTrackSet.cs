using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Набор фоновых треков (Ambient), используемый для воспроизведения в игре. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Audio/AmbientTrackSet")]
    public class AmbientTrackSet : ScriptableObject
    {
        /// <summary> Список аудиоклипов, входящих в набор. </summary>
        [field: SerializeField] public List<AudioClip> Clips { get; private set; }
    }
}