using System;
using UnityEngine;

/// <summary> Класс отвечающий за смену освещения в зависимости от времени.</summary>
public class WorldLight : MonoBehaviour
{
    /// <summary> Объект для света , который меняем.</summary>
    [SerializeField] Light mainLight;
    /// <summary> Нижний порог освещенности.</summary>
    [SerializeField] float minIntensity; //86 400 sec in day
    /// <summary> Верхний порог освещенности.</summary>
    [SerializeField] float maxIntensity; //86 400 sec in day
    /// <summary> Длительность дня.</summary>
    float dayInterval;
    /// <summary> Длительность ночи.</summary>
    float nightInterval;

    /// <summary> .</summary>
    public void Update()
    {
        dayInterval = WorldTime.Instance.nightTime - WorldTime.Instance.dayTime;
        nightInterval = 86400 - dayInterval;
        mainLight.intensity = minIntensity;

        float currTime = WorldTime.GetCurrentTime();
        if (WorldTime.isDay)
        {
            float proportion = (currTime - WorldTime.Instance.dayTime) / dayInterval;
            mainLight.intensity = minIntensity + ((maxIntensity - minIntensity) * proportion);
        }
        else
        {
            if (currTime > WorldTime.Instance.nightTime)
            {
                float proportion = (currTime - WorldTime.Instance.nightTime) / nightInterval;
                mainLight.intensity = maxIntensity - (maxIntensity - minIntensity) * proportion;
            }
            else
            {
                float proportion = (86400 - WorldTime.Instance.nightTime + currTime) / nightInterval;
                mainLight.intensity = maxIntensity - (maxIntensity - minIntensity) * proportion;
            }
        }
    }
}