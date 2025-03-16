using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Граф варпов, представляющий связи между локациями и варпами. </summary>
    public class WarpGraph
    {
        /// <summary> Список всех узлов (варпов) в графе. </summary>
        private readonly List<WarpNode> _allNodes = new();

        /// <summary> Словарь, хранящий узлы (варпы) по локациям. </summary>
        private readonly Dictionary<LocationName, List<WarpNode>> _locationToNodes = new();

        /// <summary> Добавляет узел (варп) в граф. </summary>
        /// <param name="node"> Узел, который нужно добавить. </param>
        private void AddNode(WarpNode node)
        {
            if (!_locationToNodes.ContainsKey(node.SourceWarp.ParentLocation))
                _locationToNodes[node.SourceWarp.ParentLocation] = new List<WarpNode>();

            _locationToNodes[node.SourceWarp.ParentLocation].Add(node);
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

                    var isLastDuplicate = finalPath[^2].ParentLocation == finalPath[^1].ParentLocation;
                    var isFirstDuplicate = finalPath[0].ParentLocation == finalPath[1].ParentLocation;

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
            return _locationToNodes.GetValueOrDefault(location);
        }

        /// <summary> Находит ближайший узел к позиции в указанной локации. </summary>
        /// <param name="position"> Позиция для поиска ближайшего узла. </param>
        /// <param name="location"> Локация, в которой искать узел. </param>
        /// <returns> Ближайший узел или null, если узел не найден. </returns>
        public WarpNode FindClosestWarp(Vector3 position, LocationName location)
        {
            var nodes = GetNodesByLocation(location);
            return nodes?.OrderBy(n => Vector3.Distance(position, n.SourceWarp.transform.position))
                .FirstOrDefault();
        }

        /// <summary> Выводит структуру графа в консоль для отладки. </summary>
        public void PrintGraph()
        {
            foreach (var location in _locationToNodes.Keys)
            foreach (var node in _locationToNodes[location])
            {
                var connections = "";
                foreach (var edge in node.Edges) connections += $"{edge.TargetNode.SourceWarp.ParentLocation} -> ";
                Debug.Log($"[{location}] Connected to: {connections.TrimEnd(" -> ".ToCharArray())}");
            }
        }

        /// <summary> Строит граф варпов на основе списка всех варпов. </summary>
        /// <param name="allWarps"> Список всех варпов для построения графа. </param>
        /// <returns> Построенный граф варпов. </returns>
        public static WarpGraph Build(IEnumerable<Warp> allWarps)
        {
            var locationToWarps = new Dictionary<LocationName, List<Warp>>();
            var warps = allWarps.ToList();

            // Группируем варпы по локациям
            foreach (var warp in warps)
            {
                if (!locationToWarps.ContainsKey(warp.ParentLocation))
                    locationToWarps[warp.ParentLocation] = new List<Warp>();

                locationToWarps[warp.ParentLocation].Add(warp);
            }

            var warpToNode = new Dictionary<Warp, WarpNode>();
            var graph = new WarpGraph();

            // Создаем узлы и связи внутри локаций
            foreach (var warpsInLocation in locationToWarps.Keys.Select(location => locationToWarps[location]))
            {
                foreach (var warp in warpsInLocation)
                {
                    var node = new WarpNode(warp);
                    warpToNode[warp] = node;
                    graph.AddNode(node);
                }

                // Связываем все варпы внутри локации между собой
                foreach (var warpA in warpsInLocation)
                foreach (var warpB in warpsInLocation.Where(warpB => warpA != warpB))
                    warpToNode[warpA].Edges.Add(new WarpEdge(warpToNode[warpA], warpToNode[warpB]));
            }

            // Добавляем связи между варпами разных локаций
            foreach (var warp in warps)
                if (warpToNode.TryGetValue(warp.ConnectedWarp, out var targetNode))
                    warpToNode[warp].Edges.Add(new WarpEdge(warpToNode[warp], targetNode));


            return graph;
        }
    }
}