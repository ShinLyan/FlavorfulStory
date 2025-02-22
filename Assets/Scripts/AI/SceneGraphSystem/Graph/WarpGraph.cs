using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpGraph
    {
        private Dictionary<LocationType, List<WarpNode>> _nodesByLocation = new Dictionary<LocationType, List<WarpNode>>();
        private List<WarpNode> _allNodes = new List<WarpNode>();

        public void AddNode(WarpNode node)
        {
            if (!_nodesByLocation.ContainsKey(node.SourceWarp.ParentLocation))
            {
                _nodesByLocation[node.SourceWarp.ParentLocation] = new List<WarpNode>();
            }
            _nodesByLocation[node.SourceWarp.ParentLocation].Add(node);
            _allNodes.Add(node);
        }

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
                    return ReconstructPath(path, endNode);

                foreach (var edge in current.Edges)
                {
                    if (visited.Add(edge.TargetNode))
                    {
                        path[edge.TargetNode] = current;
                        queue.Enqueue(edge.TargetNode);
                    }
                }
            }
            return null;
        }

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
        
        // Возвращает все узлы в указанной локации
        private List<WarpNode> GetNodesByLocation(LocationType location)
        {
            return _nodesByLocation.GetValueOrDefault(location);
        }

        // Находит ближайший узел к позиции в указанной локации
        public WarpNode FindClosestWarp(Vector3 position, LocationType location)
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

        public void PrintGraph()
        {
            foreach (var location in _nodesByLocation.Keys)
            {
                foreach (var node in _nodesByLocation[location])
                {
                    string connections = "";
                    foreach (var edge in node.Edges)
                    {
                        connections += $"{edge.TargetNode.SourceWarp.ParentLocation} -> ";
                    }
                    Debug.Log($"[{location}] Connected to: {connections.TrimEnd(" -> ".ToCharArray())}");
                }
            }
        }
    }
}