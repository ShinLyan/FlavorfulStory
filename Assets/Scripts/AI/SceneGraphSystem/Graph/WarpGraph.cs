using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    /// <summary> Граф варпов, представляющий связи между локациями и варпами. </summary>
    public class WarpGraph
    {
        /// <summary> Словарь, хранящий узлы (варпы) по локациям. </summary>
        private readonly Dictionary<LocationName, List<WarpNode>> _nodesByLocation = new();

        /// <summary> Список всех узлов (варпов) в графе. </summary>
        private readonly List<WarpNode> _allNodes = new();

        /// <summary> Добавляет узел (варп) в граф. </summary>
        /// <param name="node"> Узел, который нужно добавить. </param>
        public void AddNode(WarpNode node)
        {
            if (!_nodesByLocation.ContainsKey(node.SourceWarp.ParentLocation))
                _nodesByLocation[node.SourceWarp.ParentLocation] = new List<WarpNode>();

            _nodesByLocation[node.SourceWarp.ParentLocation].Add(node);
            _allNodes.Add(node);
        }

        /// <summary> Находит кратчайший путь между двумя варпами. </summary>
        /// <param name="start"> Начальный варп. </param>
        /// <param name="end"> Конечный варп. </param>
        /// <returns> Список варпов, представляющий путь, или null, если путь не найден. </returns>
        public List<Warp> FindShortestPath(Warp start, Warp end)
        {
            var queue = new Queue<WarpNode>();
            var visited = new HashSet<WarpNode>();
            var path = new Dictionary<WarpNode, WarpNode>();

            var startNode = _allNodes.Find(n => n.SourceWarp == start);
            var endNode = _allNodes.Find(n => n.SourceWarp == end);

            if (startNode == null || endNode == null) return null;

            queue.Enqueue(startNode);
            visited.Add(startNode);
            path[startNode] = null;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == endNode)
                {
                    var finalPath = ReconstructPath(path, endNode);
                    if (finalPath.Count < 2) return finalPath;

                    bool isLastDuplicate = finalPath[^2].ParentLocation == finalPath[^1].ParentLocation;
                    bool isFirstDuplicate = finalPath[0].ParentLocation == finalPath[1].ParentLocation;

                    return isLastDuplicate ? finalPath.GetRange(0, finalPath.Count - 1)
                        : isFirstDuplicate ? finalPath.GetRange(1, finalPath.Count - 1)
                        : finalPath;
                }

                foreach (var edge in current.Edges)
                    if (visited.Add(edge.TargetNode))
                    {
                        path[edge.TargetNode] = current;
                        queue.Enqueue(edge.TargetNode);
                    }
            }

            return null;
        }

        /// <summary> Восстанавливает путь от конечного узла к начальному. </summary>
        /// <param name="path"> Словарь, содержащий связи между узлами. </param>
        /// <param name="endNode"> Конечный узел. </param>
        /// <returns> Список варпов, представляющий путь. </returns>
        private static List<Warp> ReconstructPath(Dictionary<WarpNode, WarpNode> path, WarpNode endNode)
        {
            var result = new List<Warp>();
            var current = endNode;

            while (current != null)
            {
                result.Add(current.SourceWarp);
                current = path[current];
            }

            result.Reverse();
            return result;
        }

        /// <summary> Возвращает все узлы в указанной локации. </summary>
        /// <param name="location"> Локация, для которой нужно получить узлы. </param>
        /// <returns> Список узлов в локации или null, если локация не найдена. </returns>
        private List<WarpNode> GetNodesByLocation(LocationName location)
        {
            return _nodesByLocation.GetValueOrDefault(location);
        }

        /// <summary> Находит ближайший узел к позиции в указанной локации. </summary>
        /// <param name="position"> Позиция для поиска ближайшего узла. </param>
        /// <param name="location"> Локация, в которой искать узел. </param>
        /// <returns> Ближайший узел или null, если узел не найден. </returns>
        public WarpNode FindClosestWarp(Vector3 position, LocationName location)
        {
            var nodes = GetNodesByLocation(location);
            if (nodes == null || nodes.Count == 0) return null;

            WarpNode closestNode = null;
            float minDistance = float.MaxValue;

            foreach (var node in nodes)
            {
                float distance = Vector3.Distance(position, node.SourceWarp.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = node;
                }
            }

            return closestNode;
        }

        /// <summary> Выводит структуру графа в консоль для отладки. </summary>
        public void PrintGraph()
        {
            foreach (var location in _nodesByLocation.Keys)
            foreach (var node in _nodesByLocation[location])
            {
                string connections = "";
                foreach (var edge in node.Edges) connections += $"{edge.TargetNode.SourceWarp.ParentLocation} -> ";
                Debug.Log($"[{location}] Connected to: {connections.TrimEnd(" -> ".ToCharArray())}");
            }
        }
    }
}