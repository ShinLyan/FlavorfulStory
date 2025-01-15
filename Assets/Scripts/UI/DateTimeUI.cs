using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Отображение времени в пользовательском интерфейсе. </summary>
    public class DateTimeUI : MonoBehaviour
    {
        /// <summary> Текстовое поле сезона. </summary>
        [SerializeField] private TMP_Text _seasonText;

        /// <summary> Текстовое поле дня недели и сезона. </summary>
        [SerializeField] private TMP_Text _dayText;

        /// <summary> Текстовое поле времени. </summary>
        [SerializeField] private TMP_Text _timeText;

        /// <summary> Подписка на событие изменения времени при активации. </summary>
        private void OnEnable()
        {
            WorldTime.OnDateTimeChanged += SetDateTimeText;
        }

        /// <summary> Отписка от события изменения времени при деактивации. </summary>
        private void OnDisable()
        {
            WorldTime.OnDateTimeChanged -= SetDateTimeText;
        }

        /// <summary> Установка значений текстовых полей интерфейса на основе текущего времени. </summary>
        /// <param name="dateTime"> Данные о текущем времени. </param>
        private void SetDateTimeText(DateTime dateTime)
        {
            _seasonText.text = dateTime.Season.ToString();
            _dayText.text = $"{dateTime.DayOfWeek.ToString()} {dateTime.DayInSeason.ToString()}";
            _timeText.text = dateTime.TimeToString(false);
        }
    }
}