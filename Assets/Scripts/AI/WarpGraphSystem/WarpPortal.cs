using System.Collections;
using FlavorfulStory.Control;
using FlavorfulStory.InputSystem;
using FlavorfulStory.SceneManagement;
using Unity.Cinemachine;
using UnityEngine;

namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Объединенный класс для управления телепортами и связями между локациями </summary>
    public class WarpPortal : MonoBehaviour
    {
        /// <summary> Точка, куда будет телепортирован персонаж при переходе. </summary>
        [Header("Teleport Settings")]
        [Tooltip("Точка, куда будет телепортирован персонаж при переходе."), SerializeField]
        private Transform _spawnPoint;

        /// <summary> Связанная точка телепорта (целевой портал). </summary>
        [field: Tooltip("Связанная точка телепорта."), SerializeField]
        public WarpPortal ConnectedWarp { get; private set; }

        /// <summary> Цвет соединений внутри одной локации (визуализация в редакторе). </summary>
        [Header("Visualization")] [Tooltip("Цвет соединений внутри одной локации"), SerializeField]
        private Color _localConnectionColor = Color.blue;

        /// <summary> Цвет соединений между разными локациями (визуализация в редакторе). </summary>
        [Tooltip("Цвет соединений между разными локациями"), SerializeField]
        private Color _remoteConnectionColor = Color.green;

        /// <summary> Имя локации, к которой принадлежит данный портал. </summary>
        public LocationName ParentLocationName { get; private set; }

        /// <summary> Виртуальная камера. </summary>
        private CinemachineVirtualCamera _virtualCamera;

        /// <summary> Определение локации портала при инициализации. </summary>
        private void Awake()
        {
            ParentLocationName = GetComponentInParent<Location>().LocationName;
            _virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        }

        /// <summary> Обработка входа игрока в триггер телепортации. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            StartCoroutine(TeleportPlayer(other.GetComponent<PlayerController>()));
        }

        /// <summary> Корутин телепортации игрока с управлением фейдами и сменой локаций. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        private IEnumerator TeleportPlayer(PlayerController playerController)
        {
            InputWrapper.BlockAllInput();
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);

            if (_virtualCamera != null) _virtualCamera.enabled = false;

            LocationChanger.EnableLocation(ConnectedWarp.ParentLocationName);
            playerController.UpdatePosition(ConnectedWarp._spawnPoint);

            yield return null;

            if (_virtualCamera != null) _virtualCamera.enabled = true;

            yield return new WaitForSeconds(Fader.FadeWaitTime);
            PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);
            InputWrapper.UnblockAllInput();

            LocationChanger.DisableLocation(ParentLocationName);
        }

#if UNITY_EDITOR
        /// <summary> Визуализация соединений между порталами в редакторе. </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
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
#endif
    }
}