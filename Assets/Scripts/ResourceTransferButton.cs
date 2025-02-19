using System;
using UnityEngine;
using JetBrains.Annotations;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.UI;

/// <summary> Класс для кнопки, отвечающей за передачу ресурсов (добавление или возвращение). Наследуется от CustomButton. </summary>
public class ResourceTransferButton : CustomButton
{
    //Назначается один раз в инспекторе
    /// <summary> Тип кнопки для передачи ресурсов (добавление или возвращение). Назначается один раз в инспекторе. </summary>
    [SerializeField] private ResourceTransferButtonType _buttonType;

    /// <summary> Ресурс, связанный с кнопкой. Может быть null, если ресурс не назначен. </summary>
    [CanBeNull] private InventoryItem _resource;

    /// <summary> Событие, которое вызывается при нажатии на кнопку. </summary>
    public event Action<InventoryItem, ResourceTransferButtonType> OnClick;

    /// <summary> Устанавливает ресурс для кнопки. </summary>
    /// <param name="resource"> Ресурс, который будет привязан к кнопке. </param>
    public void SetResource(InventoryItem resource) => _resource = resource;

    /// <summary> Вызвать нажатие кнопки. </summary>
    /// <remarks> Будет использовано при реализации навигации по кнопкам вьюшки ремонта. </remarks>
    public void TriggerClick() => Click();

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
    protected override void Click()
    {
        if (_resource == null) Debug.LogError("Не назначен ресурс (Ремонт зданий/Требование)!");

        OnClick?.Invoke(_resource, _buttonType);
    }

    /// <summary> Вызывается при включении взаимодействия с кнопкой (в данной реализации ничего не делает). </summary>
    protected override void OnInteractionEnabled()
    {
    }

    /// <summary> Вызывается при отключении взаимодействия с кнопкой (в данной реализации ничего не делает). </summary>
    protected override void OnInteractionDisabled()
    {
    }
}