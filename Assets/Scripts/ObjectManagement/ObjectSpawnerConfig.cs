using UnityEngine;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Конфигурация спавнера объектов. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Environment/ObjectSpawnerConfig", fileName = "SpawnerConfig")]
    public class ObjectSpawnerConfig : ScriptableObject
    {
        /// <summary> Префаб объекта для спавна. </summary>
        [field: Tooltip("Префаб объекта для спавна."), SerializeField]
        public GameObject ObjectPrefab { get; private set; }

        /// <summary> Количество объектов для спавна. </summary>
        [field: Tooltip("Количество объектов для спавна."), SerializeField, Range(1f, 100f)]
        public int Quantity { get; private set; }

        /// <summary> Ширина, в пределах которой будут заспавнены объекты. </summary>
        [field: Header("Область спавна")]
        [field: Tooltip("Ширина, в пределах которой будут заспавнены объекты."), SerializeField, Range(0, 1000f)]
        public int Width { get; private set; }

        /// <summary> Длина, в пределах которой будут заспавнены объекты. </summary>
        [field: Tooltip("Длина, в пределах которой будут заспавнены объекты."), SerializeField, Range(0, 1000f)]
        public int Length { get; private set; }

        // <summary> Минимальное расстояние между объектами. </summary>
        [field: Tooltip("Минимальное расстояние между объектами."), SerializeField, Range(1f, 50f)]
        public int MinSpacing { get; private set; }

        /// <summary> Равномерное распределение объектов по сетке. </summary>
        [field: Tooltip("Равномерное распределение. Объекты заспавнятся по автоматической сетке."), SerializeField]
        public bool EvenSpread { get; private set; }
    }
}