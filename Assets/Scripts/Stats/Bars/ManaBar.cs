using FlavorfulStory.Stats.PlayerStats;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.UI.Bars
{
    /// <summary> Отображает уровень маны игрока и обновляет его в реальном времени. </summary>
    public class ManaBar : BaseBar, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент, отвечающий за запас маны игрока. </summary>
        private Mana _mana;

        /// <summary> Подписка на обновление маны при активации. </summary>
        private void OnEnable()
        {
            _mana.OnManaChanged += SetBarText;
        }

        /// <summary> Отписка от обновления маны при деактивации. </summary>
        private void OnDisable()
        {
            _mana.OnManaChanged -= SetBarText;
        }

        /// <summary> Инициализация компонента маны. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _mana = player.GetComponent<Mana>();
        }

        /// <summary> Устанавливает начальное значение маны и скрывает текст. </summary>
        private void Start()
        {
            SetBarText(_mana.CurrentValue);
            _textObject.gameObject.SetActive(false);
        }

        /// <summary> Отображает текстовое значение маны при наведении курсора. </summary>
        /// <param name="eventData"> Данные события наведения курсора. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBarText(_mana.CurrentValue);
            _textObject.gameObject.SetActive(true);
        }

        /// <summary> Скрывает текстовое значение маны при выходе курсора. </summary>
        /// <param name="eventData"> Данные события ухода курсора. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _textObject.gameObject.SetActive(false);
        }
    }
}