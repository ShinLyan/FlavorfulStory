using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Класс, представляющий варп (точку перехода между локациями). </summary>
    public class Warp : MonoBehaviour
    {
        /// <summary> Связанные точки телепорта. </summary>
        [field: Tooltip("Связанные точки телепорта."), SerializeField]
        public Warp[] ConnectedWarps { get; private set; }

        /// <summary> Локация, к которой принадлежит этот варп. </summary>
        public LocationName ParentLocation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            ParentLocation = GetComponentInParent<Location>().LocationName;
        }

        /// <summary> Отрисовывает Gizmos для визуализации связей варпа. </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.3f);

            // Связи с другими локациями
            foreach (var connected in ConnectedWarps)
                if (connected)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, connected.transform.position);
                }

            // Связи внутри локаций
            var allWarps = FindObjectsByType<Warp>(FindObjectsSortMode.None);
            foreach (var otherWarp in allWarps)
                if (otherWarp.ParentLocation == ParentLocation && otherWarp != this)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, otherWarp.transform.position);
                }
        }
    }
}