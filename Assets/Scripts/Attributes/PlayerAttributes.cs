using System;
using System.Collections.Generic;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    /// <summary> Компонент игрока, управляющий атрибутами, их инициализацией и восстановлением. </summary>
    public class PlayerAttributes : MonoBehaviour, ISaveable
    {
        /// <summary> Представление атрибута здоровья. </summary>
        [SerializeField] private BaseAttributeView _healthView;

        /// <summary> Представление атрибута выносливости. </summary>
        [SerializeField] private BaseAttributeView _staminaView;

        /// <summary> Коллекция всех атрибутов, привязанных к типу. </summary>
        private readonly Dictionary<Type, IAttribute> _attributes = new();

        private AttributesData? _loadedData;

        private void Awake()
        {
            if (_loadedData.HasValue)
            {
                var data = _loadedData.Value;

                Register(new HealthAttribute(data.HpMaxValue, data.HpCurrentValue), _healthView);
                Register(new StaminaAttribute(data.staminaMaxValue, data.staminaCurrentValue, data.staminaRegenRate),
                    _staminaView);
            }
            else
            {
                RegisterAttributes();
            }
        }

        private void Update()
        {
            foreach (var attribute in _attributes.Values)
                if (attribute is IRegenerableAttribute regen)
                    regen.TickRegen(Time.deltaTime);
        }

        private void OnDestroy()
        {
            foreach (var attributeKVP in _attributes)
                if (TryGetView(attributeKVP.Key, out var view))
                    AttributeBinder.Unbind(attributeKVP.Value, view);
        }

        private void RegisterAttributes()
        {
            Register(new HealthAttribute(100), _healthView);
            Register(new StaminaAttribute(150f, 1f), _staminaView);
        }

        private void Register<T>(T attribute, BaseAttributeView view) where T : IAttribute
        {
            _attributes[typeof(T)] = attribute;
            AttributeBinder.Bind(attribute, view);
            view.Initialize(attribute.CurrentValue, attribute.MaxValue);
        }

        public T GetAttribute<T>() where T : class, IAttribute =>
            _attributes.TryGetValue(typeof(T), out var attribute) ? attribute as T : null;

        private bool TryGetView(Type type, out BaseAttributeView view)
        {
            if (type == typeof(HealthAttribute))
                view = _healthView;
            else if (type == typeof(StaminaAttribute))
                view = _staminaView;
            else
                view = null;

            return view != null;
        }

        #region ISaveable

        [Serializable]
        private struct AttributesData
        {
            public float HpCurrentValue;
            public float HpMaxValue;
            public float staminaCurrentValue;
            public float staminaMaxValue;
            public float staminaRegenRate;
        }

        public object CaptureState()
        {
            var health = GetAttribute<HealthAttribute>();
            var stamina = GetAttribute<StaminaAttribute>();

            return new AttributesData
            {
                HpCurrentValue = health?.CurrentValue ?? 0,
                HpMaxValue = health?.MaxValue ?? 0,
                staminaCurrentValue = stamina?.CurrentValue ?? 0,
                staminaMaxValue = stamina?.MaxValue ?? 0,
                staminaRegenRate = stamina?.RegenRate ?? 0
            };
        }

        public void RestoreState(object state)
        {
            if (state is AttributesData data) _loadedData = data;
        }

        #endregion
    }
}