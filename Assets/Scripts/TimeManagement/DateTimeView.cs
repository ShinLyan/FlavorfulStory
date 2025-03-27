using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Отображение текущего времени в UI. </summary>
    public class DateTimeView : MonoBehaviour
    {
        /// <summary> Текстовое поле отображения текущего сезона. </summary>
        [SerializeField] private TMP_Text _seasonText;

        /// <summary> Текстовое поле отображения дня недели и дня в сезоне. </summary>
        [SerializeField] private TMP_Text _dayText;

        /// <summary> Текстовое поле отображения текущего времени. </summary>
        [SerializeField] private TMP_Text _timeText;

        private static readonly Dictionary<int, string> IntToDayOfWeek = new()
        {
            { 1, "Monday" },
            { 2, "Tuesday" },
            { 3, "Wednesday" },
            { 4, "Thursday" },
            { 5, "Friday" },
            { 6, "Saturday" },
            { 7, "Sunday" }
        };

        /// <summary> Подписка на событие изменения времени при активации объекта. </summary>
        private void OnEnable() => WorldTime.OnTimeUpdated += SetDateTimeText;

        /// <summary> Отписка от события изменения времени при деактивации объекта. </summary>
        private void OnDisable() => WorldTime.OnTimeUpdated -= SetDateTimeText;

        /// <summary> Обновление UI в соответствии с текущим временем. </summary>
        /// <param name="dateTime"> Текущие данные о времени. </param>
        private void SetDateTimeText(DateTime dateTime)
        {
            _seasonText.text = dateTime.Season.ToString();
            _dayText.text = $"{IntToDayOfWeek[(int)dateTime.DayOfWeek]} {dateTime.SeasonDay}";
            _timeText.text = dateTime.TimeToString(false);
        }
    }
}