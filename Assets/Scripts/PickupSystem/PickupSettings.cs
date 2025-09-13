using UnityEngine;

namespace FlavorfulStory.PickupSystem
{
    /// <summary> Настройки фейдов HUD и фонового затемнения. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/PickupSettings")]
    public class PickupSettings : ScriptableObject
    {
        /// <summary> Задержка активации подбора предмета. </summary>
        /// <remarks> В секундах. </remarks>
        [field: SerializeField, Tooltip("Задержка активации подбора предмета.")]
        public float PickupActivationDelay { get; private set; }
        
        /// <summary> Задержка активации магнетизма предмета. </summary>
        /// <remarks> В секундах. </remarks>
        [field: SerializeField, Tooltip("Задержка активации магнетизма предмета.")]
        public float MagnetActivationDelay { get; private set; }
        
        /// <summary> Дальность магнетизма. </summary>
        /// <remarks> В тайлах <c>GridSystem.CellSize</c>. </remarks>
        [field: SerializeField, Tooltip("Дальность магнетизма.")]
        public int MagnetRangeTiles {get; private set;}
        
        /// <summary> Скорость магнетизма. </summary>
        /// <remarks> В тайлах <c>GridSystem.CellSize</c>. </remarks>
        [field: SerializeField, Tooltip("Скорость магнетизма.")]
        public int MagnetSpeedTiles {get; private set;}
        
        /// <summary> Задержка активации подбора предмета. </summary>
        /// <remarks> В секундах. </remarks>
        [field: SerializeField, Tooltip("Период опроса магнетизма.")]
        public float MagnetCheckIntervalSeconds { get; private set; }
    }
}