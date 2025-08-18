using System;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Данные для спавна NPC. </summary>
    [Serializable]
    public struct NpcSpawnData
    {
        /// <summary> Точка спавна NPC. </summary>
        [field: Tooltip("Точка, где NPC будет появляться."), SerializeField]
        public Transform SpawnPoint { get; private set; }

        /// <summary> Точка деспавна NPC. </summary>
        [field: Tooltip("Точка, куда NPC должен прийти для деспавна."), SerializeField]
        public Transform DespawnPoint { get; private set; }
    }
}