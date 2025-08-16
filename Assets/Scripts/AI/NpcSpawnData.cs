using System;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Данные для спавна NPC. </summary>
    [Serializable]
    public struct NpcSpawnData
    {
        /// <summary> Точка спавна NPC. </summary>
        [field: SerializeField, Tooltip("Точка, где NPC будет появляться")]
        public Transform SpawnPoint { get; private set; }

        /// <summary> Точка деспавна NPC. </summary>
        [field: SerializeField, Tooltip("Точка, куда NPC должен прийти для деспавна")]
        public Transform DespawnPoint { get; private set; }
    }
}