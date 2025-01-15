using FlavorfulStory.Stats.CharacterStats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.UI.Bars
{
    /// <summary>  Класс, отвечающий за отрисовку количества здоровья. </summary>
    public class HealthBar : BaseBar, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент здоровья. </summary>
        private Health _health;

        /// <summary> Подписка на события. </summary>
        private void OnEnable()
        {
            _health.OnHealthChanged += SetBarText;
        }

        /// <summary> Отписка от события. </summary>
        private void OnDisable()
        {
            _health.OnHealthChanged -= SetBarText;
        }

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _health = player.GetComponent<Health>();
        }

        private void Start()
        {
            SetBarText(_health.CurrentValue);
            _textObject.gameObject.SetActive(false);
        }
        
        /// <summary> Включение объекта при наведении на него курсора. </summary>
        /// <param name="eventData"> Информация ивента. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBarText(_health.CurrentValue);
            _textObject.gameObject.SetActive(true);
        }
        
        /// <summary> Выключение объекта при наведении на него курсора. </summary>
        /// <param name="eventData"> Информация ивента. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _textObject.gameObject.SetActive(false);
        }
    }
}