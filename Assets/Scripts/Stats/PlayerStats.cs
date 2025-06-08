using System;
using System.Collections.Generic;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Stats
{
    /// <summary> Статы игрока. </summary>
    public class PlayerStats : MonoBehaviour, ISaveable
    {
        /// <summary> Представление атрибута здоровья. </summary>
        [SerializeField] private BaseAttributeView _healthView;

        /// <summary> Представление атрибута выносливости. </summary>
        [SerializeField] private BaseAttributeView _staminaView;

        /// <summary> Коллекция всех статов, индексируемая по типу. </summary>
        private readonly Dictionary<Type, CharacterStat> _stats = new();

        /// <summary> Инициализация базовых статов игрока. </summary>
        private void Awake()
        {
            if (_stats.Count != 0) return;

            Register<Health>(new Health(100), _healthView);
            Register<Stamina>(new Stamina(150), _staminaView);
        }

        /// <summary> Отписка от событий при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            foreach (var attributeKVP in _stats)
                if (TryGetView(attributeKVP.Key, out var view))
                {
                    attributeKVP.Value.OnValueChanged -= view.HandleAttributeChange;
                    attributeKVP.Value.OnReachedZero -= view.HandleAttributeReachZero;
                    attributeKVP.Value.OnMaxValueChanged -= view.HandleAttributeMaxValueChanged;
                }
        }

        /// <summary> Зарегистрировать стат и связать его с отображением. </summary>
        /// <param name="stat"> Объект стата. </param>
        /// <param name="view"> Элемент UI для отображения. </param>
        private void Register<T>(CharacterStat stat, BaseAttributeView view)
        {
            _stats[typeof(T)] = stat;

            stat.OnValueChanged += view.HandleAttributeChange;
            stat.OnReachedZero += view.HandleAttributeReachZero;
            stat.OnMaxValueChanged += view.HandleAttributeMaxValueChanged;

            view.Initialize(stat.CurrentValue, stat.MaxValue);
        }

        /// <summary> Получить стат по типу. </summary>
        /// <typeparam name="T"> Тип стата. </typeparam>
        /// <returns> Объект стата или null. </returns>
        public T GetStat<T>() where T : CharacterStat =>
            _stats.TryGetValue(typeof(T), out var stat) ? stat as T : null;

        /// <summary> Получить представление UI для заданного типа стата. </summary>
        /// <param name="type"> Тип стата. </param>
        /// <param name="view"> Ссылка на соответствующее отображение. </param>
        /// <returns> true — если найдено соответствие. </returns>
        private bool TryGetView(Type type, out BaseAttributeView view)
        {
            view = type == typeof(Health) ? _healthView :
                type == typeof(Stamina) ? _staminaView : null;

            return view;
        }

        #region ISaveable

        /// <summary> Структура для сериализации значений статов. </summary>
        [Serializable]
        private struct StatsData
        {
            /// <summary> Текущее значение здоровья. </summary>
            public float HpCurrentValue;

            /// <summary> Максимальное значение здоровья. </summary>
            public float HpMaxValue;

            /// <summary> Текущее значение выносливости. </summary>
            public float StaminaCurrentValue;

            /// <summary> Максимальное значение выносливости. </summary>
            public float StaminaMaxValue;
        }

        /// <summary> Сохранить текущее состояние всех статов. </summary>
        /// <returns> Объект с сохранёнными значениями. </returns>
        public object CaptureState()
        {
            var health = GetStat<Health>();
            var stamina = GetStat<Stamina>();

            return new StatsData
            {
                HpCurrentValue = health?.CurrentValue ?? 0,
                HpMaxValue = health?.MaxValue ?? 0,
                StaminaCurrentValue = stamina?.CurrentValue ?? 0,
                StaminaMaxValue = stamina?.MaxValue ?? 0
            };
        }

        /// <summary> Восстановить состояние всех статов из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённые данные. </param>
        public void RestoreState(object state)
        {
            if (state is not StatsData data) return;

            Register<Health>(new Health(data.HpMaxValue, data.HpCurrentValue), _healthView);
            Register<Stamina>(new Stamina(data.StaminaMaxValue, data.StaminaCurrentValue), _staminaView);
        }

        #endregion
    }
}