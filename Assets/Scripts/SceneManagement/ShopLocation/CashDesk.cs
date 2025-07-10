using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class CashDesk : ShopObject //TODO: заглушка
    {
        private Dictionary<Transform, bool> _accessPointsAvailability;

        protected void Start() { InitializeAccessPoints(); }

        private void InitializeAccessPoints()
        {
            _accessPointsAvailability = new Dictionary<Transform, bool>();
            foreach (var point in _accessiblePositions) _accessPointsAvailability.Add(point, false);
        }

        public override Vector3 GetAccessiblePoint()
        {
            var freePoints = _accessPointsAvailability
                .Where(x => !x.Value)
                .Select(x => x.Key)
                .ToList();

            if (freePoints.Count == 0) return Vector3.zero;

            var randomPoint = freePoints[Random.Range(0, freePoints.Count)];
            _accessPointsAvailability[randomPoint] = true;
            return randomPoint.position;
        }

        public void ReleasePoint(Vector3 position)
        {
            var point = _accessPointsAvailability.Keys
                .FirstOrDefault(p => p.position == position);

            if (point) _accessPointsAvailability[point] = false;
        }
    }
}