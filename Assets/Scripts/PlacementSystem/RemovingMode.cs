using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class RemovingMode : IPlacementMode
    {
        private readonly GridPositionProvider _positionProvider;
        private readonly PlacementPreview _placementPreview;
        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers;

        private static readonly PlacementLayer[] RemovalPriority =
        {
            PlacementLayer.Decoration, // мелкий декор поверх мебели
            PlacementLayer.Furniture, // на полу, коллизия
            PlacementLayer.Wall, // может быть за мебелью
            PlacementLayer.Floor // базовый слой
        };

        public RemovingMode(GridPositionProvider positionProvider, PlacementPreview placementPreview,
            Dictionary<PlacementLayer, PlacementGridData> gridLayers)
        {
            _positionProvider = positionProvider;
            _placementPreview = placementPreview;
            _gridLayers = gridLayers;
        }

        public void Enter() => _placementPreview.StartShowingRemovePreview();

        public void Exit() => _placementPreview.StopShowingPreview();

        public void Apply(Vector3Int gridPosition)
        {
            foreach (var layer in RemovalPriority)
            {
                if (!_gridLayers.TryGetValue(layer, out var gridData)) continue;
                if (gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) continue;

                var target = gridData.GetPlacedObject(gridPosition);
                if (!target) return;

                gridData.RemoveObjectAt(gridPosition);
                Object.Destroy(target);

                SfxPlayer.Play(SfxType.RemoveObject);
                _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition),
                    IsSelectionValid(gridPosition));
                return;
            }

            SfxPlayer.Play(SfxType.PlacementError);
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), false);
        }

        public void Refresh(Vector3Int gridPosition) =>
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition),
                IsSelectionValid(gridPosition));

        private bool IsSelectionValid(Vector3Int gridPosition) =>
            RemovalPriority.Any(layer => !_gridLayers[layer].CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }
}