using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    /// <summary> Компонент игрока, управляющий атрибутами, их инициализацией и восстановлением. </summary>
    public class PlayerAttributes : MonoBehaviour
    {
        /// <summary> Представление атрибута здоровья. </summary>
        [SerializeField] private BaseAttributeView _healthAttributeView;
        /// <summary> Представление атрибута выносливости. </summary>
        [SerializeField] private BaseAttributeView _staminaAttributeView;

        /// <summary> Коллекция всех атрибутов, привязанных к типу. </summary>
        private readonly Dictionary<Type, IAttribute> _attributes = new();

        /// <summary> Отвязывает обработчики событий при уничтожении объекта. </summary>
        private void OnDestroy() => AttributeBinder.Unbind(GetAttribute<HealthAttribute>(), _healthAttributeView);

        /// <summary> Инициализирует атрибуты при создании объекта. </summary>
        private void Awake() => InitializeAttributes();

        /// <summary> Обновляет состояние регенерации атрибутов каждый кадр. </summary>
        private void Update() => HandleRegenTick();

        /// <summary> Обрабатывает восстановление всех регенерируемых атрибутов. </summary>
        private void HandleRegenTick()
        {
            foreach (var attribute in _attributes.Values)
            {
                if (attribute is IRegenerableAttribute regenerableAttribute)
                {
                    regenerableAttribute.TickRegen(Time.deltaTime);
                }
            }
        }

        /// <summary> Создаёт и инициализирует атрибуты игрока. </summary>
        private void InitializeAttributes()
        {
            //TODO: Мб где-то внутри HealthAttribute мб не просто создавать с нуля, а подтягивать инфу с сейва
            var health = new HealthAttribute(100f);
            AddAttribute(health);
            AttributeBinder.Bind(GetAttribute<HealthAttribute>(), _healthAttributeView);
            //TODO: где-то внутри InitializeView подтягивать инфу с сейва
            _healthAttributeView.InitializeView(health.CurrentValue, health.MaxValue);

            var stamina = new StaminaAttribute(150f, 1f);
            AddAttribute(stamina);
            AttributeBinder.Bind(GetAttribute<StaminaAttribute>(), _staminaAttributeView);
            _staminaAttributeView.InitializeView(stamina.CurrentValue, stamina.MaxValue);
            stamina.SetValue(0);
        }

        /// <summary> Добавляет атрибут в коллекцию. </summary>
        /// <param name="attribute"> Атрибут для добавления. </param>
        private void AddAttribute<T>(T attribute) where T : IAttribute
        {
            _attributes[typeof(T)] = attribute;
        }

        /// <summary> Получает атрибут указанного типа. </summary>
        /// <typeparam name="T"> Тип атрибута. </typeparam>
        /// <returns> Атрибут, если найден, иначе null. </returns>
        public T GetAttribute<T>() where T : class, IAttribute
        {
            _attributes.TryGetValue(typeof(T), out var attribute);
            return attribute as T;
        }
    }
}