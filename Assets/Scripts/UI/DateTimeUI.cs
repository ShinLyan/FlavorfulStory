using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Отображение текущего времени в UI. </summary>
    public class DateTimeUI : MonoBehaviour
    {
        /// <summary> Текстовое поле отображения текущего сезона. </summary>
        [SerializeField] private TMP_Text _seasonText;

        /// <summary> Текстовое поле отображения дня недели и дня в сезоне. </summary>
        [SerializeField] private TMP_Text _dayText;

        /// <summary> Текстовое поле отображения текущего времени. </summary>
        [SerializeField] private TMP_Text _timeText;

        /// <summary> Подписка на событие изменения времени при активации объекта. </summary>
        private void OnEnable()
        {
            WorldTime.OnDateTimeChanged += SetDateTimeText;
        }

        /// <summary> Отписка от события изменения времени при деактивации объекта. </summary>
        private void OnDisable()
        {
            WorldTime.OnDateTimeChanged -= SetDateTimeText;
        }

        /// <summary> Обновление UI в соответствии с текущим временем. </summary>
        /// <param name="dateTime"> Текущие данные о времени. </param>
        private void SetDateTimeText(DateTime dateTime)
        {
            _seasonText.text = dateTime.Season.ToString();
            _dayText.text = $"{dateTime.DayOfWeek} {dateTime.DayInSeason}";
            _timeText.text = dateTime.TimeToString(false);
        }
    }
}