using UnityEngine;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Конфигурация спавнера объектов. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Environment/ObjectSpawnerConfig", fileName = "SpawnerConfig")]
    public class ObjectSpawnerConfig : ScriptableObject
    {
        /// <summary> Префаб объекта для спавна. </summary>
        [Tooltip("Префаб объекта для спавна.")]
        public GameObject ObjectPrefab;

        /// <summary> Количество объектов для спавна. </summary>
        [Tooltip("Количество объектов для спавна.")] [Range(1f, 100f)]
        public int Quantity;

        [Header("Область спавна")]
        /// <summary> Ширина, в пределах которой будут заспавнены объекты. </summary>
        [Tooltip("Ширина, в пределах которой будут заспавнены объекты.")]
        [Range(0, 1000f)]
        public int Width;

        /// <summary> Длина, в пределах которой будут заспавнены объекты. </summary>
        [Tooltip("Длина, в пределах которой будут заспавнены объекты.")] [Range(0, 1000f)]
        public int Length;

        // <summary> Минимальное расстояние между объектами. </summary>
        [Tooltip("Минимальное расстояние между объектами.")] [Range(1f, 50f)]
        public int MinSpacing;

        /// <summary> Равномерное распределение объектов по сетке. </summary>
        [Tooltip("Равномерное распределение. Объекты заспавнятся по автоматической сетке.")]
        public bool EvenSpread;
    }
}