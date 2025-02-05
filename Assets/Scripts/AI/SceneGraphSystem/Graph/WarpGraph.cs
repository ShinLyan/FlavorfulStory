using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpGraph
    {
        public Dictionary<TestSceneType, List<WarpNode>> Nodes { get; private set; }

        public WarpGraph()
        {
            Nodes = new Dictionary<TestSceneType, List<WarpNode>>();
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
        
        
        // Метод для вывода графа в консоль
        public void PrintGraph()
        {
            foreach (var sceneType in Nodes.Keys)
            {
                foreach (var node in Nodes[sceneType])
                {
                    // Выводим информацию о текущем узле
                    Debug.Log($"Node: SceneType = {node.SceneType}, Position = {node.Position}");

                    // Выводим информацию о связанных узлах (ребрах)
                    foreach (var edge in node.Edges)
                    {
                        Debug.Log($"    Connected: {edge.TargetNode.SceneType}, Position = {edge.TargetNode}, Duration = {edge.Duration}");
                    }
                }
            }
        }
    }
}