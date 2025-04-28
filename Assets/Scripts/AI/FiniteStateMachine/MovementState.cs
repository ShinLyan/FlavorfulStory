using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения NPC, в котором персонаж перемещается к заданной точке. </summary>
    public class MovementState : CharacterState, IScheduleDependable
    {
        #region Fields

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private readonly NavMeshAgent _navMeshAgent;

        /// <summary> Граф варпов, используемый для поиска пути между локациями. </summary>
        private readonly WarpGraph _warpGraph;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private readonly Animator _animator;

        /// <summary> Компонент Transform. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Проигрыватель корутин. </summary>
        private readonly MonoBehaviour _coroutineRunner;

        /// <summary> Позиция спавна NPC. </summary>
        private readonly Vector3 _spawnPosition;

        /// <summary> Локация спавна. </summary>
        private readonly LocationName _spawnLocation;

        /// <summary> Хэш параметра скорости анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        /// <summary> Текущее расписание. </summary>  
        private ScheduleParams _currentScheduleParams;

        /// <summary> Стек маршрута движения по расписанию. </summary>
        private Stack<SchedulePoint> _currentPath;

        /// <summary> Текущая точка маршрута, к которой движется NPC. </summary>
        private SchedulePoint _currentTargetPoint;

        /// <summary> Текущая локация. </summary>
        private LocationName _currentLocation;

        /// <summary> Текущая корутина, выполняющая перемещение NPC по пути. </summary>
        private Coroutine _currentPathCoroutine;

        /// <summary> Минимальное расстояние до точки для её достижения. </summary>
        private const float DistanceToReachPoint = 1.0f;

        #endregion

        /// <summary> Инициализирует новое состояние движения. </summary>
        /// <param name="navMeshAgent"> Компонент для навигации по NavMesh. </param>
        /// <param name="warpGraph"> Граф варпов. </param>
        /// <param name="animator"> Аниматор. </param>
        /// <param name="npcTransform"> Компонент Transform. </param>
        /// <param name="coroutineRunner"> Проигрыватель корутин. </param>
        public MovementState(NavMeshAgent navMeshAgent, WarpGraph warpGraph,
            Animator animator, Transform npcTransform, MonoBehaviour coroutineRunner)
        {
            _navMeshAgent = navMeshAgent;
            _warpGraph = warpGraph;
            _animator = animator;
            _npcTransform = npcTransform;
            _coroutineRunner = coroutineRunner;

            _spawnPosition = npcTransform.position;
            _spawnLocation = GetCurrentLocationName();
            _currentLocation = _spawnLocation;

            WorldTime.OnTimePaused += OnTimePaused;
            WorldTime.OnTimeUnpaused += OnTimeUnpaused;
            WorldTime.OnTimeUpdated += UpdateCurrentSchedulePoint;
        }

        /// <summary> Получить имя локации, в которой находится NPC. </summary>
        /// <returns> Имя локации. </returns>
        private LocationName GetCurrentLocationName()
        {
            foreach (var location in Object.FindObjectsByType<Location>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
                if (location.IsPositionInLocation(_npcTransform.position))
                    return location.LocationName;
            return LocationName.RockyIsland;
        }

        /// <summary> Вызывается при входе в состояние движения. </summary>
        public override void Enter() { }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit() => StopMovementWithoutWarp();

        /// <summary> Обновляет логику состояния движения каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            float speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude) * 0.5f;
            PlayMoveAnimation(speed);

            if (_currentTargetPoint == null) return;

            SwitchStateIfPointReached();
        }

        /// <summary> Сброс состояния в начальное состояние. </summary>
        public override void Reset() => StopMovementAndWarp();

        /// <summary> Остановка движения без телепортации. </summary>
        private void StopMovementWithoutWarp()
        {
            StopCurrentCoroutine();
            PlayMoveAnimation(0f, 0f);
            _navMeshAgent.ResetPath();
        }

        /// <summary> Полный сброс движения с возвратом на точку спавна. </summary>
        private void StopMovementAndWarp()
        {
            StopMovementWithoutWarp();
            _navMeshAgent.Warp(_spawnPosition);
            _currentLocation = _spawnLocation;
            _currentTargetPoint = null;
        }

        /// <summary> Обновляет текущую точку маршрута согласно времени. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void UpdateCurrentSchedulePoint(DateTime currentTime)
        {
            if (_currentPath.Count <= 0) return;

            var nextPoint = _currentPath.Peek();
            if ((int)currentTime.Hour != nextPoint.Hour || (int)currentTime.Minute != nextPoint.Minutes) return;

            StopCurrentCoroutine();
            _currentTargetPoint = _currentPath.Pop();
            MoveToCurrentTarget();
        }

        /// <summary> Начать движение к текущей целевой точке. </summary>
        private void MoveToCurrentTarget()
        {
            if (_currentTargetPoint == null) return;

            if (_currentLocation != _currentTargetPoint.LocationName)
                StartWarpTransition(_currentTargetPoint);
            else
                _navMeshAgent.SetDestination(_currentTargetPoint.Position);
        }

        /// <summary> Проверяет, достиг ли NPC текущей точки, и переключает состояние, если это так. </summary>
        private void SwitchStateIfPointReached()
        {
            bool isInTargetLocation = _currentLocation == _currentTargetPoint.LocationName;
            bool isCloseEnough =
                Vector3.Distance(_currentTargetPoint.Position, _npcTransform.position) <= DistanceToReachPoint;

            if (isInTargetLocation && isCloseEnough) RequestStateChange(typeof(RoutineState));
        }

        /// <summary> Обрабатывает переход NPC через варпы между локациями. </summary>
        /// <param name="destination"> Целевая точка расписания. </param>
        private void StartWarpTransition(SchedulePoint destination)
        {
            var currentWarp = FindClosestWarpInScene(_npcTransform.position, _currentLocation);
            var targetWarp = FindClosestWarpInScene(destination.Position, destination.LocationName);

            if (!currentWarp || !targetWarp)
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

            _currentPathCoroutine = _coroutineRunner.StartCoroutine(TraverseWarpPath(path, destination));
        }

        /// <summary> Перемещает NPC по пути через варпы. </summary>
        /// <param name="path"> Список варпов, через которые нужно пройти. </param>
        /// <param name="destination"> Целевая точка расписания. </param>
        private IEnumerator TraverseWarpPath(List<WarpPortal> path, SchedulePoint destination)
        {
            foreach (var currentWarp in path)
                if (currentWarp.ParentLocationName != _currentLocation)
                {
                    _navMeshAgent.Warp(currentWarp.transform.position);
                    _currentLocation = currentWarp.ParentLocationName;
                }
                else
                {
                    _navMeshAgent.SetDestination(currentWarp.transform.position);
                    while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > DistanceToReachPoint)
                        yield return null;
                }

            // когда прошел через все варпы и оказался на конечной сцене
            _navMeshAgent.SetDestination(destination.Position);

            while (_navMeshAgent.remainingDistance > DistanceToReachPoint) yield return null;
        }

        /// <summary> Находит ближайший варп в указанной локации. </summary>
        /// <param name="position"> Позиция для поиска ближайшего варпа. </param>
        /// <param name="location"> Локация, в которой искать варп. </param>
        /// <returns> Ближайший варп или null, если варп не найден. </returns>
        private WarpPortal FindClosestWarpInScene(Vector3 position, LocationName location)
        {
            var closestWarp = _warpGraph.FindClosestWarp(position, location);
            return closestWarp?.SourceWarp;
        }

        /// <summary> Остановить корутину. </summary>
        private void StopCurrentCoroutine()
        {
            if (_currentPathCoroutine == null) return;

            _coroutineRunner.StopCoroutine(_currentPathCoroutine);
            _currentPathCoroutine = null;
        }

        /// <summary> Установить новое расписание. </summary>
        /// <param name="scheduleParams"> Новое расписание. </param>
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams)
        {
            _currentScheduleParams = scheduleParams;
            if (_currentScheduleParams == null) return;

            _currentPath = _currentScheduleParams.GetSortedSchedulePointsStack();
        }

        /// <summary> Воспроизведение анимации движения. </summary>
        /// <param name="speed"> Скорость движения. </param>
        /// <param name="dampTime"> Время сглаживания перехода анимации. </param>
        private void PlayMoveAnimation(float speed, float dampTime = 0.2f) =>
            _animator.SetFloat(_speedParameterHash, speed, dampTime, Time.deltaTime);

        /// <summary> Обработчик паузы времени. </summary>
        private void OnTimePaused() => StopMovementWithoutWarp();

        /// <summary> Обработчик возобновления времени. </summary>
        private void OnTimeUnpaused() => MoveToCurrentTarget();
    }
}