using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    /// <summary>Базовый класс для расписаний NPC. Включает в себя Лист из Маршрутов и соответствующий им List с ожидаемым временем его окончания (по GlobalWorldTime) </summary>
    public class NpcScheduleManager : MonoBehaviour //Подумать как в инспекторе соединить два листа в 1
    {
        #region Fields
        /// <summary>List из Маршрутов</summary>
        [SerializeField] private List<NpcRoute> _targetRoutesList; //В инспекторе создавать строго по возрастанию времени
        /// <summary>List с ожидаемым временем окончания маршрутов(по GlobalWorldTime).</summary>
        [SerializeField] private List<float> _targetRouteTimeList; //В инспекторе создавать строго по возрастанию времени
        /// <summary> NPC к которому прикреплено расписание.</summary>
        private Npc _targetNpc;
        /// <summary>.</summary>
        private NpcDailySchedule _dailySchedule; //В инспекторе создавать строго по возрастанию времени
        #endregion

        #region Methods
        /// <summary> В данном методе контролируется время и сопоставляется с расписанием.</summary>
        private void Start()
        {
            _targetNpc = GetComponent<Npc>();
            _dailySchedule = GetComponent<NpcDailySchedule>();
            WorldTime.DayEndedEvent += OnDayChanged;
            _dailySchedule.NewDaySchedule(_targetRoutesList, _targetRouteTimeList);
        }
        /// <summary> Вызывается при старте нового дня.</summary>
        public void OnDayChanged() 
        {
            foreach (var route in _targetRoutesList)
            {
                route.Commit(_targetNpc.Name);
            }
            _dailySchedule.NewDaySchedule(_targetRoutesList, _targetRouteTimeList);
        }
        #endregion
    }
}
  