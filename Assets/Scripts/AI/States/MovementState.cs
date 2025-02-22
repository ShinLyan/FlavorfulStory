using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.AI.SceneGraphSystem;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения NPC, в котором персонаж перемещается к заданной точке. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Минимальное расстояние до точки, при котором считается, что NPC достиг её. </summary>
        private const float DistanceToReachPoint = 0.3f;

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private NavMeshAgent _navMeshAgent;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private Animator _animator;

        /// <summary> Расписание NPC, определяющее его маршруты и точки перемещения. </summary>
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Контроллер NPC, управляющий его поведением и анимациями. </summary>
        private NPC _npc;

        /// <summary> Текущая точка маршрута, к которой движется NPC. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Текущая скорость NPC, используемая для анимации. </summary>
        private float _speed;
        
        private WarpGraph _warpGraph;
        private bool _isHandlingWarp;

        /// <summary> Инициализирует новое состояние движения. </summary>
        /// <param name="stateController"> Контроллер состояний. </param>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="navMeshAgent"> Компонент для навигации по NavMesh. </param>
        /// <param name="npc"> Контроллер NPC. </param>
        public MovementState(StateController stateController, NpcSchedule npcSchedule,
            NavMeshAgent navMeshAgent, NPC npc, WarpGraph warpGraph) : base(stateController)
        {
            _stateController = stateController;
            _npcSchedule = npcSchedule;
            _navMeshAgent = navMeshAgent;
            _npc = npc;
            _warpGraph = warpGraph;
        }

        /// <summary> Вызывается при входе в состояние движения. </summary>
        public override void Enter()
        {
            WorldTime.OnDateTimeChanged += FindDestinationPoint;
        }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit()
        {
            WorldTime.OnDateTimeChanged -= FindDestinationPoint;
            _npc.PlayMoveAnimation(0f, 0f);
            _currentPoint = null;
            _isHandlingWarp = false;
        }

        /// <summary> Обновляет логику состояния движения каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            _speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude);
            _npc.PlayMoveAnimation(_speed);
            SwitchStateIfPointReached();
        }

        /// <summary> Проверяет, достиг ли NPC текущей точки, и переключает состояние, если это так. </summary>
        private void SwitchStateIfPointReached()
        {
            if (_currentPoint == null) 
                return;

            bool isInTargetLocation = _npc._currentLocationName == _currentPoint.LocationName;
            bool isCloseEnough = _navMeshAgent.remainingDistance <= DistanceToReachPoint;
    
            if (isInTargetLocation && isCloseEnough)
                _stateController.SetState<RoutineState>();
        }

        /// <summary> Находит ближайшую точку маршрута на основе текущего времени
        /// и задаёт её как цель для NPC. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void FindDestinationPoint(DateTime currentTime)
        {
            SchedulePoint closestPoint = _npcSchedule.Params[0].GetClosestSchedulePointInPath(currentTime);

            if (closestPoint == null)
            {
                Debug.LogError("Ближайшая точка отсутствует!");
                return;
            }
            
            if (_isHandlingWarp || _currentPoint == closestPoint) 
                return;

            if (_npc._currentLocationName != closestPoint.LocationName)
            {
                _isHandlingWarp = true;
                _currentPoint = closestPoint;
                HandleWarpTransition(closestPoint);
            }
            else
            {
                _navMeshAgent.SetDestination(closestPoint.Position);
                _currentPoint = closestPoint;
            }
        }

        private void HandleWarpTransition(SchedulePoint destination)
        {
            Warp currentWarp = FindClosestWarpInCurrentScene();
            Warp targetWarp = FindClosestWarpInScene(destination.Position, destination.LocationName);

            if (currentWarp == null || targetWarp == null)
            {
                Debug.LogError("Не удалось найти начальный или конечный варп!");
                return;
            }
            
            List<Warp> path = _warpGraph.FindShortestPath(currentWarp, targetWarp);

            if (path == null)
            {
                Debug.LogError("Путь не найден!");
                return;
            }
            
            _npc.StartCoroutine(TraverseWarpPath(path, destination));
        }

        private IEnumerator TraverseWarpPath(List<Warp> path, SchedulePoint destination)
        {
            if (_navMeshAgent.isActiveAndEnabled)
                _navMeshAgent.ResetPath();
            
            foreach (var currentWarp in path)
            {
                if (currentWarp.ParentLocation != _npc._currentLocationName)
                {
                    Debug.Log("TELEPORT");
                    _navMeshAgent.Warp(currentWarp.transform.position);
                    _npc._currentLocationName = currentWarp.ParentLocation;
                }
                else
                {
                    _navMeshAgent.SetDestination(currentWarp.transform.position);

                    while (_navMeshAgent.pathPending || 
                           (_navMeshAgent.remainingDistance > DistanceToReachPoint && _navMeshAgent.hasPath))
                        yield return null;
                }
            }

            // когда прошел через все порталы и оказался на конечной сцене
            _navMeshAgent.SetDestination(destination.Position); 
    
            while (_navMeshAgent.remainingDistance > DistanceToReachPoint)
                yield return null;
            
            _isHandlingWarp = false;
        }

        private Warp FindClosestWarpInScene(Vector3 position, LocationType scene)
        {
            WarpNode closestWarp = _warpGraph.FindClosestWarp(position, scene);
            return closestWarp?.SourceWarp;
        }

        private Warp FindClosestWarpInCurrentScene()
        {
            WarpNode closestWarp = _warpGraph.FindClosestWarp(_npc.transform.position, _npc._currentLocationName);
            return closestWarp?.SourceWarp;
        }
    }
}