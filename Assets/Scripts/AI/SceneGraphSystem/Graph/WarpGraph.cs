using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpGraph
    {
        public Dictionary<LocationType, List<WarpNode>> Nodes { get; private set; }

        public WarpGraph()
        {
            Nodes = new Dictionary<LocationType, List<WarpNode>>();
        }

        public void AddNode(WarpNode node)
        {
            if (!Nodes.ContainsKey(node.SceneType))
            {
                Nodes[node.SceneType] = new List<WarpNode>();
            }
            Nodes[node.SceneType].Add(node);
        }

        public void AddEdge(int fromId, int toId, int duration)
        {
            var fromNode = FindNode(fromId);
            var toNode = FindNode(toId);

            if (fromNode != null && toNode != null)
            {
                fromNode.AddEdge(toNode, duration);
            }
        }

        private WarpNode FindNode(int id)
        {
            foreach (var sceneType in Nodes.Keys)
            {
                foreach (var node in Nodes[sceneType])
                {
                    if (node.Id == id)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public List<WarpNode> FindShortestPath(WarpNode startNode, WarpNode targetNode)
        {
            Dictionary<WarpNode, int> distances = new Dictionary<WarpNode, int>();
            Dictionary<WarpNode, WarpNode> previousNodes = new Dictionary<WarpNode, WarpNode>();
            HashSet<WarpNode> visited = new HashSet<WarpNode>();

            // Очередь с приоритетом на основе SortedSet
            SortedSet<KeyValuePair<int, WarpNode>> priorityQueue = 
                new SortedSet<KeyValuePair<int, WarpNode>>(Comparer<KeyValuePair<int, WarpNode>>.Create((a, b) => 
                    a.Key == b.Key ? a.Value.Id - b.Value.Id : a.Key - b.Key));

            // Инициализация
            foreach (var scene in Nodes.Values)
            {
                foreach (var node in scene)
                {
                    distances[node] = int.MaxValue;
                    previousNodes[node] = null;
                }
            }

            distances[startNode] = 0;
            priorityQueue.Add(new KeyValuePair<int, WarpNode>(0, startNode));

            // Основной алгоритм Dijkstra
            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Min.Value;
                priorityQueue.Remove(priorityQueue.Min);

                if (current == targetNode)
                    break;

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                foreach (var edge in current.Edges)
                {
                    WarpNode neighbor = edge.TargetNode;
                    int newDist = distances[current] + edge.Duration;

                    if (newDist < distances[neighbor])
                    {
                        priorityQueue.Remove(new KeyValuePair<int, WarpNode>(distances[neighbor], neighbor));
                        distances[neighbor] = newDist;
                        previousNodes[neighbor] = current;
                        priorityQueue.Add(new KeyValuePair<int, WarpNode>(newDist, neighbor));
                    }
                }
            }

            // Восстановление пути
            List<WarpNode> path = new List<WarpNode>();
            WarpNode step = targetNode;

            while (step != null)
            {
                path.Add(step);
                step = previousNodes[step];
            }

            path.Reverse();
            return path;
        }

        public void PrintGraph()
        {
            foreach (var sceneType in Nodes.Keys)
            {
                foreach (var node in Nodes[sceneType])
                {
                    Debug.Log($"Node: SceneType = {node.SceneType}, Position = {node.Position}");
                    foreach (var edge in node.Edges)
                    {
                        Debug.Log($"    Connected: {edge.TargetNode.SceneType}, Position = {edge.TargetNode}, Duration = {edge.Duration}");
                    }
                }
            }
        }
    }
}
