using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class PlacementGridData
    {
        private readonly Dictionary<Vector3Int, PlacementData> _placedObjects = new();

        public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, GameObject instance)
        {
            var positions = CalculatePositions(gridPosition, objectSize);
            var data = new PlacementData(positions, instance);
            foreach (var position in positions) _placedObjects[position] = data;
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
            var positions = CalculatePositions(gridPosition, objectSize);
            return positions.All(position => !_placedObjects.ContainsKey(position));
        }

        public GameObject GetPlacedObject(Vector3Int gridPosition) =>
            _placedObjects.TryGetValue(gridPosition, out var placementData) ? placementData.Instance : null;

        public void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var position in _placedObjects[gridPosition].OccupiedPositions) _placedObjects.Remove(position);
        }
    }

    public class PlacementData
    {
        public List<Vector3Int> OccupiedPositions { get; }
        public GameObject Instance { get; }

        public PlacementData(List<Vector3Int> occupiedPositions, GameObject instance)
        {
            OccupiedPositions = occupiedPositions;
            Instance = instance;
        }
    }
}