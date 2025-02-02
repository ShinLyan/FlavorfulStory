using FlavorfulStory.Stats.CharacterStats;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.UI.Bars
{
    /// <summary> Отображает уровень здоровья игрока и обновляет его в реальном времени. </summary>
    public class HealthBar : BaseBar, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент, отвечающий за здоровье игрока. </summary>
        private Health _health;

        /// <summary> Подписка на обновление здоровья при активации. </summary>
        private void OnEnable()
        {
            _health.OnHealthChanged += SetBarText;
        }

        /// <summary> Отписка от обновления здоровья при деактивации. </summary>
        private void OnDisable()
        {
            _health.OnHealthChanged -= SetBarText;
        }

        /// <summary> Инициализация компонента здоровья. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _health = player.GetComponent<Health>();
        }

        /// <summary> Устанавливает начальное значение здоровья и скрывает текст. </summary>
        private void Start()
        {
            SetBarText(_health.CurrentValue);
            _textObject.gameObject.SetActive(false);
        }
        
        /// <summary> Отображает текстовое значение здоровья при наведении курсора. </summary>
        /// <param name="eventData"> Данные события наведения курсора. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBarText(_health.CurrentValue);
            _textObject.gameObject.SetActive(true);
        }
        
        /// <summary> Скрывает текстовое значение здоровья при выходе курсора. </summary>
        /// <param name="eventData"> Данные события ухода курсора. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _textObject.gameObject.SetActive(false);
        }
    }
}