using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpNode
    {
        public int Id { get; private set; }
        public LocationType SceneType { get; private set; }
        public Vector3 Position { get; private set; }
        public List<WarpEdge> Edges { get; private set; }

        public WarpNode(int id,LocationType sceneType, Vector3 position)
        {
            Id = id;
            SceneType = sceneType;
            Position = position;
            Edges = new List<WarpEdge>();
        }

        public void AddEdge(WarpNode targetNode, int duration)
        {
            Edges.Add(new WarpEdge(targetNode, duration));
        }
    }

}