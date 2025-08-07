using System.Collections.Generic;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Режим размещения объекта. </summary>
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

        public void Refresh(Vector3Int gridPosition)
        {
            if (!PlaceableObject) return;

            bool isValid = CanPlace(gridPosition, PlaceableObject.Size, PlaceableObject.Layer);
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), isValid);
        }

        private bool CanPlace(Vector3Int position, Vector2Int size, PlacementLayer layer) =>
            _gridLayers[layer].CanPlaceObjectAt(position, size) &&
            (layer == PlacementLayer.Floor || !HasBlockingColliders(position, size));

        private bool HasBlockingColliders(Vector3Int position, Vector2Int size)
        {
            var worldPosition = _positionProvider.GridToWorld(position);
            var center = worldPosition + new Vector3(size.x / 2f, 1f, size.y / 2f);
            var halfExtents = new Vector3(size.x / 2f, 1.5f, size.y / 2f);

            var colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
            foreach (var collider in colliders)
            {
                if (collider.isTrigger ||
                    collider.GetComponentInParent<PlacementPreview>() ||
                    (collider.TryGetComponent<PlaceableObject>(out var obj) && obj.Layer == PlacementLayer.Floor) ||
                    collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    continue;

                return true;
            }

            return false;
        }
    }
}