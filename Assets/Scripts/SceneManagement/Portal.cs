using System;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Movement;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет перемещением игрока между сценами через порталы. </summary>
    public class Portal : MonoBehaviour
    {
        /// <summary> Точка появления игрока после телепортации. </summary>
        [SerializeField] private Transform _spawnPoint;

        [SerializeField] private LocationType _parentScene;

        /// <summary> Тип сцены, в которую нужно перейти. </summary>
        [SerializeField] private LocationType _sceneToLoad;

        /// <summary> Идентификатор назначения портала. </summary>
        [SerializeField] private DestinationIdentifier _destination;

        /// <summary> Возможные идентификаторы для порталов. </summary>
        private enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }

        /// <summary> Срабатывает при входе объекта в триггер портала. </summary>
        /// <param name="other"> Коллайдер объекта, входящего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            StartCoroutine(TeleportPlayer(other.GetComponent<Rigidbody>(), other.GetComponent<PlayerMover>()));
        }

        /// <summary> Телепортирует игрока через портал, загружая новую сцену. </summary>
        /// <returns> Корутина для выполнения последовательных действий. </returns>
        private System.Collections.IEnumerator TeleportPlayer(Rigidbody playerRigidbody, PlayerMover playerMover)
        {
            InputWrapper.BlockAllInput();

            yield return PersistentObject.Instance.GetFader().FadeOut(Fader.FadeOutTime);

            LocationChanger.EnableLocation(_sceneToLoad);

            UpdatePlayerPosition(GetOtherPortal(), playerRigidbody, playerMover);

            yield return new WaitForSeconds(Fader.FadeWaitTime);
            PersistentObject.Instance.GetFader().FadeIn(Fader.FadeInTime);

            InputWrapper.UnblockAllInput();
            LocationChanger.DisableLocation(_parentScene);
        }

        /// <summary> Обновляет позицию и поворот игрока после телепортации. </summary>
        /// <param name="portal"> Портал, в который переместился игрок. </param>
        private void UpdatePlayerPosition(Portal portal, Rigidbody playerRigidbody, PlayerMover playerMover)
        {
            playerRigidbody.MovePosition(portal._spawnPoint.transform.position);
            playerMover.SetLookRotation(portal._spawnPoint.transform.rotation);
        }

        /// <summary> Находит другой портал с тем же идентификатором назначения. </summary>
        /// <returns> Портал с совпадающим идентификатором или null, если такого нет. </returns>
        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                if (portal == this || portal._destination != _destination) continue;
                return portal;
            }

            return null;
        }
    }
}