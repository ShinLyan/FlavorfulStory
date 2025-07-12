using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class CashDesk : ShopObject //TODO: заглушка
    {
        private Dictionary<Transform, bool> _accessPointsAvailability;

        private readonly Color _occupiedPointColor = new(1f, 0.3f, 0.3f, 0.8f); // Красный для занятых
        private readonly Color _freePointColor = new(0.3f, 1f, 0.3f, 0.8f); // Зелёный для свободных
        private readonly float _pointLabelHeight = 0.5f; // Высота надписи над точкой
        private readonly int _labelFontSize = 11; // Размер шрифта


        protected void Start() { InitializeAccessPoints(); }

        private void InitializeAccessPoints()
        {
            _accessPointsAvailability = new Dictionary<Transform, bool>();
            foreach (var point in _accessiblePositions) _accessPointsAvailability.Add(point, false);
        }

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

        public void ReleasePoint(Transform point)
        {
            if (point) _accessPointsAvailability[point] = false;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos(); // Базовая визуализация из ShopObject

            if (_accessiblePositions == null) return;

            // Создаем стиль для текста один раз
            GUIStyle labelStyle = new GUIStyle
            {
                fontSize = _labelFontSize,
                normal = new GUIStyleState { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                richText = true // Для возможности использования цветного текста
            };

            foreach (var point in _accessiblePositions)
            {
                if (point == null) continue;

                // Определяем статус точки
                bool isOccupied = _accessPointsAvailability != null &&
                                  _accessPointsAvailability.ContainsKey(point) &&
                                  _accessPointsAvailability[point];

                // Рисуем линию к точке
                Gizmos.color = isOccupied ? _occupiedPointColor : _freePointColor;
                Gizmos.DrawLine(transform.position, point.position);

                string statusText = isOccupied ? "<color=#ff3333>Occupied</color>" : "<color=#33ff33>Free</color>";
                Vector3 labelPosition = point.position + Vector3.up * _pointLabelHeight;

                Handles.Label(labelPosition, statusText, labelStyle);
            }
        }
    }
}