using System;
using UnityEngine;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Точка назначения для перемещения NPC. </summary>
    [Serializable]
    public class DestinationPoint
    {
        /// <summary> Позиция точки назначения. </summary>
        public Vector3 Position;

        /// <summary> Поворот в точке назначения. </summary>
        public Quaternion Rotation;

        /// <summary> Создает точку назначения с нулевыми значениями. </summary>
        public DestinationPoint()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        /// <summary> Создает точку назначения с заданными параметрами. </summary>
        /// <param name="position"> Позиция точки назначения. </param>
        /// <param name="rotation"> Поворот в точке назначения. </param>
        public DestinationPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}