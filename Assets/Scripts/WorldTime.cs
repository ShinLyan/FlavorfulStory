using FlavorfulStory.Saving;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Глобальное игровое время. </summary>
public class WorldTime : MonoBehaviour, ISaveable
{
    /// <summary>
    /// 
    /// </summary>
    public static WorldTime Instance;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] public float dayTime = 18000f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] public float nightTime = 72000f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] public float dayDuration = 86400f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private float tick = 100f;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private float startTime; //86 400 sec in day

    /// <summary>
    /// 
    /// </summary>
    private static float _currentTime; //86 400 sec in day

    /// <summary>
    /// 
    /// </summary>
    public static bool isDay = true;

    /// <summary>
    /// 
    /// </summary>
    public delegate void DayEndHandler();

    /// <summary>
    /// 
    /// </summary>
    public static event DayEndHandler DayEndedEvent;

    private List<string> daysConst;
    private int currentDay = 0;
    public string currentDayString = "Mon";
    public float Tick {  get { return tick; } } 

    public void Start()
    {
        daysConst = new List<string>();
        daysConst.Add("Mon");
        daysConst.Add("Tue");
        daysConst.Add("Wed");
        daysConst.Add("Thu");
        daysConst.Add("Fri");
        daysConst.Add("Sat");
        daysConst.Add("Sun");
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            _currentTime = startTime;
        }

        DontDestroyOnLoad(Instance.gameObject);

        if (_currentTime < dayTime || _currentTime > nightTime) { isDay = false; }
        else { isDay = true; }
    }

    public void Update()
    {
        //consts
        float dayInterval = nightTime - dayTime;
        float nightInterval = dayDuration - dayInterval; //ToDo: Убрать хардкод
        float nightTickMultiplier = dayInterval / nightInterval;
        //

        _currentTime += tick;
        //Debug.Log(_currentTime.ToString());

        if (_currentTime < dayTime || _currentTime > nightTime) isDay = false;
        else isDay = true;

        if (_currentTime > dayDuration) 
        {
            currentDay++;
            currentDayString = daysConst[(int)currentDay%7];
            _currentTime = 0;
            DayEndedEvent?.Invoke(); 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static float GetCurrentTime()
    {
        return _currentTime;
    }

    #region Saving
    public object CaptureState() => _currentTime;

    public void RestoreState(object state) => _currentTime = (float)state;
    #endregion
}