using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NPC
{
    /// <summary>Базовый класс для расписаний NPC. Включает в себя Лист из Маршрутов и соответствующий им List с ожидаемым временем его окончания (по GlobalWorldTime) </summary>
    public class NpcDailySchedule : MonoBehaviour //Подумать как в инспекторе соединить два листа в 1
    {
        #region Fields
        /// <summary>List из Маршрутов</summary>
        private List<NpcRoute> _targetRoutesList; //В инспекторе создавать строго по возрастанию времени
        /// <summary>List с ожидаемым временем окончания маршрутов(по GlobalWorldTime).</summary>
        private List<float> _targetRouteTimeList; //В инспекторе создавать строго по возрастанию времени
        /// <summary> идентификатор данного расписания.</summary>
        [SerializeField] private string _tag;
        /// <summary> контроллер NPC к которому прикреплено расписание.</summary>
        private NpcController _targetNpcController;
        #endregion

        #region Methods
        /// <summary> В данном методе контролируется время и сопоставляется с расписанием.</summary>
        private void Update()
        {
            NpcRoute route = null;
            if (_targetRouteTimeList.Count > 0)
            {
                if (!_targetNpcController.IsOnRoute)
                {
                    float predictedTime = _targetRouteTimeList[0] - NpcRouteTimePredictor.PredictTime(_targetNpcController.Position, _targetRoutesList[0]);
                    Debug.Log("Predicted Route Time " + WorldTime.GetCurrentTime().ToString() + "/" + predictedTime.ToString());
                    if (WorldTime.GetCurrentTime() > predictedTime) route = _targetRoutesList[0];
                }
            }
            if (route != null)
            {
                Debug.Log("Send to route " + route.name);
                _targetNpcController.SendNpcOnRoute(route);
                _targetRouteTimeList.RemoveAt(_targetRoutesList.IndexOf(route));
                _targetRoutesList.Remove(route);  
            }
        }
        /// <summary> Метод для инициализации расписания.</summary>
        private void Start()
        {
            _targetNpcController = GetComponent<NpcController>();
        }
        /// <summary> Метод для инициализации расписания в новый день.</summary>
        public void NewDaySchedule(List<NpcRoute> routes , List<float> times)
        {
            _targetRoutesList = new List<NpcRoute>();
            _targetRouteTimeList = new List<float>();
            foreach (NpcRoute route in routes)
            {
                _targetRoutesList.Add(route);
            }
            foreach (float time in times)
            {
                _targetRouteTimeList.Add(time);
            }
            _targetNpcController.ResetNpcRoute();
        }
        #endregion
    }
}