using UnityEngine;
#if UNITY_EDITOR
#endif

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Визуализатор расписания NPC. Отображает маршруты, ключевые точки 
    /// и параметры навигации в редакторе Unity. Позволяет настраивать внешний вид
    /// элементов визуализации и управлять выбранными точками/параметрами. </summary>
    public class NpcScheduleDemonstrator : MonoBehaviour
    {
        /// <summary> Расписание NPC, отображаемое в сцене. Настраивается в инспекторе. </summary>
        [field: SerializeField] public NpcSchedule Schedule { get; private set; }

        /// <summary> Толщина линии, используемой для визуализации маршрута. </summary>
        [field: SerializeField] public float LineThickness { get; private set; }

        /// <summary> Цвет линии маршрута. </summary>
        [field: SerializeField] public Color LineColor { get; private set; }

        /// <summary> Размер сфер, отображающих ключевые точки маршрута. </summary>
        [field: SerializeField] public float SphereSize { get; private set; }

        /// <summary> Отображать ли информационную метку над точками. </summary>
        [field: SerializeField] public bool ShowInfoLabel { get; private set; }

        /// <summary> Индекс выбранного параметра для редактирования. </summary>
        public int SelectedParamIndex { get; private set; }

        /// <summary> Индекс выбранной точки маршрута. </summary>
        public int SelectedPointIndex { get; private set; }

        /// <summary> Высота над уровнем земли для отрисовки элементов. </summary>
        [field: SerializeField]
        public float GroundHeight { get; private set; }

        /// <summary> Маска слоёв, определяющая поверхность для размещения объектов. </summary>
        [field: SerializeField]
        public LayerMask GroundMask { get; private set; }

        /// <summary> Цвет для визуализации зон навигации (NavMesh). </summary>
        [field: SerializeField]
        public Color NavMeshColor { get; private set; }

        /// <summary> Устанавливает новый индекс для выбранного параметра. </summary>
        /// <param name="newVal"> Новое значение индекса. </param>
        public void SetNewValForParamIndex(int newVal) => SelectedParamIndex = newVal;

        /// <summary> Устанавливает новый индекс для выбранной точки маршрута. </summary>
        /// <param name="newVal"> Новое значение индекса. </param>
        public void SetNewValForPointIndex(int newVal) => SelectedPointIndex = newVal;

        /// <summary> Увеличивает индекс выбранной точки маршрута на 1. </summary>
        public void IncrementSelectedPointIndex() => SelectedPointIndex++;

        /// <summary> Уменьшает индекс выбранной точки маршрута на 1. </summary>
        public void DecrementSelectedPointIndex() => SelectedPointIndex--;
    }
}