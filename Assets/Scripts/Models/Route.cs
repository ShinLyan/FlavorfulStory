using System;
using System.Collections.Generic;
using UnityEngine;

//public struct Vector3OnScene
//{
//    public Vector3OnScene(string name, Vector3 position)
//    {
//        Name = name;
//        Position = position;
//    }
//    public string Name;
//    public Vector3 Position;
//}

//public class Route : MonoBehaviour //Разнести логику на несколько классов
//{
//    //[SerializeField] private ScenePortals portals;
//    [SerializeField] private List<PathPoint> route;
//    [SerializeField] private float time;
//    [SerializeField] private string routeName;
//    [SerializeField] private string npcName;
//    public float Time { get { return time; } }
//    public string Name { get { return routeName; } }
//    public bool Achieved { get; set; }
//    public int Count { get { return route.Count; } }

//    public void Start()
//    {
//        foreach (var pt in route)
//        {
//            pt.ResetAcheved(npcName);
//        }
//        Achieved = false;
//    }

//    public void Update()
//    {
//        if (Achieved)
//        {
//            ResetAllPoints();
//        }
//    }

//    public Vector3 GetPositionByFactor(PathPoint startPoint, double factor)
//    {
//        List<Vector3> path = GetPath(100, GetPathPoints(startPoint));
//        int pathIndex = 0;
//        if (factor > 0 && factor < 1)
//        {
//            pathIndex = (int)Math.Round(path.Count * factor);
//        }
//        else
//        {
//            double fractionalFactor = factor - Math.Truncate(factor);
//            pathIndex = (int)Math.Round(path.Count * fractionalFactor);
//        }
//        if (pathIndex > 0 && pathIndex < path.Count)
//        {
//            return path[pathIndex];
//        }
//        else
//        {
//            return path[path.Count - 1];
//        }
//    }

//    public List<Vector3OnScene> GetPathPoints(PathPoint startPoint)
//    {
//        List<Vector3OnScene> path = new List<Vector3OnScene>();
//        path.Add(new Vector3OnScene(startPoint.Scene, startPoint.GetPosition()));
//        foreach (var p in route)
//        {
//            path.Add(new Vector3OnScene(p.Scene, p.GetPosition()));
//        }
//        return path;
//    }

//    public List<Vector3> GetPath(float accuracy, List<Vector3OnScene> oldList)
//    {
//        List<Vector3> newList = new List<Vector3>();
//        for (int i = 0; i < oldList.Count - 1; i++)
//        {
//            List<Vector3> toAdd = new List<Vector3>();
//            toAdd.Add(oldList[i].Position);
//            if (oldList[i].Name == oldList[i + 1].Name)
//            {
//                Vector3 distance = oldList[i + 1].Position - oldList[i].Position;
//                Vector3 distanceNormalized = distance / accuracy;
//                float num = distance.magnitude / distanceNormalized.magnitude;
//                for (int v = 1; v < num; v++)
//                {
//                    toAdd.Add(oldList[i].Position + distanceNormalized * v);
//                }
//                newList.InsertRange(newList.Count, toAdd);
//            }
//            else
//            {
//                foreach (var p in portals.Portals)
//                {
//                    if ((p.Scene == oldList[i].Name) && (p.GetComponent<PortalCollider>().Destination.Scene == oldList[i + 1].Name))
//                    {
//                        //
//                    }
//                }
//            }
//        }
//        return newList;
//    }
//    public PathPoint GetFirst()
//    {
//        return route[0];
//    }
//    public PathPoint GetFinal()
//    {
//        return route[route.Count - 1];
//    }
//    public void ResetAllPoints()
//    {
//        foreach (var p in route)
//        {
//            Debug.Log("RESET: " + route.IndexOf(p));
//            p.ResetAcheved(npcName);
//        }
//    }
//    public PathPoint GetOnScene(string sceneName)
//    {
//        for (int i = 0; i < route.Count; i++)
//        {
//            if (route[i].Scene == sceneName && route[i].IsAchieved(npcName) == false) return route[i];
//        }
//        return null;
//    }
//    public PathPoint GetNextPoint()
//    {
//        if (!Achieved)
//        {
//            for (int i = 0; i < route.Count; i++)
//            {
//                if (route[i].IsAchieved(npcName) == false) return route[i];
//            }
//            ResetAllPoints();
//            Achieved = true;
//        }
//        return null;
//    }
//    public PathPoint GetLastOnScene(string sceneName)
//    {
//        for (int i = route.Count - 1; i > -1; i--)
//        {
//            if (route[i].Scene == sceneName) return route[i];
//        }
//        return null;
//    }
//    public float GetLength(PathPoint startPoint) //Переписать этот стыд. Оно работает и трогать пока страшно
//    {
//        float length = 0;

//        if (route.Count == 1)
//        {
//            length += (startPoint.GetPosition() - route[0].GetPosition()).magnitude;
//            if (startPoint.Scene != route[0].Scene)
//            {
//                foreach (var p in portals.Portals)
//                {
//                    if ((p.Scene == startPoint.Scene) && (p.GetComponent<PortalCollider>().Destination.Scene == route[0].Scene))
//                    {
//                        length -= (p.GetComponent<PathPoint>().GetPosition() - p.GetComponent<PortalCollider>().Destination.GetPosition()).magnitude;
//                    }
//                }
//            }
//        }
//        else
//        {
//            for (int i = 0; i < route.Count - 1; i++)
//            {
//                if (i == 0)
//                {
//                    length += (startPoint.GetPosition() - route[i].GetPosition()).magnitude;
//                }
//                else
//                {
//                    length += (route[i].GetPosition() - route[i + 1].GetPosition()).magnitude;
//                }
//            }

//            for (int i = 0; i < route.Count - 1; i++)
//            {
//                if (i == 0)
//                {
//                    if (startPoint.Scene != route[i].Scene)
//                    {
//                        foreach (var p in portals.Portals)
//                        {
//                            if ((p.Scene == startPoint.Scene) && (p.GetComponent<PortalCollider>().Destination.Scene == route[i].Scene))
//                            {
//                                length -= (p.GetComponent<PathPoint>().GetPosition() - p.GetComponent<PortalCollider>().Destination.GetPosition()).magnitude;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    if (route[i].Scene != route[i + 1].Scene)
//                    {
//                        foreach (var p in portals.Portals)
//                        {
//                            if ((p.Scene == route[i].Scene) && (p.GetComponent<PortalCollider>().Destination.Scene == route[i + 1].Scene))
//                            {
//                                length -= (p.GetComponent<PathPoint>().GetPosition() - p.GetComponent<PortalCollider>().Destination.GetPosition()).magnitude;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        return length;
//    }
//}