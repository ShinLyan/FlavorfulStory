using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    /// <summary> Класс, представляющий варп (точку перехода между локациями). </summary>
    public class Warp : MonoBehaviour
    {
        /// <summary> Локация, к которой принадлежит этот варп. </summary>
        public LocationType ParentLocation;

        /// <summary> Список варпов, с которыми этот варп связан. </summary>
        public Warp[] ConnectedWarps;

        /// <summary> Длительность перехода через этот варп (в секундах). </summary>
        public int TransitionDuration = 1;

        /// <summary> Включает отрисовку связей внутри локации в редакторе. </summary>
        [Header("Debug")]
        public bool DrawIntraLocationConnections = true;

        /// <summary> Отрисовывает Gizmos для визуализации связей варпа. </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.3f);

            // Связи с другими локациями
            foreach (var connected in ConnectedWarps)
            {
                if (connected != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, connected.transform.position);
                }
            }

            // Связи внутри локаций
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