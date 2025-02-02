using FlavorfulStory.Stats.PlayerStats;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.UI.Bars
{
    /// <summary> Отображает уровень энергии игрока и обновляет его в реальном времени. </summary>
    public class EnergyBar : BaseBar, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент, отвечающий за запас энергии игрока. </summary>
        private Stamina _stamina;

        /// <summary> Подписка на обновление энергии при активации. </summary>
        private void OnEnable()
        {
            _stamina.OnStaminaChanged += SetBarText;
        }

        /// <summary> Отписка от обновления энергии при деактивации. </summary>
        private void OnDisable()
        {
            _stamina.OnStaminaChanged -= SetBarText;
        }

        /// <summary> Инициализация компонента энергии. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _stamina = player.GetComponent<Stamina>();
        }

        /// <summary> Устанавливает начальное значение энергии и скрывает текст. </summary>
        private void Start()
        {
            SetBarText(_stamina.CurrentValue);
            _textObject.gameObject.SetActive(false);
        }

        /// <summary> Отображает текстовое значение энергии при наведении курсора. </summary>
        /// <param name="eventData"> Данные события наведения курсора. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBarText(_stamina.CurrentValue);
            _textObject.gameObject.SetActive(true);
        }

        /// <summary> Скрывает текстовое значение энергии при выходе курсора. </summary>
        /// <param name="eventData"> Данные события ухода курсора. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _textObject.gameObject.SetActive(false);
        }
    }
}