using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    /// <summary>Вспомогательная структура для восстановления позиции NPC в проивзольной точке маршрута </summary>
    public struct Vector3OnScene
    {
        public string Name;
        public Vector3 Position;
        public Vector3OnScene(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
    }
    /// <summary>Базовый класс для маршрута NPC. Включает в себя List из PathPoint в порядке очередности их достижения </summary>
    public class NpcRoute : MonoBehaviour
    {
        #region Fields
        /// <summary>Список точек в порядке очередности их достижения </summary>
        [SerializeField] private List<NpcPathPoint> _routePoints;
        /// <summary>Название маршрута </summary>
        [SerializeField] private string _routeName;
        #endregion

        #region Properties
        /// <summary>Список точек в порядке очередности их достижения </summary>
        public List<NpcPathPoint> Points
        {
            get
            {
                return _routePoints;
            }
        }
        public string RouteName
        {
            get
            {
                return _routeName;
            }
        }
        public double Length
        {
            get
            {
                double result = 0.0;
                for (int i = 0; i < _routePoints.Count; i++)
                {
                    if (i > 0)
                    {
                        Vector3 previousCoordinate = _routePoints[i - 1].Coordinate;
                        if (_routePoints[i - 1].IsInstancePortal)
                        {
                            if(_routePoints[i - 1].PortalDestination) previousCoordinate = _routePoints[i - 1].PortalDestination.Coordinate;
                        }
                        result += (previousCoordinate - _routePoints[i].Coordinate).magnitude;
                    }
                }
                return result;
            }
        }
        #endregion

        #region Methods
        /// <summary>Возвращает true/false если NPC прошел/не прошел весь маршрут </summary>
        public bool IsAchieved(string npcIdentifier)
        {
            foreach (NpcPathPoint point in Points)
            {
                if (!point.IsNpcAchieved(npcIdentifier)) return false;
            }
            return true;
        }
        /// <summary>Выдает текущую точку для следования. </summary>
        public NpcPathPoint GetRoutePoint(string npcIdentifier)
        {
            foreach (NpcPathPoint point in Points)
            {
                if (!point.IsNpcAchieved(npcIdentifier)) return point;
            }
            return null;//default value. Таких ситуаций при корректной работы логики маршрутов быть не должно
        }
        /// <summary>Устанавливает все точки маршрута в исходное состояние для указанного npc</summary>
        public void Commit(string npcIdentifier)
        {
            foreach (NpcPathPoint point in Points)
            {
                point.RemoveNpcAchieved(npcIdentifier);
            }
        }
        //Restore Route Logic
        /// <summary>Получение координаты маршрута по величине factor (0.0 -> 1.0). 0% - 100% </summary>
        public Vector3 GetPositionByFactor(double factor)
        {
            List<Vector3> path = GetPath(100, GetPathPointsByList());
            int pathIndex = 0;
            if (factor > 0 && factor < 1)
            {
                pathIndex = (int)Math.Round(path.Count * factor);
            }
            else
            {
                double fractionalFactor = factor - Math.Truncate(factor);
                pathIndex = (int)Math.Round(path.Count * fractionalFactor);
            }
            if (pathIndex > 0 && pathIndex < path.Count) return path[pathIndex];
            else return path[path.Count - 1];
        }
        /// <summary>Преобразование коллекции точек вида NpcPathPoint в вспомогательный тип Vector3OnScene</summary>
        private List<Vector3OnScene> GetPathPointsByList()
        {
            List<Vector3OnScene> path = new List<Vector3OnScene>();
            foreach (NpcPathPoint point in Points)
            {
                path.Add(new Vector3OnScene(point.InstanceName, point.Coordinate));
            }
            return path;
        }
        /// <summary>Разбиение маршрута на количество промежуточных точек прямо пропорциональное значению accuracy</summary>
        private List<Vector3> GetPath(float accuracy, List<Vector3OnScene> inputList)
        {
            List<Vector3> resultList = new List<Vector3>();
            for (int i = 0; i < inputList.Count - 1; i++)
            {
                List<Vector3> tempList = new List<Vector3>();
                tempList.Add(inputList[i].Position);
                if (inputList[i].Name == inputList[i + 1].Name)
                {
                    Vector3 distance = inputList[i + 1].Position - inputList[i].Position;
                    Vector3 distanceNormalized = distance / accuracy;
                    float num = distance.magnitude / distanceNormalized.magnitude;
                    for (int v = 1; v < num; v++)
                    {
                        tempList.Add(inputList[i].Position + distanceNormalized * v);
                    }
                    resultList.InsertRange(resultList.Count, tempList);
                }
            }
            return resultList;
        }
        #endregion

    }
}