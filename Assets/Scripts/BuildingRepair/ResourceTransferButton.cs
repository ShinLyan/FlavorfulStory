using UnityEngine;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.UI;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Класс для кнопки, отвечающей за передачу ресурсов (добавление или возвращение).
    /// Наследуется от CustomButton. </summary>
    public class ResourceTransferButton : CustomButton
    {
        /// <summary> Тип кнопки для передачи ресурсов (добавление или возвращение). </summary>
        [SerializeField] private ResourceTransferButtonType _buttonType;

        /// <summary> Ресурс, связанный с кнопкой. Может быть null, если ресурс не назначен. </summary>
        private InventoryItem _resource;

        /// <summary> Событие, которое вызывается при нажатии на кнопку. </summary>
        public event System.Action<InventoryItem, ResourceTransferButtonType> OnClick;

        /// <summary> Инициализация кнопки (в данной реализации ничего не делает). </summary>
        protected override void Initialize()
        {
        }

        /// <summary> Вызывается при начале наведения курсора на кнопку (в данной реализации ничего не делает). </summary>
        protected override void HoverStart()
        {
        }

        /// <summary> Вызывается при окончании наведения курсора на кнопку (в данной реализации ничего не делает). </summary>
        protected override void HoverEnd()
        {
        }

        /// <summary> Обработчик события клика по кнопке. Проверяет наличие ресурса и вызывает событие OnClick. </summary>
        protected override void Click() => OnClick?.Invoke(_resource, _buttonType);

        /// <summary> Вызывается при включении взаимодействия с кнопкой (в данной реализации ничего не делает). </summary>
        protected override void OnInteractionEnabled()
        {
        }

        /// <summary> Вызывается при отключении взаимодействия с кнопкой (в данной реализации ничего не делает). </summary>
        protected override void OnInteractionDisabled()
        {
        }

        /// <summary> Устанавливает ресурс для кнопки. </summary>
        /// <param name="resource"> Ресурс, который будет привязан к кнопке. </param>
        public void SetResource(InventoryItem resource) => _resource = resource;

        /// <summary> Вызвать нажатие кнопки. </summary>
        /// <remarks> Будет использовано при реализации навигации по кнопкам вьюшки ремонта. </remarks>
        public void TriggerClick() => Click();
    }
}