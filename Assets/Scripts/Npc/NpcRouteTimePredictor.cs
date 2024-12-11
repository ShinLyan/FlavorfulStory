using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    /// <summary>Класс для расчета предполагаемой длительности маршрута</summary>
    public class NpcRouteTimePredictor
    {
        private const float _factor = 100f; //Расчитать нужное значение
        public static float PredictTime(Vector3 currentPosition, NpcRoute targetRoute)
        {
            float predictedTime = 0f;
            if (targetRoute.Points.Count > 0)
            {

                predictedTime += (float)targetRoute.Length;
                predictedTime += (float)(currentPosition - targetRoute.Points[0].Coordinate).magnitude;
            }
            predictedTime *= _factor;
            //Debug.Log("Time : " + WorldTime.GetCurrentTime() + "/ From Predictor: " + predictedTime.ToString());
            return predictedTime;
        }
    }
}