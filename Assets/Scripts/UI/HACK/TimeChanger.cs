using TMPro;
using UnityEngine;

/// <summary> Класс отвечающий за изменение UI счетчика времени.</summary>
public class TimeChanger : MonoBehaviour //Скорее всего сделать singleton + положить в название UI (для четкости)
{
    [SerializeField] TMP_Text textObject;
    public void Update()
    {
        string hour = System.Math.Truncate(WorldTime.GetCurrentTime() / 3600).ToString();
        double minutes = System.Math.Truncate(((WorldTime.GetCurrentTime() - System.Math.Truncate(WorldTime.GetCurrentTime() / 3600) * 3600)/60));
        string minutesText = minutes.ToString();
        if(minutes < 10) minutesText = "0" + minutesText;
        textObject.text = WorldTime.Instance.currentDayString + " " + hour + " : " + minutesText;
    }
}