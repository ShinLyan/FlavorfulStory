using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory
{
    public class PlacementState : IBuildingState
    {
        private readonly int _selectedObjectIndex;
        private int _id;
        private readonly Grid _grid;
        private readonly PreviewSystem _previewSystem;
        private readonly ObjectsDatabase _objectsDatabase;
        private readonly GridData _floorData;
        private readonly GridData _furnitureData;
        private readonly ObjectPlacer _objectPlacer;

        public PlacementState(int id, Grid grid, PreviewSystem previewSystem, ObjectsDatabase objectsDatabase,
            GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
        {
            _id = id;
            _grid = grid;
            _previewSystem = previewSystem;
            _objectsDatabase = objectsDatabase;
            _floorData = floorData;
            _furnitureData = furnitureData;
            _objectPlacer = objectPlacer;

            _selectedObjectIndex = _objectsDatabase.ObjectsData.FindIndex(data => data.Id == id);
            if (_selectedObjectIndex > -1)
                _previewSystem.StartShowingPlacementPreview(_objectsDatabase.ObjectsData[_selectedObjectIndex].Prefab,
                    _objectsDatabase.ObjectsData[_selectedObjectIndex].Size);
        }

        public void EndState() => _previewSystem.StopShowingPreview();

        public void OnAction(Vector3Int gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
            if (!placementValidity)
            {
                SfxPlayer.Play(SfxType.PlacementError);
                return;
            }

            SfxPlayer.Play(SfxType.PlacementSuccess);

            int index = _objectPlacer.PlaceObject(_objectsDatabase.ObjectsData[_selectedObjectIndex].Prefab,
                _grid.CellToWorld(gridPosition));

            var selectedData = _objectsDatabase.ObjectsData[_selectedObjectIndex].Id == 0 ? _floorData : _furnitureData;
            selectedData.AddObjectAt(gridPosition, _objectsDatabase.ObjectsData[_selectedObjectIndex].Size,
                _objectsDatabase.ObjectsData[_selectedObjectIndex].Id, index);

            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            var selectedData = _objectsDatabase.ObjectsData[selectedObjectIndex].Id == 0 ? _floorData : _furnitureData;
            return selectedData.CanPlaceObjectAt(gridPosition, _objectsDatabase.ObjectsData[selectedObjectIndex].Size);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}