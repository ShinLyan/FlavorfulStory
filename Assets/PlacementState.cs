using System.Collections.Generic;
using FlavorfulStory.Audio;
using FlavorfulStory.PlacementSystem;
using UnityEngine;

namespace FlavorfulStory
{
    public class PlacementState : IBuildingState
    {
        private readonly int _selectedObjectIndex;
        private readonly Grid _grid;
        private readonly PreviewSystem _previewSystem;
        private readonly PlaceableObjectsCatalog _placeableObjectsCatalog;
        private readonly Dictionary<PlacementLayer, GridData> _gridLayers;
        private readonly ObjectPlacer _objectPlacer;

        public PlacementState(int index, Grid grid, PreviewSystem previewSystem,
            PlaceableObjectsCatalog placeableObjectsCatalog,
            Dictionary<PlacementLayer, GridData> gridLayers, ObjectPlacer objectPlacer)
        {
            _selectedObjectIndex = index;
            _grid = grid;
            _previewSystem = previewSystem;
            _placeableObjectsCatalog = placeableObjectsCatalog;
            _gridLayers = gridLayers;
            _objectPlacer = objectPlacer;
        }

        public void BeginState()
        {
            var placeable = _placeableObjectsCatalog.GetByIndex(_selectedObjectIndex);
            _previewSystem.StartShowingPlacementPreview(placeable.gameObject, placeable.Size);
        }

        public void EndState() => _previewSystem.StopShowingPreview();

        public void OnAction(Vector3Int gridPosition)
        {
            var placeable = _placeableObjectsCatalog.GetByIndex(_selectedObjectIndex);
            if (!CheckPlacementValidity(gridPosition, placeable.Size))
            {
                SfxPlayer.Play(SfxType.PlacementError);
                return;
            }

            SfxPlayer.Play(SfxType.PlacementSuccess);

            int index = _objectPlacer.PlaceObject(placeable.gameObject, _grid.CellToWorld(gridPosition));
            _gridLayers[placeable.Layer].AddObjectAt(gridPosition, placeable.Size, placeable.GetInstanceID(), index);

            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
        {
            var placeable = _placeableObjectsCatalog.GetByIndex(_selectedObjectIndex);
            return _gridLayers[placeable.Layer].CanPlaceObjectAt(gridPosition, size);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            var placeable = _placeableObjectsCatalog.GetByIndex(_selectedObjectIndex);
            bool placementValidity = CheckPlacementValidity(gridPosition, placeable.Size);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}