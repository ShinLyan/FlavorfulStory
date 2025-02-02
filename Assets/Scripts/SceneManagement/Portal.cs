using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет перемещением игрока между сценами через порталы. </summary>
    public class Portal : MonoBehaviour
    {
        /// <summary> Точка появления игрока после телепортации. </summary>
        [SerializeField] private Transform _spawnPoint;

        /// <summary> Тип сцены, в которую нужно перейти. </summary>
        [SerializeField] private SceneType _sceneToLoad;

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

            StartCoroutine(TeleportPlayer());
        }

        /// <summary> Телепортирует игрока через портал, загружая новую сцену. </summary>
        /// <returns> Корутина для выполнения последовательных действий. </returns>
        private System.Collections.IEnumerator TeleportPlayer()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            InputWrapper.BlockAllInput();
            yield return PersistentObject.Instance.GetFader().FadeOut(Fader.FadeOutTime);

            SavingWrapper.Save();
            yield return SavingWrapper.LoadSceneAsyncByName(_sceneToLoad.ToString());

            SavingWrapper.Load();
            UpdatePlayerPosition(GetOtherPortal());

            SavingWrapper.Save();

            yield return new WaitForSeconds(Fader.FadeWaitTime);
            PersistentObject.Instance.GetFader().FadeIn(Fader.FadeInTime);

            InputWrapper.UnblockAllInput();
            Destroy(gameObject);
        }

        /// <summary> Обновляет позицию и поворот игрока после телепортации. </summary>
        /// <param name="portal"> Портал, в который переместился игрок. </param>
        private void UpdatePlayerPosition(Portal portal)
        {
            var player = GameObject.FindWithTag("Player");
            player.transform.SetPositionAndRotation(portal._spawnPoint.position, portal._spawnPoint.rotation);
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