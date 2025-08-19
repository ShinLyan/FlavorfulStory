using System.Collections;
using DG.Tweening;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI.Animation;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

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
        [Header("Visualization")]
        [Tooltip("Цвет соединений внутри одной локации"), SerializeField]
        private Color _localConnectionColor = Color.blue;

        /// <summary> Цвет соединений между разными локациями (визуализация в редакторе). </summary>
        [Tooltip("Цвет соединений между разными локациями"), SerializeField]
        private Color _remoteConnectionColor = Color.green;

        /// <summary> Имя локации, к которой принадлежит данный портал. </summary>
        public LocationName ParentLocationName { get; private set; }

        /// <summary> Компонент затемнения экрана при переходах между сценами. </summary>
        private CanvasGroupFader _canvasGroupFader;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Виртуальная камера. </summary>
        private CinemachineCamera _virtualCamera;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="canvasGroupFader"> Компонент затемнения экрана при переходах между сценами. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        /// <param name="virtualCamera"> Виртуальная камера. </param>
        [Inject]
        private void Construct(CanvasGroupFader canvasGroupFader, LocationManager locationManager,
            CinemachineCamera virtualCamera)
        {
            _canvasGroupFader = canvasGroupFader;
            _locationManager = locationManager;
            _virtualCamera = virtualCamera;
        }

        /// <summary> Определение локации портала при инициализации. </summary>
        private void Awake() => ParentLocationName = GetComponentInParent<Location>().LocationName;

        /// <summary> Обработка входа игрока в триггер телепортации. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || !other.TryGetComponent(out PlayerController playerController)) return;

            StartCoroutine(TeleportPlayer(playerController));
        }

        /// <summary> Корутин телепортации игрока с управлением фейдами и сменой локаций. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        private IEnumerator TeleportPlayer(PlayerController playerController) // TODO: Переписать на UniTask
        {
            InputWrapper.BlockAllInput();
            //TODO: Перевести на UIFadeCoordinator или чет такое
            yield return _canvasGroupFader.Show().WaitForCompletion();

            if (_virtualCamera) _virtualCamera.enabled = false;

            playerController.UpdatePosition(ConnectedWarp._spawnPoint);

            yield return null;

            _locationManager.UpdateActiveLocation();
            if (_virtualCamera) _virtualCamera.enabled = true;

            //TODO: Перевести на UIFadeCoordinator или чет такое
            yield return _canvasGroupFader.Hide().WaitForCompletion();
            InputWrapper.UnblockAllInput();
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