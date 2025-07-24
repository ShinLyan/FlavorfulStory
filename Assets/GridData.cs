using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    public class GridData
    {
        private readonly Dictionary<Vector3Int, PlacementData> _placedObjects = new();

        public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int id, int placedObjectIndex)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize);
            var data = new PlacementData(positionToOccupy, id, placedObjectIndex);
            foreach (var position in positionToOccupy)
            {
                if (_placedObjects.ContainsKey(position))
                    Debug.LogError($"Dictionary already contains a placed object at {position}");

                _placedObjects[position] = data;
            }
        }

        private static List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var returnValue = new List<Vector3Int>();
            for (int x = 0; x < objectSize.x; x++)
            for (int y = 0; y < objectSize.y; y++)
                returnValue.Add(gridPosition + new Vector3Int(x, 0, y));

            return returnValue;
        }

        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var positionToOccupy = CalculatePositions(gridPosition, objectSize);
            foreach (var position in positionToOccupy)
                if (_placedObjects.ContainsKey(position))
                    return false;
            return true;
        }

        public int GetRepresentationIndex(Vector3Int gridPosition) =>
            _placedObjects.TryGetValue(gridPosition, out var placementData) ? placementData.PlacedObjectIndex : -1;

        public void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var position in _placedObjects[gridPosition].OccupiedPositions) _placedObjects.Remove(position);
        }
    }

    public class PlacementData
    {
        public List<Vector3Int> OccupiedPositions { get; }
        public int Id { get; private set; }
        public int PlacedObjectIndex { get; }

        public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
        {
            OccupiedPositions = occupiedPositions;
            Id = id;
            PlacedObjectIndex = placedObjectIndex;
        }
    }
}