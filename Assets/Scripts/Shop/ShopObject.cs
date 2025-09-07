using FlavorfulStory.AI.BaseNpc;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Базовый класс для объектов магазина с функциональностью занятости и доступными позициями. </summary>
    public abstract class ShopObject : MonoBehaviour
    {
        /// <summary> Массив доступных позиций для взаимодействия с объектом. </summary>
        [SerializeField] protected Transform[] _accessiblePositions;

        /// <summary> Анимация, которая будет включаться у объекта для НПС. </summary>
        [field: SerializeField] public AnimationType InteractableObjectAnimation { get; private set; }

        /// <summary> Указывает, занят ли объект в данный момент. </summary>
        public bool IsOccupied { get; set; }

        /// <summary> Возвращает случайную доступную позицию для взаимодействия с объектом. </summary>
        /// <returns> Transform случайной доступной позиции. </returns>
        public virtual NpcDestinationPoint? GetAccessiblePoint()
        {
            var randomPosition = _accessiblePositions[Random.Range(0, _accessiblePositions.Length)];
            return new NpcDestinationPoint(randomPosition.position, randomPosition.rotation);
        }

#if UNITY_EDITOR

        /// <summary> Отрисовывает гизмо объекта при выборе в редакторе. </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Color occupiedColor = new(1f, 0.3f, 0.3f, 1f);
            Color freeColor = new(0.3f, 1f, 0.3f, 1f);
            Color accessPointColor = new(0.3f, 0.3f, 1f, 1f);

            if (_accessiblePositions != null)
            {
                Gizmos.color = accessPointColor;
                foreach (var point in _accessiblePositions)
                    if (point)
                        Gizmos.DrawLine(transform.position, point.position);
            }

            var style = new GUIStyle { normal = { textColor = IsOccupied ? occupiedColor : freeColor } };
            Handles.Label(transform.position + Vector3.up, IsOccupied ? "Occupied" : "Free", style);
        }

#endif
    }
}