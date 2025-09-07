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
        [SerializeField] private StatView _healthView;

        /// <summary> Представление атрибута выносливости. </summary>
        [SerializeField] private StatView _staminaView;

        /// <summary> Коллекция всех статов, индексируемая по типу. </summary>
        private readonly Dictionary<Type, CharacterStat> _stats = new();

        /// <summary> Инициализация базовых статов игрока. </summary>
        private void Start()
        {
            if (_stats.Count != 0) return;

            Register<Health>(new Health(200), _healthView);
            Register<Stamina>(new Stamina(150), _staminaView);
        }

        /// <summary> Отписка от событий при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            foreach (var attributeKVP in _stats)
                if (TryGetView(attributeKVP.Key, out var view))
                {
                    attributeKVP.Value.OnValueChanged -= view.UpdateCurrentValue;
                    attributeKVP.Value.OnMaxValueChanged -= view.UpdateMaxValue;
                }
        }

        /// <summary> Зарегистрировать стат и связать его с отображением. </summary>
        /// <param name="stat"> Объект стата. </param>
        /// <param name="view"> Элемент UI для отображения. </param>
        private void Register<T>(CharacterStat stat, StatView view)
        {
            _stats[typeof(T)] = stat;

            stat.OnValueChanged += view.UpdateCurrentValue;
            stat.OnMaxValueChanged += view.UpdateMaxValue;
            stat.OnReachedZero += () => Debug.Log("ReachedZero");

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
        private bool TryGetView(Type type, out StatView view)
        {
            view = type == typeof(Health) ? _healthView :
                type == typeof(Stamina) ? _staminaView : null;

            return view;
        }

        #region ISaveable

        /// <summary> Структура для сериализации значений статов. </summary>
        [Serializable]
        private readonly struct StatsRecord
        {
            /// <summary> Текущее значение здоровья. </summary>
            public float HpCurrentValue { get; }

            /// <summary> Максимальное значение здоровья. </summary>
            public float HpMaxValue { get; }

            /// <summary> Текущее значение выносливости. </summary>
            public float StaminaCurrentValue { get; }

            /// <summary> Максимальное значение выносливости. </summary>
            public float StaminaMaxValue { get; }

            /// <summary> Конструктор с параметрами. </summary>
            /// <param name="hpCurrentValue"> Текущее значение здоровья. </param>
            /// <param name="hpMaxValue"> Максимальное значение здоровья. </param>
            /// <param name="staminaCurrentValue"> Текущее значение выносливости. </param>
            /// <param name="staminaMaxValue"> Максимальное значение выносливости. </param>
            public StatsRecord(float hpCurrentValue, float hpMaxValue, float staminaCurrentValue, float staminaMaxValue)
            {
                HpCurrentValue = hpCurrentValue;
                HpMaxValue = hpMaxValue;
                StaminaCurrentValue = staminaCurrentValue;
                StaminaMaxValue = staminaMaxValue;
            }
        }

        /// <summary> Сохранить текущее состояние всех статов. </summary>
        /// <returns> Объект с сохранёнными значениями. </returns>
        public object CaptureState()
        {
            var health = GetStat<Health>();
            var stamina = GetStat<Stamina>();

            return new StatsRecord(health?.CurrentValue ?? 0, health?.MaxValue ?? 0,
                stamina?.CurrentValue ?? 0, stamina?.MaxValue ?? 0);
        }

        /// <summary> Восстановить состояние всех статов из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённые данные. </param>
        public void RestoreState(object state)
        {
            if (state is not StatsRecord record) return;

            _stats.Clear();

            Register<Health>(new Health(record.HpCurrentValue, record.HpMaxValue), _healthView);
            Register<Stamina>(new Stamina(record.StaminaCurrentValue, record.StaminaMaxValue), _staminaView);
        }

        #endregion
    }
}