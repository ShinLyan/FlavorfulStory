using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory
{
    public class PlacementState : IBuildingState
    {
        private readonly int _selectedObjectIndex;
        private readonly Grid _grid;
        private readonly PreviewSystem _previewSystem;
        private readonly ObjectsDatabase _objectsDatabase;
        private readonly GridData _floorData;
        private readonly GridData _furnitureData;
        private readonly ObjectPlacer _objectPlacer;

        public PlacementState(int index, Grid grid, PreviewSystem previewSystem, ObjectsDatabase objectsDatabase,
            GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
        {
            _selectedObjectIndex = index;
            _grid = grid;
            _previewSystem = previewSystem;
            _objectsDatabase = objectsDatabase;
            _floorData = floorData;
            _furnitureData = furnitureData;
            _objectPlacer = objectPlacer;

            if (_selectedObjectIndex < 0 || _selectedObjectIndex >= _objectsDatabase.Count) return;

            var placeable = _objectsDatabase.GetByIndex(_selectedObjectIndex);
            _previewSystem.StartShowingPlacementPreview(placeable.gameObject, placeable.Size);
        }

        public void EndState() => _previewSystem.StopShowingPreview();

        public void OnAction(Vector3Int gridPosition)
        {
            var placeable = _objectsDatabase.GetByIndex(_selectedObjectIndex);
            if (!CheckPlacementValidity(gridPosition, placeable.Size))
            {
                SfxPlayer.Play(SfxType.PlacementError);
                return;
            }

            SfxPlayer.Play(SfxType.PlacementSuccess);

            int index = _objectPlacer.PlaceObject(placeable.gameObject, _grid.CellToWorld(gridPosition));

            var selectedData = placeable.IsObstacle ? _furnitureData : _floorData;
            selectedData.AddObjectAt(gridPosition, placeable.Size, placeable.GetInstanceID(), index);

            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
        {
            var placeable = _objectsDatabase.GetByIndex(_selectedObjectIndex);
            var selectedData = placeable.IsObstacle ? _furnitureData : _floorData;
            return selectedData.CanPlaceObjectAt(gridPosition, size);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            var placeable = _objectsDatabase.GetByIndex(_selectedObjectIndex);
            bool placementValidity = CheckPlacementValidity(gridPosition, placeable.Size);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}