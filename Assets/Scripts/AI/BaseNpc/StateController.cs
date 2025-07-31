using System;
using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public abstract class StateController : IDisposable, ICharacterCollisionHandler
    {
        /// <summary> Текущее активное состояние персонажа. </summary>
        protected CharacterState _currentState;

        /// <summary> Словарь для быстрого доступа к состояниям по их типу. </summary>
        protected readonly Dictionary<StateName, CharacterState> _nameToCharacterStates;

        /// <summary> Контроллер анимации NPC для управления анимационными состояниями. </summary>
        protected readonly NpcAnimationController _animationController;

        /// <summary> Контроллер игрока для взаимодействия. </summary>
        protected readonly PlayerController _playerController;


        /// <summary> Transform NPC для определения позиции. </summary>
        protected readonly Transform _npcTransform;

        /// <summary> Имя текущего стейта. </summary>
        /// <remarks> Для дебага. </remarks>
        public StateName CurrentStateName { get; private set; }

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="playerController"> Контроллерн игрока. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        protected StateController(NpcAnimationController npcAnimationController, PlayerController playerController,
            Transform npcTransform)
        {
            _nameToCharacterStates = new Dictionary<StateName, CharacterState>();
            _animationController = npcAnimationController;
            _playerController = playerController;
            _npcTransform = npcTransform;
        }

        public void Dispose() => UnsubscribeFromEvents();

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

        /// <summary> Подписывается на события. </summary>
        /// <remarks> Может быть переопределен в наследниках для дополнительных подписок. </remarks>
        protected virtual void SubscribeToEvents() => WorldTime.OnDayEnded += OnReset;

        /// <summary> Подписывается на события. </summary>
        protected virtual void UnsubscribeFromEvents() => WorldTime.OnDayEnded -= OnReset;

        /// <summary> Сбрасывает систему состояний при смене дня или инициализации. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        protected virtual void OnReset(DateTime currentTime) => ResetStates();

        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        public void Update() => _currentState?.Update();

        /// <summary> Сбрасывает все состояния к начальному и устанавливает состояние рутины. </summary>
        /// <remarks> Может быть переопределен в наследниках для специфической логики сброса. </remarks>
        protected abstract void ResetStates();

        /// <summary> Устанавливает новое состояние персонажа по типу. </summary>
        /// <param name="stateName"> Тип состояния для установки. </param>
        protected void SetState(StateName stateName)
        {
            if (!_nameToCharacterStates.TryGetValue(stateName, out var next) || _currentState == next) return;

            CurrentStateName = stateName;

            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter();
        }

        /// <summary> Вызывается при входе игрока в триггер NPC. Переводит NPC в состояние ожидания. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public abstract void OnTriggerEntered(Collider other);

        /// <summary> Вызывается при выходе игрока из триггера NPC. Переводит NPC в состояние движения. </summary>
        /// <param name="other"> Коллайдер, вышедший из триггера. </param>
        public abstract void OnTriggerExited(Collider other);
    }
}