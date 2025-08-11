using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Хранит данные о размещённых объектах на гриде
    /// и предоставляет методы для проверки и изменения состояния. </summary>
    public class PlacementGridData
    {
        /// <summary> Словарь размещённых объектов, ключ – позиция клетки, значение – данные о размещении. </summary>
        private readonly Dictionary<Vector3Int, PlacementData> _placedObjects = new();

        /// <summary> Добавляет объект в указанные позиции грида. </summary>
        /// <param name="gridPosition"> Левая нижняя позиция объекта на гриде. </param>
        /// <param name="objectSize"> Размер объекта в клетках. </param>
        /// <param name="instance"> Экземпляр объекта в сцене. </param>
        public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, GameObject instance)
        {
            var positions = CalculatePositions(gridPosition, objectSize);
            var data = new PlacementData(positions, instance);
            foreach (var position in positions) _placedObjects[position] = data;
        }

        /// <summary> Вычисляет все позиции грида, занимаемые объектом указанного размера. </summary>
        /// <param name="gridPosition"> Левая нижняя позиция объекта на гриде. </param>
        /// <param name="objectSize"> Размер объекта в клетках. </param>
        /// <returns> Список координат клеток, которые занимает объект. </returns>
        private static List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
        {
            var returnValue = new List<Vector3Int>();
            for (int x = 0; x < objectSize.x; x++)
            for (int y = 0; y < objectSize.y; y++)
                returnValue.Add(gridPosition + new Vector3Int(x, 0, y));

            return returnValue;
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
        public GameObject GetPlacedObject(Vector3Int gridPosition) =>
            _placedObjects.TryGetValue(gridPosition, out var placementData) ? placementData.Instance : null;

        /// <summary> Удаляет объект, размещённый в указанной позиции грида. </summary>
        /// <param name="gridPosition"> Позиция клетки грида, в которой находится объект. </param>
        public void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var position in _placedObjects[gridPosition].OccupiedPositions) _placedObjects.Remove(position);
        }
    }

    /// <summary> Данные о размещении объекта на гриде. </summary>
    public class PlacementData // TODO: ВОЗМОЖНО УДАЛИТЬ
    {
        /// <summary> Список клеток грида, которые занимает объект. </summary>
        public List<Vector3Int> OccupiedPositions { get; }

        /// <summary> Экземпляр объекта в сцене. </summary>
        public GameObject Instance { get; }

        /// <summary> Создаёт данные о размещении объекта. </summary>
        /// <param name="occupiedPositions"> Клетки грида, которые занимает объект. </param>
        /// <param name="instance"> Экземпляр объекта в сцене. </param>
        public PlacementData(List<Vector3Int> occupiedPositions, GameObject instance)
        {
            OccupiedPositions = occupiedPositions;
            Instance = instance;
        }
    }
}