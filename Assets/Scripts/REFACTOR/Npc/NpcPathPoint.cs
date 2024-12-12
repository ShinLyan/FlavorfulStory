using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    /// <summary>Класс - точка для маршрутов NPC. По умолчанию является "Пулом" из 1 точки</summary>
    public class NpcPathPoint : MonoBehaviour
    {
        #region Fields
        /// <summary>Является ли точка порталом для перемещения между инстансами (флаг).</summary>
        [SerializeField] private bool _isInstancePortal; //В будущем унести логику порталов в отдельный класс
        /// <summary>Имя инстанса на котором расположена эта точка.</summary>
        [SerializeField] private string _instanceName; //В будущем унести логику порталов в отдельный класс
        /// <summary>Координата точки.</summary>
        private Vector3 _coordinate;
        /// <summary>Динамический список из имен NPC, кто прошел эту точку. После завершения маршрута данный NPC пропадет из списка</summary>
        [SerializeField] private List<string> _achievedByNpcList;
        #endregion 

        #region Properties
        /// <summary>Свойство для получения координаты точки. Readonly .</summary>
        public Vector3 Coordinate
        {
            get
            {
                return _coordinate;
            }
            set
            {
                _coordinate = value;
            }
        }
        /// <summary>Свойство возвращающее true/false в зависимости от того, является ли точка порталом между инстансами. Readonly .</summary>
        public bool IsInstancePortal
        {
            get
            {
                return _isInstancePortal;
            }
        }
        /// <summary>Свойство возвращающее точку назначения портала между инстансами. Readonly .</summary>
        public NpcPathPoint PortalDestination //В будущем унести логику порталов в отдельный класс
        {
            get
            {
                if (IsInstancePortal) return GetComponent<InstancePortalCollider>().Destination;
                return null;
            }
        }
        /// <summary>Имя инстанса на котором расположена точка. Readonly .</summary>
        public string InstanceName //В будущем унести логику порталов в отдельный класс
        {
            get
            {
                return _instanceName;
            }
        }
        #endregion

        #region Methods
        /// <summary>Метод старта объекта. Для инициализаций.</summary>
        private void Awake() //Awake для того, чтобы к моменту Start() все ссылки на PathPoint получали уже инициализированную позицию
        {
            _coordinate = this.transform.position;
        }
        private void Start()
        {
            _achievedByNpcList = new List<string>();
        }
        /// <summary>Метод возвращающий true/false в зависимости от достижения точки NPC.</summary>
        public bool IsNpcAchieved(string npcIdentifier)
        {
            return _achievedByNpcList.Contains(npcIdentifier);
        }
        /// <summary>Метод для удаления NPC из списка прошедших точку (вызывается после завершения маршрута).</summary>
        public bool RemoveNpcAchieved(string npcIdentifier)
        {
            return _achievedByNpcList.Remove(npcIdentifier);
        }
        /// <summary>Метод для добавления NPC в список прошедших точку (вызывается после завершения маршрута).</summary>
        public bool SetNpcAchieved(string npcIdentifier)
        {
            if (_achievedByNpcList.Contains(npcIdentifier)) return false;

            _achievedByNpcList.Add(npcIdentifier);
            return true;
        }
        #endregion
    }
}