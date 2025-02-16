using System;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    [Serializable]
    public class Warp
    {
        public LocationType sceneType;
        public Vector3 position;
        public ConnectedWarp[] connectedWarps;
    }

    [Serializable]
    public class ConnectedWarp
    {
        public LocationType _sceneType;
        public int _pathTimeDuration;
    }
}