using System;
using FlavorfulStory.AI.BaseNpc;
using UnityEngine;

namespace FlavorfulStory.AI.Scheduling
{
    /// <summary> Точка расписания для NPC, определяющая, куда и когда он должен переместиться. </summary>
    [Serializable]
    public class NpcSchedulePoint
    {
        /// <summary> Час, в который NPC начнет движение в указанную точку. </summary>
        [field: Header("Start Time")]
        [field: Tooltip("Час, в который NPC начнет движение в указанную точку."), SerializeField, Range(0, 23)]
        public int Hour { get; set; }

        /// <summary> Минута, в которую NPC начнет движение в указанную точку. </summary>
        [field: Tooltip("Минута, в которую NPC начнет движение в указанную точку."), SerializeField, Range(0, 59)]
        public int Minutes { get; set; }

        /// <summary> Название анимации, которая будет воспроизводиться в данной точке. </summary>
        [field: Header("Other Settings")]
        [field: Tooltip("Название анимации, которая будет воспроизводиться в данной точке."), SerializeField]
        public AnimationType NpcAnimation { get; set; }

        /// <summary> Позиция точки назначения. </summary>
        [SerializeField] private Vector3 _position;

        /// <summary> Поворот точки назначения в углах Эйлера. </summary>
        [SerializeField] private Vector3 _rotationEuler;

        /// <summary> Кэшированная точка назначения. </summary>
        [NonSerialized] private NpcDestinationPoint? _cachedPoint;

        /// <summary> Получает точку назначения NPC. </summary>
        public NpcDestinationPoint NpcDestinationPoint
        {
            get
            {
                if (_cachedPoint == null)
                {
                    var rot = Quaternion.Euler(_rotationEuler);
                    if (rot == default) rot = Quaternion.identity;
                    _cachedPoint = new NpcDestinationPoint(_position, rot);
                }

                return _cachedPoint.Value;
            }
        }

        /// <summary> Устанавливает трансформ точки назначения. </summary>
        /// <param name="pos"> Новая позиция. </param>
        /// <param name="rot"> Новый поворот. </param>
        public void SetTransform(Vector3 pos, Quaternion rot)
        {
            _position = pos;
            _rotationEuler = rot.eulerAngles;
            _cachedPoint = new NpcDestinationPoint(_position, rot == default ? Quaternion.identity : rot);
        }

        /// <summary> Сбрасывает кэш при изменении в инспекторе. </summary>
        private void OnValidate() => _cachedPoint = null;
    }
}