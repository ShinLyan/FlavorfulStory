using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Глобальный объект, сохраняющийся между сценами (Singleton). </summary>
    public class PersistentObject : MonoBehaviour
    {
        /// <summary> Экземпляр Singleton. </summary>
        public static PersistentObject Instance { get; private set; }

        /// <summary> Ссылка на компонент Fader. </summary>
        public Fader Fader { get; private set; }

        /// <summary> Ссылка на компонент SavingWrapper. </summary>
        public SavingWrapper SavingWrapper { get; private set; }
        
        /// <summary> Инициализация Singleton. </summary>
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Fader = GetComponentInChildren<Fader>();
            SavingWrapper = GetComponentInChildren<SavingWrapper>();
        }
    }
}