using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Audio;
using FlavorfulStory.PlacementSystem;
using UnityEngine;

namespace FlavorfulStory
{
    public class RemovingState : IBuildingState
    {
        private int _gameObjectIndex = -1;
        private readonly Grid _grid;
        private readonly PreviewSystem _previewSystem;
        private readonly Dictionary<PlacementLayer, GridData> _gridLayers;
        private readonly ObjectPlacer _objectPlacer;

        private static readonly PlacementLayer[] RemovalPriority =
        {
            PlacementLayer.Decoration, // мелкий декор поверх мебели
            PlacementLayer.Furniture, // на полу, коллизия
            PlacementLayer.Wall, // может быть за мебелью
            PlacementLayer.Floor // базовый слой
        };

        public RemovingState(Grid grid, PreviewSystem previewSystem, Dictionary<PlacementLayer, GridData> gridLayers,
            ObjectPlacer objectPlacer)
        {
            _grid = grid;
            _previewSystem = previewSystem;
            _gridLayers = gridLayers;
            _objectPlacer = objectPlacer;
        }

        public void BeginState() => _previewSystem.StartShowingRemovePreview();

        public void EndState() => _previewSystem.StopShowingPreview();

        public void OnAction(Vector3Int gridPosition)
        {
            foreach (var layer in RemovalPriority)
            {
                var gridData = _gridLayers[layer];

                if (gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) continue;

                SfxPlayer.Play(SfxType.RemoveObject);

                _gameObjectIndex = gridData.GetRepresentationIndex(gridPosition);
                if (_gameObjectIndex == -1) return;

                gridData.RemoveObjectAt(gridPosition);
                _objectPlacer.RemoveObjectAt(_gameObjectIndex);

                _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), IsSelectionValid(gridPosition));
                return;
            }

            SfxPlayer.Play(SfxType.PlacementError);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            bool validity = IsSelectionValid(gridPosition);
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), validity);
        }

        private bool IsSelectionValid(Vector3Int gridPosition) =>
            RemovalPriority.Any(layer => !_gridLayers[layer].CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }
}