using TMPro;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.UI
{
    /// <summary> Отображение времени. </summary>
    public class TimeVisualiser : MonoBehaviour
    {
        /// <summary> День недели. </summary>
        [SerializeField] private TMP_Text _weekDayText;
        
        /// <summary> День сезона. </summary>
        [SerializeField] private TMP_Text _dateText;
        
        /// <summary> Время. </summary>
        [SerializeField] private TMP_Text _timeText;

        private void OnEnable()
        {
            TimeManagement.WorldTime.OnDateTimeChanged += SetDateTimeText;
        }

        private void OnDisable()
        {
            TimeManagement.WorldTime.OnDateTimeChanged -= SetDateTimeText;
        }
        
        /// <summary> Установить текстовое значение времени. </summary>
        /// <param name="dateTime"> Объект времени. </param>
        private void SetDateTimeText(DateTime dateTime)
        {
            _weekDayText.text = dateTime.DayOfWeek.ToString();
            _dateText.text = dateTime.DayInSeason.ToString();
            _timeText.text = dateTime.TimeToString(false);
        }
    }
}