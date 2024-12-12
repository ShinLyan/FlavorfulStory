using FlavorfulStory.Stats.PlayerStats;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.UI.Bars
{
    /// <summary>  Класс, отвечающий за отрисовку количества энергии.</summary>
    public class EnergyBar : BaseBar, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент энергии.</summary>
        private Stamina _stamina;

        /// <summary> Подписка на события.</summary>
        private void OnEnable()
        {
            _stamina.OnStaminaChanged += SetBarText;
        }

        /// <summary> Отписка от событий.</summary>
        private void OnDisable()
        {
            _stamina.OnStaminaChanged -= SetBarText;
        }

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _stamina = player.GetComponent<Stamina>();
        }
        
        private void Start()
        {
            SetBarText(_stamina.CurrentValue);
            _textObject.gameObject.SetActive(false);
        }

        /// <summary> Включение объекта при наведении на него курсора.</summary>
        /// <param name="eventData"> Информация ивента.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBarText(_stamina.CurrentValue);
            _textObject.gameObject.SetActive(true);
        }

        /// <summary> Выключение объекта при наведении на него курсора.</summary>
        /// <param name="eventData"> Информация ивента.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _textObject.gameObject.SetActive(false);
        }
    }
}
