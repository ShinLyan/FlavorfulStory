using UnityEngine;

namespace FlavorfulStory.GridSystem
{
    public interface IGridPositionProvider
    {
        bool TryGetCursorGridPosition(out Vector3Int position);

        Vector3 GridToWorld(Vector3Int gridPosition);

        Vector3Int WorldToGrid(Vector3 worldPosition);
    }
}