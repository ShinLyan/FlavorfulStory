using UnityEngine;

namespace NPC
{
    /// <summary>Класс описывающий логику коллайдера у точки маршрута.</summary>
    ///<remarks> Является переключателем, для установки точки "достигнутой" .</remarks>
    public class NpcPathPointCollider : MonoBehaviour
    {
        #region Fields
        /// <summary>PathPoint на котором висит коллайдер.</summary>
        private NpcPathPoint _pathPoint;
        #endregion

        #region Methods
        /// <summary>Метод вызывающийся при старте скрипта.</summary>
        public void Start()
        {
            _pathPoint = GetComponent<NpcPathPoint>();
        }

        /// <summary>Метод вызывающийся при входе в коллайдер объекта на котором этот скрипт висит .</summary>
        /// <param name="other"> Коллайдер входящего объекта.</param>
        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Npc"))
            {
                Npc npc = other.gameObject.GetComponent<Npc>();
                _pathPoint.SetNpcAchieved(npc.Name);
            }
        }
        #endregion
    }
}