using System.Collections;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Movement;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Объединенный класс для управления телепортами и связями между локациями </summary>
    public class WarpPortal : MonoBehaviour
    {
        [Header("Teleport Settings")] [SerializeField]
        private Transform _spawnPoint;

        [field: Tooltip("Связанная точка телепорта."), SerializeField]
        public WarpPortal ConnectedWarp { get; private set; }

        [Header("Visualization")] [Tooltip("Цвет соединений внутри локации"), SerializeField]
        private Color _localConnectionColor = Color.blue;

        [Tooltip("Цвет соединений между локациями"), SerializeField]
        private Color _remoteConnectionColor = Color.green;

        public LocationName ParentLocationName { get; private set; }

        private void Awake()
        {
            ParentLocationName = GetComponentInParent<Location>().LocationName;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            StartCoroutine(TeleportPlayer(other.GetComponent<Rigidbody>(), other.GetComponent<PlayerMover>()));
        }

        private IEnumerator TeleportPlayer(Rigidbody playerRigidbody, PlayerMover playerMover)
        {
            InputWrapper.BlockAllInput();
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);

            LocationChanger.EnableLocation(ConnectedWarp.ParentLocationName);
            ConnectedWarp.UpdatePlayerPosition(playerRigidbody, playerMover);

            yield return new WaitForSeconds(Fader.FadeWaitTime);
            PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);
            InputWrapper.UnblockAllInput();

            LocationChanger.DisableLocation(ParentLocationName);
        }

        private void UpdatePlayerPosition(Rigidbody playerRigidbody, PlayerMover playerMover)
        {
            playerRigidbody.MovePosition(_spawnPoint.position);
            playerMover.SetLookRotation(_spawnPoint.rotation);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.3f);

            // Соединение с парным порталом
            if (ConnectedWarp)
            {
                Gizmos.color = _remoteConnectionColor;
                Gizmos.DrawLine(transform.position, ConnectedWarp.transform.position);
            }

            // Соединения внутри локации
            foreach (var portal in FindObjectsByType<WarpPortal>(FindObjectsSortMode.None))
                if (portal != this && portal.ParentLocationName == ParentLocationName)
                {
                    Gizmos.color = _localConnectionColor;
                    Gizmos.DrawLine(transform.position, portal.transform.position);
                }
        }
    }
}