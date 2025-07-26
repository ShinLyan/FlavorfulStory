using FlavorfulStory.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.GridSystem
{
    public class GridPositionProvider : IGridPositionProvider
    {
        private readonly Camera _camera;
        private readonly Grid _grid;
        private static readonly int PlacementLayerMask = LayerMask.GetMask("Placement");

        public GridPositionProvider(Camera camera, Grid grid)
        {
            _camera = camera;
            _grid = grid;
        }

        /// <summary> Попробовать получить позицию на гриде по указателю мыши. </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool TryGetCursorGridPosition(out Vector3Int gridPosition)
        {
            gridPosition = default;

            if (EventSystem.current.IsPointerOverGameObject()) return false;

            var mousePosition = InputWrapper.GetMousePosition();
            mousePosition.z = _camera.nearClipPlane;

            var ray = _camera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f, PlacementLayerMask)) return false;

            gridPosition = _grid.WorldToCell(hit.point);
            return true;
        }

        public Vector3 GridToWorld(Vector3Int gridPosition) => _grid.CellToWorld(gridPosition);

        public Vector3Int WorldToGrid(Vector3 worldPosition) => _grid.WorldToCell(worldPosition);
    }
}