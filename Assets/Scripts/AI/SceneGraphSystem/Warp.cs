using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    // Warp.cs
    public class Warp : MonoBehaviour
    {
        public LocationType ParentLocation;
        public Warp[] ConnectedWarps;
        public int TransitionDuration = 1;

        [Header("Debug")]
        public bool DrawIntraLocationConnections = true;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.3f);

            // связи с другими локациями
            foreach (var connected in ConnectedWarps)
            {
                if (connected != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, connected.transform.position);
                }
            }

            // связи внутри локаций
            if (DrawIntraLocationConnections)
            {
                var allWarps = FindObjectsOfType<Warp>();
                foreach (var otherWarp in allWarps)
                {
                    if (otherWarp.ParentLocation == ParentLocation && otherWarp != this)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(transform.position, otherWarp.transform.position);
                    }
                }
            }
        }
    }
}