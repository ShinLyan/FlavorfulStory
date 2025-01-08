using TMPro;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.UI
{
    public class TimeVisualiser : MonoBehaviour
    {
        [SerializeField] private TMP_Text _weekDayText;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _timeText;

        private void Awake()
        {
            TimeManagement.WorldTime.OnDateTimeChanged += SetDateTimeText;
        }

        private void OnDestroy()
        {
            TimeManagement.WorldTime.OnDateTimeChanged -= SetDateTimeText;
        }
        private void SetDateTimeText(DateTime dateTime)
        {
            _weekDayText.text = dateTime.GetDayOfWeek().ToString();
            _dateText.text = dateTime.GetDayInSeason().ToString();
            _timeText.text = dateTime.TimeToString();
        }
    }
}