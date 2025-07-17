using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Касса магазина с управлением доступными точками обслуживания покупателей. </summary>
    public class CashDesk : ShopObject
    {
        /// <summary> Словарь доступности точек доступа к кассе. </summary>
        private Dictionary<Transform, bool> _accessPointsAvailability;

        /// <summary> Инициализирует точки доступа к кассе при запуске. </summary>
        protected void Start() => InitializeAccessPoints();

        /// <summary> Инициализирует словарь доступности точек доступа, устанавливая все точки как свободные. </summary>
        private void InitializeAccessPoints()
        {
            _accessPointsAvailability = new Dictionary<Transform, bool>();
            foreach (var point in _accessiblePositions) _accessPointsAvailability.Add(point, false);
        }

        /// <summary> Возвращает случайную свободную точку доступа и помечает её как занятую. </summary>
        /// <returns> Transform свободной точки доступа или null, если все точки заняты. </returns>
        public override Transform GetAccessiblePoint()
        {
            var freePoints = _accessPointsAvailability
                .Where(x => !x.Value)
                .Select(x => x.Key)
                .ToList();

            if (freePoints.Count == 0) return null;

            var randomPoint = freePoints[Random.Range(0, freePoints.Count)];
            _accessPointsAvailability[randomPoint] = true;
            return randomPoint;
        }

        /// <summary> Освобождает указанную точку доступа, делая её доступной для использования. </summary>
        /// <param name="point"> Transform точки доступа для освобождения. </param>
        public void ReleasePoint(Transform point)
        {
            if (point) _accessPointsAvailability[point] = false;
        }

#if UNITY_EDITOR
        /// <summary> Отрисовывает гизмо кассы с детализированной информацией о состоянии каждой точки доступа. </summary>
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (_accessiblePositions == null) return;

            // Константы для гизмо
            Color occupiedPointColor = new(1f, 0.3f, 0.3f, 0.8f);
            Color freePointColor = new(0.3f, 1f, 0.3f, 0.8f);
            const float pointLabelHeight = 0.5f;
            const int labelFontSize = 11;

            GUIStyle labelStyle = new GUIStyle
            {
                fontSize = labelFontSize,
                normal = new GUIStyleState { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            foreach (var point in _accessiblePositions)
            {
                if (!point) continue;

                bool isOccupied = _accessPointsAvailability != null &&
                                  _accessPointsAvailability.ContainsKey(point) &&
                                  _accessPointsAvailability[point];

                Gizmos.color = isOccupied ? occupiedPointColor : freePointColor;
                Gizmos.DrawLine(transform.position, point.position);

                string statusText = isOccupied ? "<color=#ff3333>Occupied</color>" : "<color=#33ff33>Free</color>";
                Vector3 labelPosition = point.position + Vector3.up * pointLabelHeight;

                Handles.Label(labelPosition, statusText, labelStyle);
            }
        }
#endif
    }
}