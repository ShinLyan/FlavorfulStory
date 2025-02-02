using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private SceneType _sceneToLoad;
        [SerializeField] private DestinationIdentifier _destination;

        private enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            StartCoroutine(TeleportPlayer());
        }

        /// <summary> ��������������� ������ � ������� ������� � ����� ������. </summary>
        /// <returns> ���������� ��������, ������� �������������. </returns>
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

        private static void UpdatePlayerPosition(Portal portal)
        {
            var player = GameObject.FindWithTag("Player");
            player.transform.SetPositionAndRotation(portal._spawnPoint.position, portal._spawnPoint.rotation);
        }

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