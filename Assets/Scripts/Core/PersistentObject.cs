using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Глобальный объект, сохраняющийся между сценами (Singleton). </summary>
    public class PersistentObject : MonoBehaviour
    {
        /// <summary> Экземпляр Singleton. </summary>
        public static PersistentObject Instance { get; private set; }

        /// <summary> Инициализация Singleton. </summary>
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary> Получение ссылки на компонент SavingWrapper. </summary>
        /// <returns> Экземпляр SavingWrapper. </returns>
        public SavingWrapper GetSavingWrapper() => GetComponentInChildren<SavingWrapper>();

        /// <summary> Получение ссылки на компонент Fader. </summary>
        /// <returns> Экземпляр Fader. </returns>
        public Fader GetFader() => GetComponentInChildren<Fader>();
    }
}