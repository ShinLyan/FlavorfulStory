using System;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Данные для спавна NPC. </summary>
    [Serializable]
    public class NpcSpawnData
    {
        /// <summary> Точка спавна NPC. </summary>
        [Tooltip("Точка, где NPC будет появляться")]
        public Transform spawnPoint;

        /// <summary> Точка деспавна NPC. </summary>
        [Tooltip("Точка, куда NPC должен прийти для деспавна")]
        public Transform despawnPoint;
    }
}