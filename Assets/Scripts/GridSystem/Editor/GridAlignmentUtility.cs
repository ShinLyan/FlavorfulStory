#if UNITY_EDITOR

using FlavorfulStory.GridSystem;
using FlavorfulStory.PlacementSystem;
using UnityEditor;
using UnityEngine;

namespace GridSystem.Editor
{
    /// <summary> Утилита для выравнивания объектов по глобальной сетке. </summary>
    public static class GridAlignmentUtility
    {
        /// <summary> Путь в меню для выравнивания всех размещаемых объектов. </summary>
        private const string AlignPlaceablesPath = "FlavorfulStory/Grid/Align all placeables to grid";

        /// <summary> Путь в меню для выравнивания всех GridView-компонентов. </summary>
        private const string AlignViewsPath = "FlavorfulStory/Grid/Snap GridViews to grid";

        /// <summary> Выравнивает все размещаемые объекты (PlaceableObject) по ячейкам глобальной сетки. </summary>
        [MenuItem(AlignPlaceablesPath)]
        public static void AlignAllPlaceables()
        {
            var grid = GetGlobalGrid();
            if (!grid) return;

            var placeables = Object.FindObjectsByType<PlaceableObject>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            int alignedCount = 0;
            foreach (var placeable in placeables)
            {
                Undo.RecordObject(placeable.transform, "Align Placeable");

                var alignedPosition = GetCellAlignedPosition(grid, placeable.transform.position);

                if (TryGetPropsCollider(placeable, out var props, out var collider))
                {
                    float yOffset = CalculateYOffset(placeable.transform, props, collider);
                    alignedPosition.y -= yOffset;
                }
                else
                {
                    alignedPosition.y = 0f;
                }

                placeable.transform.position = alignedPosition;
                alignedCount++;
            }

            Debug.Log($"Aligned {alignedCount} PlaceableObject(s) to grid.");
        }

        /// <summary> Выравнивает все GridView-компоненты по глобальной сетке. </summary>
        [MenuItem(AlignViewsPath)]
        public static void SnapGridViews()
        {
            var grid = GetGlobalGrid();
            if (!grid) return;

            float cellSizeX = grid.cellSize.x;
            float cellSizeZ = grid.cellSize.z;

            var views = Object.FindObjectsByType<GridView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            int snappedCount = 0;
            foreach (var view in views)
            {
                Undo.RecordObject(view.transform, "Snap GridView");

                var position = view.transform.position;
                position.x = Mathf.Round(position.x / cellSizeX) * cellSizeX;
                position.z = Mathf.Round(position.z / cellSizeZ) * cellSizeZ;

                view.transform.position = position;
                snappedCount++;
            }

            Debug.Log($"Snapped {snappedCount} GridView(s) to grid.");
        }

        /// <summary> Получает ссылку на глобальный Grid в сцене. </summary>
        /// <returns> Ссылка на Grid, если найден; иначе null. </returns>
        private static Grid GetGlobalGrid()
        {
            var grid = Object.FindFirstObjectByType<Grid>();
            if (!grid) Debug.LogError("No global Grid found in scene.");
            return grid;
        }

        /// <summary> Возвращает позицию, выровненную по ячейке грида. </summary>
        /// <param name="grid"> Ссылка на Grid. </param>
        /// <param name="worldPosition"> Мировая позиция для выравнивания. </param>
        /// <returns> Выровненная позиция. </returns>
        private static Vector3 GetCellAlignedPosition(Grid grid, Vector3 worldPosition)
        {
            var cell = grid.WorldToCell(worldPosition);
            return grid.CellToWorld(cell);
        }

        /// <summary> Пытается получить BoxCollider дочернего объекта Props. </summary>
        /// <param name="placeable"> Размещаемый объект. </param>
        /// <param name="props"> Трансформ Props, если найден. </param>
        /// <param name="collider"> Коллайдер, если найден. </param>
        /// <returns> true, если Props и Collider найдены; иначе false. </returns>
        private static bool TryGetPropsCollider(PlaceableObject placeable, out Transform props,
            out BoxCollider collider)
        {
            props = placeable.transform.Find("Props");
            collider = props ? props.GetComponent<BoxCollider>() : null;
            return props && collider;
        }

        /// <summary> Вычисляет смещение по оси Y на основе BoxCollider и локальной позиции Props. </summary>
        /// <param name="root"> Корневой трансформ объекта. </param>
        /// <param name="props"> Трансформ Props. </param>
        /// <param name="collider"> BoxCollider компонента Props. </param>
        /// <returns> Смещение по Y для корректного выравнивания. </returns>
        private static float CalculateYOffset(Transform root, Transform props, BoxCollider collider)
        {
            float propsLocalY = props.localPosition.y;
            float colliderBottom = collider.center.y - collider.size.y / 2f;
            return root.position.y + propsLocalY + colliderBottom;
        }
    }
}

#endif