using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace NPC
{
    /// <summary>Класс описывающий логику передвижения NPC.</summary>
    public class NpcController : MonoBehaviour
    {
        #region Fields
        /// <summary>Базовая точка для агента UNITY AI NavMeshAgent (точка спавна).</summary>
        [SerializeField] private NpcPathPoint _basePoint;
        /// <summary> NPC к которому прикреплен контроллер.</summary>
        private Npc _targetNpc;
        /// <summary> Текущий маршрут NPC.</summary>
        private NpcRoute _targetNpcRoute;
        /// <summary>Агент UNITY AI NavMeshAgent.</summary>
        private NavMeshAgent _navMeshAgent;
        /// <summary> Аниматор NPC.</summary>
        private Animator _animator;
        /// <summary> Твердое тело.</summary>
        private Rigidbody _rigidbody;
        /// <summary> Твердое тело.</summary>
        private bool _IsOnRoute;
        #endregion

        #region Properties
        /// <summary>Свойство для получения экземпляра агента UNITY AI NavMeshAgent. Readonly .</summary>
        public NavMeshAgent NavMeshAgent
        {
            get
            {
                return _navMeshAgent;
            }
        }
        /// <summary>Свойство для получения базовой точки NPC.</summary>
        public Vector3 BasePoint
        {
            get
            {
                return _basePoint.Coordinate;
            }
            set
            {
                _basePoint.Coordinate = value;
            }
        }
        /// <summary>Возвращает текущую позицию NPC.</summary>
        public Vector3 Position
        {
            get
            {
                return this.gameObject.transform.position;
            }
        }
        public bool IsOnRoute
        {
            get
            {
                return _IsOnRoute;
            }
            set
            {
                _IsOnRoute = value;
            }
        }
        #endregion

        #region Methods
        /// <summary> Метод для инициализации NPC контроллера .</summary>
        private void Start()
        {
            _IsOnRoute = false;
            _targetNpc = GetComponent<Npc>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            this.ToBasePoint();
            WorldTime.DayEndedEvent += this.ToBasePoint;
        }
        /// <summary> Метод для обновления контроллера .</summary>
        private void Update()
        {
            AnimateMovement(NavMeshAgent.velocity.normalized.magnitude);
            //Debug.Log(WorldTime.GetCurrentTime().ToString());
            //OffMeshLink logic
            if (NavMeshAgent.isOnOffMeshLink)
            {
                OffMeshLinkData data = NavMeshAgent.currentOffMeshLinkData;
                Vector3 endPos = data.endPos + Vector3.up * NavMeshAgent.baseOffset;
                Vector3 destination = NavMeshAgent.destination;
                NavMeshAgent.Warp(endPos);
                NavMeshAgent.CompleteOffMeshLink();
                NavMeshAgent.SetDestination(destination);
            }
            //Route logic
            if (_targetNpcRoute)
            {
                NpcPathPoint point = _targetNpcRoute.GetRoutePoint(_targetNpc.Name);
                if (point != null)
                {
                    if (NavMeshAgent.destination != point.Coordinate)
                        SetDestination(point.Coordinate);
                    IsOnRoute = true;
                }
                else
                {
                    IsOnRoute = false;
                }
            }
        }
        /// <summary> Метод для отправки NPC по маршруту.</summary>
        public void SendNpcOnRoute(NpcRoute route)
        {            
            if (_targetNpcRoute != null) _targetNpcRoute.Commit(_targetNpc.Name);
            _targetNpcRoute = route;
            _targetNpcRoute.Commit(_targetNpc.Name); //Сброс точек маршрута которые NPC мог зацепить
        }
        /// <summary> Метод для отправки NPC по маршруту.</summary>
        public void ResetNpcRoute()
        {
            _targetNpcRoute = null;
        }
        /// <summary>Метод указания точки для движения NPC.</summary>
        public NpcAgentResult SetDestination(Vector3 transform)
        {
            if (!NavMeshAgent.enabled) return NpcAgentResult.eAgentIsDisabled;
            if (NavMeshAgent.isStopped) return NpcAgentResult.eAgentIsStopped;

            bool setDestinationResult = NavMeshAgent.SetDestination(transform);

            if (setDestinationResult) return NpcAgentResult.eStartedMoving;
            else return NpcAgentResult.eInvalidDestination;
        }

        /// <summary>Метод для перемещения NPC в строго указанную точку.</summary>
        public void Warp(Vector3 position)
        {
            Vector3 destination = NavMeshAgent.destination;
            NavMeshAgent.Warp(position);
            //NavMeshAgent.SetDestination(destination);
        }

        /// <summary>Метод для перемещения NPC в базовую точку.</summary>
        public void ToBasePoint()
        {
            this.Warp(BasePoint);
        }

        /// <summary>Метод остановки NPC.</summary>
        public void StopMoving()
        {
            if (NavMeshAgent.enabled)
            {
                NavMeshAgent.isStopped = true;
            }
        }

        /// <summary>Метод продолжения движения NPC.</summary>
        public void StartMoving()
        {
            if (NavMeshAgent.enabled)
            {
                NavMeshAgent.isStopped = false;
            }
        }
        /// <summary> Анимировать передвижение.</summary>
        /// <param name="directionMagnitude"> Величина вектора направления.</param>
        private void AnimateMovement(float speed)
        {
            const float DampTime = 0.2f; // Значение получено эмпирически
            _animator.SetFloat("Speed", speed, DampTime, Time.deltaTime);
        }
        #endregion
    }
}