using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        /// <summary> Занимаемый размер объекта в тайлах. </summary>
        [field: SerializeField, Tooltip("Занимаемый размер объекта в тайлах.")]
        public Vector2Int Size { get; private set; } = Vector2Int.one;

        /// <summary> Слой, на котором размещается объект. </summary>
        [field: SerializeField, Tooltip("Слой, на котором размещается объект.")]
        public PlacementLayer Layer { get; private set; }

        /// <summary> Является ли объект препятствием (непроходимый)? </summary>
        public bool IsObstacle => Layer == PlacementLayer.Furniture;

        private void OnDrawGizmos()
        {
            var origin = transform.position;

            for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
            {
                Gizmos.color = (x + y) % 2 == 0
                    ? new Color(0.88f, 0f, 1f, 0.3f)
                    : new Color(1f, 0.68f, 0f, 0.3f);

                var cellCenter = origin + new Vector3(x + 0.5f, 0f, y + 0.5f);
                Gizmos.DrawCube(cellCenter, new Vector3(1f, 0.1f, 1f));
            }
        }

        /// <summary> Можно ли вращать объект при размещении? </summary>
        // [field: SerializeField, Tooltip("Можно ли вращать объект при размещении?")]
        // public bool IsRotatable { get; private set; } = true;

        // [field: SerializeField, Tooltip("Можно ли взаимодействовать с объектом (сундук, магазин и т.д.).")]
        // public bool IsInteractable { get; private set; }

        // [field: SerializeField, Tooltip("Префаб объекта, который будет спавниться.")]
        // public GameObject RuntimePrefab { get; private set; }

        // /// <summary> Установлен ли объект на сцене? </summary>
        // public bool IsPlaced { get; private set; }
        //
        // /// <summary> Метод вызывается при окончательной установке объекта в мир. </summary>
        // public void ConfirmPlacement()
        // {
        //     IsPlaced = true;
        //     // отключить коллайдеры-снапперы, включить физику, добавить в save и т.д.
        // }
        //
        // /// <summary> Метод вызывается при предпросмотре (ещё не размещён). </summary>
        // public void SetPreviewMode(bool isPreview)
        // {
        //     // покрасить материалы в зелёный/красный, отключить взаимодействия и т.д.
        // }
        //
        // /// <summary> Возвращает мировые координаты всех занимаемых тайлов. </summary>
        // public Vector3[] GetOccupiedWorldPositions()
        // {
        //     // расчёт позиций по Size и Transform
        //     return default;
        // }
    }
}