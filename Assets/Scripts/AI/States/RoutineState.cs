using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.States;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние рутины NPC, в котором персонаж выполняет действия согласно расписанию. </summary>
    public class RoutineState : CharacterState
    {
        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Расписание NPC, определяющее его действия и маршруты. </summary>
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер NPC, управляющий его поведением и анимациями. </summary>
        private NpcController _npcController;

        /// <summary> Текущая точка расписания, в которой находится NPC. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Инициализирует новое состояние рутины. </summary>
        /// <param name="stateController"> Контроллер состояний. </param>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="npcController"> Контроллер NPC. </param>
        public RoutineState(StateController stateController, NpcSchedule npcSchedule,
            NpcController npcController) : base(stateController)
        {
            _stateController = stateController;
            _npcSchedule = npcSchedule;
            _npcController = npcController;
            _currentPoint = null;
        }

        /// <summary> Вызывается при входе в состояние рутины. Подписывается на событие изменения времени. </summary>
        public override void Enter()
        {
            WorldTime.OnDateTimeChanged += CheckNewTime;
        }

        /// <summary> Вызывается при выходе из состояния рутины. Отписывается от события изменения времени
        /// и сбрасывает текущую точку. </summary>
        public override void Exit()
        {
            WorldTime.OnDateTimeChanged -= CheckNewTime;
            _currentPoint = null;
        }

        /// <summary> Обновляет логику состояния рутины каждый кадр.
        /// Воспроизводит анимацию, если текущая точка задана. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            if (_currentPoint != null)
            {
                var animationClipName = _currentPoint.NpcAnimationClipName;

                if (_currentPoint != null)
                    _npcController.PlayStateAnimation(animationClipName);
            }
        }

        /// <summary> Проверяет, изменилось ли время, и обновляет текущую точку расписания.
        /// Переключает состояние на движение, если время совпадает с точкой. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void CheckNewTime(DateTime currentTime)
        {
            SchedulePoint closestPoint = _npcSchedule.Params[0].GetClosestSchedulePointInPath(currentTime);
            if (closestPoint == null)
            {
                Debug.LogError("Ближайшая точка отсутствует!");
                return;
            }

            if (closestPoint == _currentPoint)
                return;

            _currentPoint = closestPoint;
            if (closestPoint.Hour == currentTime.Hour && closestPoint.Minutes == currentTime.Minute)
                _stateController.SetState<MovementState>();
        }
    }
}