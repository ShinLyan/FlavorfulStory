using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Данные о размещённых объектах на гриде. </summary>
    public class PlacementGridData
    {
        /// <summary> Словарь размещённых объектов, ключ – позиция клетки, значение – PlaceableObject. </summary>
        private readonly Dictionary<Vector3Int, PlaceableObject> _placedObjects = new();

        /// <summary> Добавляет объект в указанные позиции грида. </summary>
        /// <param name="gridPosition"> Левая нижняя позиция объекта на гриде. </param>
        /// <param name="instance"> Размещаемый объект в сцене. </param>
        public void AddObjectAt(Vector3Int gridPosition, PlaceableObject instance)
        {
            var positions = CalculatePositions(gridPosition, instance.Size);
            foreach (var position in positions) _placedObjects[position] = instance;
        }

        /// <summary> Вычисляет все позиции грида, занимаемые объектом указанного размера. </summary>
        /// <param name="gridPosition"> Левая нижняя позиция объекта на гриде. </param>
        /// <param name="objectSize"> Размер объекта в клетках. </param>
        /// <returns> Список координат клеток, которые занимает объект. </returns>
        private static List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var positions = new List<Vector3Int>(objectSize.x * objectSize.y);
            for (int x = 0; x < objectSize.x; x++)
            for (int y = 0; y < objectSize.y; y++)
                positions.Add(gridPosition + new Vector3Int(x, 0, y));

            return positions;
        }

        /// <summary> Проверяет, можно ли разместить объект в указанной позиции грида. </summary>
        /// <param name="gridPosition"> Левая нижняя позиция объекта на гриде. </param>
        /// <param name="objectSize"> Размер объекта в клетках. </param>
        /// <returns> <c>true</c> – если все клетки свободны; <c>false</c> – если хотя бы одна клетка занята. </returns>
        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var positions = CalculatePositions(gridPosition, objectSize);
            return positions.All(position => !_placedObjects.ContainsKey(position));
        }

        /// <summary> Получает объект, размещённый в указанной позиции грида. </summary>
        /// <param name="gridPosition"> Позиция клетки грида. </param>
        /// <returns> Экземпляр объекта или null, если клетка пуста. </returns>
        public PlaceableObject GetPlacedObject(Vector3Int gridPosition) =>
            _placedObjects.GetValueOrDefault(gridPosition);

        /// <summary> Удаляет объект, размещённый в указанной позиции грида. </summary>
        /// <param name="gridPosition"> Позиция клетки грида, в которой находится объект. </param>
        public void RemoveObjectAt(Vector3Int gridPosition)
        {
            if (!_placedObjects.TryGetValue(gridPosition, out var instance)) return;

            var occupied = _placedObjects.Where(pair => pair.Value == instance).Select(pair => pair.Key).ToList();
            foreach (var pos in occupied) _placedObjects.Remove(pos);
        }
    }
}