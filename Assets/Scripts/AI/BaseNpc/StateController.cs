using System;
using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.TimeManagement;
using UnityEngine;
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
        protected readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Контроллер анимации NPC для управления анимационными состояниями. </summary>
        protected readonly NpcAnimationController _animationController;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        protected StateController(NpcAnimationController npcAnimationController)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();
            _animationController = npcAnimationController;
        }

        protected void Initialize()
        {
            InitializeStates();
            SubscribeToEvents();
            OnReset(WorldTime.CurrentGameTime);
        }

        /// <summary> Инициализирует все доступные состояния персонажа и настраивает связи между ними. </summary>
        protected abstract void InitializeStates();


        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        public void Update()
        {
            _currentState?.Update();
            Debug.Log(_currentState);
        }

        /// <summary> Сбрасывает систему состояний при смене дня или инициализации. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void OnReset(DateTime currentTime)
        {
            OnPreReset(currentTime);
            ResetStates();
        }

        protected virtual void OnPreReset(DateTime currentTime) { }

        /// <summary> Сбрасывает все состояния к начальному и устанавливает состояние рутины. </summary>
        protected virtual void ResetStates() { }

        /// <summary> Устанавливает новое состояние персонажа по типу. </summary>
        /// <param name="type"> Тип состояния для установки. </param>
        protected void SetState(Type type)
        {
            if (!_typeToCharacterStates.TryGetValue(type, out var next) || _currentState == next) return;

            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter();
        }

        protected virtual void SubscribeToEvents() => WorldTime.OnDayEnded += OnReset;
    }
}