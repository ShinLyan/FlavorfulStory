using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.TimeManagement;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public abstract class StateController
    {
        /// <summary> Текущее активное состояние персонажа. </summary>
        protected CharacterState _currentState;

        /// <summary> Словарь для быстрого доступа к состояниям по их типу. </summary>
        protected readonly Dictionary<string, CharacterState> _nameToCharacterStates;

        /// <summary> Контроллер анимации NPC для управления анимационными состояниями. </summary>
        protected readonly NpcAnimationController _animationController;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        protected StateController(NpcAnimationController npcAnimationController)
        {
            _nameToCharacterStates = new Dictionary<string, CharacterState>();
            _animationController = npcAnimationController;
        }

        /// <summary> Выполняет полную инициализацию контроллера состояний. </summary>
        /// <remarks> Инициализирует состояния, подписывается на события и сбрасывает систему к начальному состоянию. </remarks>
        protected void Initialize()
        {
            InitializeStates();
            SubscribeToEvents();
            OnReset(WorldTime.CurrentGameTime);
        }

        /// <summary> Инициализирует все доступные состояния персонажа и настраивает связи между ними. </summary>
        /// <remarks> Должен быть реализован в наследниках. </remarks>
        protected abstract void InitializeStates();

        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        public virtual void Update() => _currentState?.Update();

        /// <summary> Сбрасывает систему состояний при смене дня или инициализации. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        protected virtual void OnReset(DateTime currentTime) => ResetStates();

        /// <summary> Сбрасывает все состояния к начальному и устанавливает состояние рутины. </summary>
        /// <remarks> Может быть переопределен в наследниках для специфической логики сброса. </remarks>
        protected abstract void ResetStates();

        /// <summary> Устанавливает новое состояние персонажа по типу. </summary>
        /// <param name="type"> Тип состояния для установки. </param>
        protected void SetState(string type)
        {
            if (!_nameToCharacterStates.TryGetValue(type, out var next) || _currentState == next) return;

            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter();
        }

        /// <summary> Подписывается на события системы времени. </summary>
        /// <remarks> Может быть переопределен в наследниках для дополнительных подписок. </remarks>
        protected virtual void SubscribeToEvents() => WorldTime.OnDayEnded += OnReset;

        /// <summary> Начинает выполнение именованной последовательности действий. </summary>
        /// <param name="sequenceName"> Имя последовательности для запуска. </param>
        public virtual void StartSequence(string sequenceName) { }

        /// <summary> Возвращает персонажа из выполнения последовательности к обычному поведению. </summary>
        public virtual void ReturnFromSequence() { }
    }
}