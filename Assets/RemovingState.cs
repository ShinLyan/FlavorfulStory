using UnityEngine;

namespace FlavorfulStory
{
    public class RemovingState : IBuildingState
    {
        private int _gameObjectIndex = -1;
        private readonly Grid _grid;
        private readonly PreviewSystem _previewSystem;
        private readonly GridData _floorData;
        private readonly GridData _furnitureData;
        private readonly ObjectPlacer _objectPlacer;

        public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData furnitureData,
            ObjectPlacer objectPlacer)
        {
            _grid = grid;
            _previewSystem = previewSystem;
            _floorData = floorData;
            _furnitureData = furnitureData;
            _objectPlacer = objectPlacer;

            _previewSystem.StartShowingRemovePreview();
        }

        public void EndState() { _previewSystem.StopShowingPreview(); }

        public void OnAction(Vector3Int gridPosition)
        {
            GridData selectedData = null;
            if (!_furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
                selectedData = _furnitureData;
            else if (!_floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) selectedData = _floorData;

            if (selectedData == null)
            {
                //   SfxPlayer.Play(); SfxType.WrongPlacement
            }
            else
            {
                //   SfxPlayer.Play(); SfxType.Remove

                _gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
                if (_gameObjectIndex == -1) return;

                selectedData.RemoveObjectAt(gridPosition);
                _objectPlacer.RemoveObjectAt(_gameObjectIndex);
            }

            var cellPosition = _grid.CellToWorld(gridPosition);
            _previewSystem.UpdatePosition(cellPosition, IsSelectionValid(gridPosition));
        }

        private bool IsSelectionValid(Vector3Int gridPosition) =>
            !(_furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
              _floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));

        public void UpdateState(Vector3Int gridPosition)
        {
            bool validity = IsSelectionValid(gridPosition);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), validity);
        }
    }
}