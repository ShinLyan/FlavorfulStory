using System.Collections.Generic;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Режим размещения объекта из предмета (PlaceableItem). </summary>
    public class PlacementMode : IPlacementMode
    {
        private readonly IGridPositionProvider _positionProvider;
        private readonly PlacementPreview _placementPreview;
        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers;

        public PlaceableObject PlaceableObject { get; set; }

        public PlacementMode(IGridPositionProvider positionProvider, PlacementPreview placementPreview,
            Dictionary<PlacementLayer, PlacementGridData> gridLayers)
        {
            _positionProvider = positionProvider;
            _placementPreview = placementPreview;
            _gridLayers = gridLayers;
        }

        public void Enter() => _placementPreview.StartShowingPlacementPreview(PlaceableObject);

        public void Exit() => _placementPreview.StopShowingPreview();

        public void Apply(Vector3Int gridPosition)
        {
            if (!PlaceableObject || !CanPlace(gridPosition, PlaceableObject.Size, PlaceableObject.Layer))
            {
                SfxPlayer.Play(SfxType.PlacementError);
                return;
            }

            SfxPlayer.Play(SfxType.PlacementSuccess);

            var instance = Object.Instantiate(PlaceableObject.gameObject);
            instance.transform.position = _positionProvider.GridToWorld(gridPosition);

            _gridLayers[PlaceableObject.Layer].AddObjectAt(gridPosition, PlaceableObject.Size, instance);

            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), false);
        }

        private bool CanPlace(Vector3Int position, Vector2Int size, PlacementLayer layer) =>
            _gridLayers[layer].CanPlaceObjectAt(position, size);

        public void Refresh(Vector3Int gridPosition)
        {
            if (!PlaceableObject) return;

            bool isValid = CanPlace(gridPosition, PlaceableObject.Size, PlaceableObject.Layer);
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), isValid);
        }
    }
}