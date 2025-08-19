using System;
using UnityEngine;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Точка назначения для перемещения NPC. </summary>
    [Serializable]
    public readonly struct NpcDestinationPoint
    {
        /// <summary> Позиция точки назначения. </summary>
        public Vector3 Position { get; }

        /// <summary> Поворот в точке назначения. </summary>
        public Quaternion Rotation { get; }

        /// <summary> Создает точку назначения с заданными параметрами. </summary>
        /// <param name="position"> Позиция точки назначения. </param>
        /// <param name="rotation"> Поворот в точке назначения. </param>
        public NpcDestinationPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}