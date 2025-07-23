using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Хранилище всех доступных SFX-данных. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Audio/SfxDatabase")]
    public class SfxDatabase : ScriptableObject
    {
        /// <summary> Список всех доступных звуковых данных. </summary>
        [Tooltip("Список всех доступных звуковых данных."), SerializeField]
        private List<SfxData> _sfxList;

        /// <summary> Коллекция всех доступных звуковых данных. </summary>
        public IEnumerable<SfxData> SfxList => _sfxList;
    }
}