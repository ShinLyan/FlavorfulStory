#if UNITY_EDITOR

using FlavorfulStory.PlacementSystem;
using UnityEditor;
using UnityEngine;

namespace PlacementSystem.Editor
{
    /// <summary> Утилита редактора для выравнивания всех размещаемых объектов по гриду. </summary>
    public static class AlignPlaceablesToGridEditor
    {
        /// <summary> Путь к пункту меню выравнивания объектов. </summary>
        private const string MenuPath = "FlavorfulStory/Placement/Align all placeables to grid";

        /// <summary> Выровнить все PlaceableObject в сцене по ближайшей ячейке грида.  </summary>
        [MenuItem(MenuPath)]
        public static void AlignAllPlaceables()
        {
            var grid = Object.FindFirstObjectByType<Grid>();
            if (!grid)
            {
                Debug.LogError("No Grid found in scene.");
                return;
            }

            var placeables =
                Object.FindObjectsByType<PlaceableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            int alignedCount = 0;

            foreach (var placeable in placeables)
            {
                if (!placeable) continue;

                Undo.RecordObject(placeable.transform, "Align Placeable To Grid");

                var alignedGridPosition = GetAlignedGridPosition(grid, placeable);

                if (TryGetPropsCollider(placeable, out var props, out var collider))
                {
                    float yOffset = CalculateYOffset(placeable.transform, props, collider);
                    alignedGridPosition.y -= yOffset;
                }
                else
                {
                    alignedGridPosition.y = 0f;
                }

                placeable.transform.position = alignedGridPosition;
                alignedCount++;
            }

            Debug.Log($"Aligned {alignedCount} PlaceableObject(s) to grid.");
        }

        /// <summary> Получить позицию объекта, выровненную по ближайшей ячейке грида. </summary>
        /// <param name="grid"> Ссылка на Grid-компонент сцены. </param>
        /// <param name="placeable"> Объект, который нужно выровнять. </param>
        /// <returns> Мировая позиция, соответствующая ближайшей ячейке грида. </returns>
        private static Vector3 GetAlignedGridPosition(Grid grid, PlaceableObject placeable)
        {
            var worldPos = placeable.transform.position;
            var cellPos = grid.WorldToCell(worldPos);
            return grid.CellToWorld(cellPos);
        }

        /// <summary> Попробовать получить дочерний объект "Props" и получить его BoxCollider. </summary>
        /// <param name="placeable"> Объект размещения. </param>
        /// <param name="props"> Найденный дочерний объект "Props". </param>
        /// <param name="collider"> BoxCollider, если найден. </param>
        /// <returns> True — если Props и BoxCollider найдены; False — иначе. </returns>
        private static bool TryGetPropsCollider(PlaceableObject placeable, out Transform props,
            out BoxCollider collider)
        {
            props = placeable.transform.Find("Props");
            collider = props ? props.GetComponent<BoxCollider>() : null;
            return props && collider;
        }

        /// <summary> Вычисляет смещение по Y для выравнивания объекта по основанию коллайдера. </summary>
        /// <param name="root"> Трансформ корня объекта. </param>
        /// <param name="props"> Трансформ дочернего объекта "Props". </param>
        /// <param name="collider"> Коллайдер, определяющий нижнюю границу. </param>
        /// <returns> Смещение по оси Y. </returns>
        private static float CalculateYOffset(Transform root, Transform props, BoxCollider collider)
        {
            float propsLocalY = props.localPosition.y;
            float colliderBottom = collider.center.y - collider.size.y / 2f;
            return root.position.y + propsLocalY + colliderBottom;
        }
    }
}

#endif