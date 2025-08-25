using System;
using System.Collections.Generic;
using FlavorfulStory.AI.FSM;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public abstract class NpcStateController : IInitializable, IDisposable
    {
        /// <summary> Словарь для быстрого доступа к состояниям по их типу. </summary>
        protected readonly Dictionary<NpcStateName, CharacterState> _nameToCharacterStates;

        /// <summary> Контроллер анимации NPC для управления анимационными состояниями. </summary>
        protected readonly NpcAnimationController _animationController;

        /// <summary> Transform NPC для определения позиции. </summary>
        protected readonly Transform _npcTransform;

        /// <summary> Текущее активное состояние персонажа. </summary>
        protected CharacterState CurrentState { get; private set; }

        /// <summary> Имя текущего стейта. </summary>
        /// <remarks> Для дебага. </remarks>
        public NpcStateName CurrentNpcStateName { get; private set; }

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        protected NpcStateController(NpcAnimationController npcAnimationController, Transform npcTransform)
        {
            _nameToCharacterStates = new Dictionary<NpcStateName, CharacterState>();
            _animationController = npcAnimationController;
            _npcTransform = npcTransform;
        }

        /// <summary> Выполняет полную инициализацию контроллера состояний. </summary>
        /// <remarks> Инициализирует состояния, подписывается на события и сбрасывает систему к начальному состоянию. </remarks>
        public virtual void Initialize() => WorldTime.OnDayEnded += OnReset;

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        public virtual void Dispose() => WorldTime.OnDayEnded -= OnReset;

        /// <summary> Сбрасывает систему состояний при смене дня или инициализации. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        protected abstract void OnReset(DateTime currentTime);

        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        public void Update() => CurrentState?.Update();

        /// <summary> Устанавливает новое состояние персонажа по типу. </summary>
        /// <param name="npcStateName"> Тип состояния для установки. </param>
        protected void SetState(NpcStateName npcStateName)
        {
            if (!_nameToCharacterStates.TryGetValue(npcStateName, out var next))
            {
                Debug.LogWarning($"{npcStateName} state does not exist.");
                return;
            }

            CurrentNpcStateName = npcStateName;

            CurrentState?.Exit();
            CurrentState = next;
            CurrentState?.Enter();
        }
    }
}