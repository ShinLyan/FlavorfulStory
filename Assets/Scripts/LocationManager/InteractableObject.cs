using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Объект для взаимодействия.</summary>
    [RequireComponent(typeof(Outline))]
    public class InteractableObject : MonoBehaviour
    {
        /// <summary> Компонент обводки.</summary>
        private Outline _outline;
        
        /// <summary> Компонент смены внешнего вида.</summary>
        protected AppearanceSwitcher _appearanceSwitcher;

        /// <summary> Получение компонентов.</summary>
        private void Awake()
        {
            _outline = GetComponent<Outline>();
            SwitchOutline(false);
            _appearanceSwitcher = GetComponentInParent<AppearanceSwitcher>();
        }

        /// <summary> Взаимодействие.</summary>
        public virtual void Interact()
        {
            _appearanceSwitcher.ChangeAppearance();
            print("Interacted");
        }

        /// <summary> Включение/выключение обводки.</summary>
        public void SwitchOutline(bool enabled) => _outline.enabled = enabled;
    }
}