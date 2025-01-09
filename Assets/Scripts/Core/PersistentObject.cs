using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Альтернатива использованию паттерна Singleton.</summary>
    public class PersistentObject : MonoBehaviour
    {
        /// <summary> Синглтон.</summary>
        public static PersistentObject Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary> Получить ссылку на SavingWrapper.</summary>
        /// <returns> Возвращает ссылку на SavingWrapper.</returns>
        public SavingWrapper GetSavingWrapper() => GetComponentInChildren<SavingWrapper>();

        /// <summary> Получить ссылку на Fader.</summary>
        /// <returns> Возвращает ссылку на Fader.</returns>
        public Fader GetFader() => GetComponentInChildren<Fader>();
    }
}