using System;
using UnityEngine;

namespace FlavorfulStory.AI.BaseNpc
{
    [Serializable]
    public class DestinationPoint
    {
        public Vector3 Position;

        public Quaternion Rotation;

        public DestinationPoint()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        public DestinationPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}