using System;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    [Serializable]
    public class Warp
    {
        public TestSceneType sceneType;
        public Vector3 position;
        public ConnectedWarp[] connectedWarps;
    }

    [Serializable]
    public class ConnectedWarp
    {
        public TestSceneType _sceneType;
        public int _pathTimeDuration;
    }
}