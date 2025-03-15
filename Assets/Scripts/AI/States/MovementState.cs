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
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private Animator _animator;

        /// <summary> Расписание NPC, определяющее его маршруты и точки перемещения. </summary>
        private readonly NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private readonly StateController _stateController;

        /// <summary> Контроллер NPC, управляющий его поведением и анимациями. </summary>
        private readonly NPC _npc;

        /// <summary> Текущая точка маршрута, к которой движется NPC. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Текущая скорость NPC, используемая для анимации. </summary>
        private float _speed;

        /// <summary> Граф варпов, используемый для поиска пути между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Текущая корутина, выполняющая перемещение NPC по пути. </summary>
        private Coroutine _currentPathCoroutine;

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
            WorldTime.OnTimeUpdated += FindDestinationPoint;
        }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit()
        {
            WorldTime.OnTimeUpdated -= FindDestinationPoint;
            _npc.PlayMoveAnimation(0f, 0f);
            _currentPoint = null;

            if (_currentPathCoroutine != null)
            {
                _npc.StopCoroutine(_currentPathCoroutine);
                _currentPathCoroutine = null;
            }
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

            bool isInTargetLocation = _npc.CurrentLocationName == _currentPoint.LocationName;
            bool isCloseEnough = _navMeshAgent.remainingDistance <= DistanceToReachPoint;

            if (isInTargetLocation && isCloseEnough)
                _stateController.SetState<RoutineState>();
        }

        /// <summary> Находит ближайшую точку маршрута на основе текущего времени
        /// и задаёт её как цель для NPC. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void FindDestinationPoint(DateTime currentTime)
        {
            var newPoint = _npcSchedule.Params[0].GetClosestSchedulePointInPath(currentTime);

            if (newPoint == null)
            {
                Debug.LogError("Ближайшая точка отсутствует!");
                return;
            }

            if (_currentPoint == newPoint) return;

            Debug.Log(_currentPathCoroutine);
            if (_currentPathCoroutine != null)
            {
                Debug.Log("STOP COROUTINE");
                _npc.StopCoroutine(_currentPathCoroutine);
                _currentPathCoroutine = null;
            }

            _currentPoint = newPoint;

            if (_npc.CurrentLocationName != newPoint.LocationName)
                HandleWarpTransition(newPoint);
            else
                _navMeshAgent.SetDestination(newPoint.Position);
        }

        /// <summary> Обрабатывает переход NPC через варпы между локациями. </summary>
        /// <param name="destination"> Целевая точка расписания. </param>
        private void HandleWarpTransition(SchedulePoint destination)
        {
            var currentWarp = FindClosestWarpInCurrentScene();
            var targetWarp = FindClosestWarpInScene(destination.Position, destination.LocationName);

            if (currentWarp == null || targetWarp == null)
            {
                Debug.LogError("Не удалось найти начальный или конечный варп!");
                return;
            }

            var path = _warpGraph.FindShortestPath(currentWarp, targetWarp);

            if (path == null)
            {
                Debug.LogError("Путь не найден!");
                return;
            }

            _currentPathCoroutine = _npc.StartCoroutine(TraverseWarpPath(path, destination));
        }

        /// <summary> Перемещает NPC по пути через варпы. </summary>
        /// <param name="path"> Список варпов, через которые нужно пройти. </param>
        /// <param name="destination"> Целевая точка расписания. </param>
        private IEnumerator TraverseWarpPath(List<Warp> path, SchedulePoint destination)
        {
            foreach (var currentWarp in path)
                if (currentWarp.ParentLocation != _npc.CurrentLocationName)
                {
                    _navMeshAgent.Warp(currentWarp.transform.position);
                    _npc.CurrentLocationName = currentWarp.ParentLocation;
                }
                else
                {
                    _navMeshAgent.SetDestination(currentWarp.transform.position);

                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > DistanceToReachPoint)
                        yield return null;
                }

            // когда прошел через все варпы и оказался на конечной сцене
            _navMeshAgent.SetDestination(destination.Position);

            while (_navMeshAgent.remainingDistance > DistanceToReachPoint)
                yield return null;
        }

        /// <summary> Находит ближайший варп в указанной локации. </summary>
        /// <param name="position"> Позиция для поиска ближайшего варпа. </param>
        /// <param name="location"> Локация, в которой искать варп. </param>
        /// <returns> Ближайший варп или null, если варп не найден. </returns>
        private Warp FindClosestWarpInScene(Vector3 position, LocationName location)
        {
            var closestWarp = _warpGraph.FindClosestWarp(position, location);
            return closestWarp?.SourceWarp;
        }

        /// <summary> Находит ближайший варп в текущей локации NPC. </summary>
        /// <returns> Ближайший варп или null, если варп не найден. </returns>
        private Warp FindClosestWarpInCurrentScene()
        {
            var closestWarp = _warpGraph.FindClosestWarp(_npc.transform.position, _npc.CurrentLocationName);
            return closestWarp?.SourceWarp;
        }
    }
}