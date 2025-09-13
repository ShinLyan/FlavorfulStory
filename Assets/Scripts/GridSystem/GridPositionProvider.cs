using FlavorfulStory.InputSystem;
using FlavorfulStory.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.GridSystem
{
    /// <summary> Провайдер позиции на гриде. </summary>
    public class GridPositionProvider
    {
        /// <summary> Грид. </summary>
        private readonly Grid _grid;

        /// <summary> Слой, на который можно размещать объекты. </summary>
        private static readonly int PlacementLayerMask = LayerMask.GetMask("Placement");

        /// <summary> Половина размера клетки. </summary>
        public static readonly Vector3 CellHalfExtents = new(0.5f, 0.5f, 0.5f);

        /// <summary> Размер клетки в гриде. </summary>
        public const float CellSize = 1f;
        
        /// <summary> Конструктор, принимающий ссылку на компонент Grid. </summary>
        /// <param name="grid"> Сетка, в которой происходит размещение объектов. </param>
        public GridPositionProvider(Grid grid) => _grid = grid;

        /// <summary> Пытается получить координаты грида под указателем мыши. </summary>
        /// <param name="gridPosition"> Результат: позиция в координатах грида. </param>
        /// <returns> True – если позиция успешно получена; False – если указатель над UI или луч
        /// не попал по слою размещения. </returns>
        public bool TryGetCursorGridPosition(out Vector3Int gridPosition)
        {
            gridPosition = default;

            if (EventSystem.current.IsPointerOverGameObject()) return false;

            var mousePosition = InputWrapper.GetMousePosition();
            mousePosition.z = CameraUtils.MainCamera.nearClipPlane;

            var ray = CameraUtils.MainCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out var hit, 100f, PlacementLayerMask)) return false;

            gridPosition = _grid.WorldToCell(hit.point);
            return true;
        }

        /// <summary> Преобразует координаты грида в мировые координаты. </summary>
        /// <param name="gridPosition"> Позиция в гриде. </param>
        /// <returns> Мировые координаты центра ячейки. </returns>
        public Vector3 GridToWorld(Vector3Int gridPosition) => _grid.CellToWorld(gridPosition);

        /// <summary> Преобразует мировые координаты в координаты грида. </summary>
        /// <param name="worldPosition"> Позиция в мировых координатах. </param>
        /// <returns> Позиция в координатах грида. </returns>
        public Vector3Int WorldToGrid(Vector3 worldPosition) => _grid.WorldToCell(worldPosition);

        /// <summary> Получить центр ячейки по её координатам. </summary>
        /// <param name="gridPosition"> Позиция в гриде. </param>
        /// <returns> Центр ячейки по её координатам. </returns>
        public Vector3 GetCellCenterWorld(Vector3Int gridPosition) =>
            GridToWorld(gridPosition) + new Vector3(CellSize, 0f, CellSize) * 0.5f;

        /// <summary> Переводит количество клеток в мировое расстояние. </summary>
        /// <param name="cellsCount"> Количество клеток. </param>
        /// <returns> Мировое расстояние в клетках. </returns>
        public static float CellsToWorldDistance(int cellsCount) => cellsCount * CellSize;
    }
}