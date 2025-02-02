using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Объект, с которым игрок может взаимодействовать. </summary>
    [RequireComponent(typeof(Outline))]
    public class InteractableObject2 : MonoBehaviour
    {
        /// <summary> Компонент, отвечающий за обводку объекта. </summary>
        private Outline _outline;

        /// <summary> Ссылка на компонент для смены внешнего вида. </summary>
        protected AppearanceSwitcher _appearanceSwitcher;

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _outline = GetComponent<Outline>();
            SwitchOutline(false);
            _appearanceSwitcher = GetComponentInParent<AppearanceSwitcher>();
        }

        /// <summary> Выполняет действие взаимодействия с объектом. </summary>
        public virtual void Interact()
        {
            _appearanceSwitcher.ChangeAppearance();
            print("Interacted");
        }

        /// <summary> Включает или отключает обводку объекта. </summary>
        /// <param name="enabled"> true для включения обводки, false для отключения. </param>
        public void SwitchOutline(bool enabled) => _outline.enabled = enabled;
    }
}