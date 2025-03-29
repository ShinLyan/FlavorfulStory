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
            if (!_locationToNodes.ContainsKey(node.SourceWarp.ParentLocationName))
                _locationToNodes[node.SourceWarp.ParentLocationName] = new List<WarpNode>();

            _locationToNodes[node.SourceWarp.ParentLocationName].Add(node);
            _allNodes.Add(node);
        }

        /// <summary> Находит кратчайший путь между двумя варп-порталами через поиск в ширину (BFS). </summary>
        /// <param name="start"> Стартовый варп-портал. </param>
        /// <param name="end"> Целевой варп-портал. </param>
        /// <returns> Список варп-порталов пути или null, если путь не найден. </returns>
        public List<WarpPortal> FindShortestPath(WarpPortal start, WarpPortal end)
        {
            var startNode = _allNodes.Find(n => n.SourceWarp == start);
            var endNode = _allNodes.Find(n => n.SourceWarp == end);

            if (startNode == null || endNode == null) return null;

            (bool pathFound, var pathMap) = PerformBFS(startNode, endNode);
            return pathFound
                ? TrimPath(ReconstructPath(pathMap, endNode))
                : null;
        }

        /// <summary> Выполняет обход графа в ширину (BFS) для поиска пути между узлами. </summary>
        /// <param name="startNode"> Начальный узел графа. </param>
        /// <param name="endNode"> Целевой узел графа. </param>
        /// <returns> Кортеж: 
        /// - found: флаг успешности поиска пути,
        /// - pathMap: словарь связей для восстановления пути. </returns>
        private (bool found, Dictionary<WarpNode, WarpNode> pathMap) PerformBFS(WarpNode startNode, WarpNode endNode)
        {
            var queue = new Queue<WarpNode>();
            var visited = new HashSet<WarpNode>();
            var path = new Dictionary<WarpNode, WarpNode>();

            queue.Enqueue(startNode);
            visited.Add(startNode);
            path[startNode] = null;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == endNode) return (true, path);

                foreach (var edge in current.Edges)
                    if (visited.Add(edge.TargetNode))
                    {
                        path[edge.TargetNode] = current;
                        queue.Enqueue(edge.TargetNode);
                    }
            }

            return (false, null);
        }

        /// <summary> Удаляет дублирующиеся локации в начале или конце пути. </summary>
        /// <param name="path"> Исходный путь для обработки. </param>
        /// <returns> Оптимизированный путь без избыточных локаций. </returns>
        private List<WarpPortal> TrimPath(List<WarpPortal> path)
        {
            if (path.Count < 2) return path;

            bool isLastDuplicate = path[^2].ParentLocationName == path[^1].ParentLocationName;
            bool isFirstDuplicate = path[0].ParentLocationName == path[1].ParentLocationName;

            return isLastDuplicate ? path.GetRange(0, path.Count - 1)
                : isFirstDuplicate ? path.GetRange(1, path.Count - 1)
                : path;
        }


        /// <summary> Восстанавливает путь от конечного узла к начальному. </summary>
        /// <param name="path"> Словарь, содержащий связи между узлами. </param>
        /// <param name="endNode"> Конечный узел. </param>
        /// <returns> Список варпов, представляющий путь. </returns>
        private static List<WarpPortal> ReconstructPath(Dictionary<WarpNode, WarpNode> path, WarpNode endNode)
        {
            var result = new List<WarpPortal>();
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
        private List<WarpNode> GetNodesByLocation(LocationName location) =>
            _locationToNodes.GetValueOrDefault(location);

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

        /// <summary> Строит граф варпов на основе списка всех варпов. </summary>
        /// <param name="allWarps">Список всех варпов для построения графа.</param>
        /// <returns>Построенный граф варпов.</returns>
        public static WarpGraph Build(IEnumerable<WarpPortal> allWarps)
        {
            var warps = allWarps.ToList();
            var locationToWarps = GroupWarpsByLocation(warps);
            var (warpToNode, graph) = CreateNodesAndMap(locationToWarps);

            ConnectIntraLocationEdges(locationToWarps, warpToNode);
            ConnectInterLocationEdges(warps, warpToNode);

            return graph;
        }

        /// <summary> Группирует варпы по их родительским локациям. </summary>
        private static Dictionary<LocationName, List<WarpPortal>> GroupWarpsByLocation(List<WarpPortal> warps)
        {
            var locationMap = new Dictionary<LocationName, List<WarpPortal>>();

            foreach (var warp in warps)
            {
                if (!locationMap.ContainsKey(warp.ParentLocationName))
                    locationMap[warp.ParentLocationName] = new List<WarpPortal>();

                locationMap[warp.ParentLocationName].Add(warp);
            }

            return locationMap;
        }

        /// <summary> Создает узлы графа и карту соответствия варпов узлам. </summary>
        private static (Dictionary<WarpPortal, WarpNode>, WarpGraph) CreateNodesAndMap(
            Dictionary<LocationName, List<WarpPortal>> locationMap)
        {
            var warpToNode = new Dictionary<WarpPortal, WarpNode>();
            var graph = new WarpGraph();

            foreach (var warpsInLocation in locationMap.Values)
            foreach (var warp in warpsInLocation)
            {
                var node = new WarpNode(warp);
                warpToNode[warp] = node;
                graph.AddNode(node);
            }

            return (warpToNode, graph);
        }

        /// <summary> Создает связи между варпами внутри одной локации. </summary>
        private static void ConnectIntraLocationEdges(
            Dictionary<LocationName, List<WarpPortal>> locationMap,
            Dictionary<WarpPortal, WarpNode> warpToNode)
        {
            foreach (var warpsInLocation in locationMap.Values)
            foreach (var warpA in warpsInLocation)
            foreach (var warpB in warpsInLocation.Where(warpB => warpA != warpB))
                warpToNode[warpA].Edges.Add(new WarpEdge(warpToNode[warpB]));
        }

        /// <summary> Создает связи между варпами разных локаций. </summary>
        private static void ConnectInterLocationEdges(List<WarpPortal> warps,
            Dictionary<WarpPortal, WarpNode> warpToNode)
        {
            foreach (var warp in warps)
                if (warp.ConnectedWarp != null && warpToNode.TryGetValue(warp.ConnectedWarp, out var targetNode))
                    warpToNode[warp].Edges.Add(new WarpEdge(targetNode));
        }
    }
}