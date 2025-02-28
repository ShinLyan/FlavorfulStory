using System.Collections.Generic;
using FlavorfulStory.Actions;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InventorySystem;
using UnityEngine;

/// <summary> Абстрактный класс для объекта взаимодействия. </summary>
/// <remarks> Реализует интерфейс IInteractable. </remarks>
public abstract class AbstractInteractableObject : MonoBehaviour, IInteractable
{
    /// <summary> Предметы, что будут добавлены в инвентарь при взаимодействии. </summary>
    [SerializeField] private List<DropItem> _dropItems;
    
    /// <summary> Название объекта, отображаемое в тултипе. </summary>
    // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
    private const string Name = "Wending stump";

    /// <summary> Описание объекта, отображаемое в тултипе. </summary>
    // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
    private const string Description = "Press E, if hungry";

    /// <summary> Проверяет, доступно ли взаимодействие с объектом. </summary>
    public virtual bool IsInteractionAllowed { get; set; } = true;
    
    /// <summary> Возвращает расстояние до другого трансформа. </summary>
    /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
    /// <returns> Расстояние между объектами. </returns>
    public float GetDistanceTo(Transform otherTransform) =>
        Vector3.Distance(transform.position, otherTransform.position);

    /// <summary> Выполняет взаимодействие с объектом. </summary>
    public virtual void Interact()
    {
        if (!IsInteractionAllowed) return;

        IsInteractionAllowed = false;
        foreach (var dropItem in _dropItems)
        {
            Inventory.PlayerInventory.TryAddToFirstAvailableSlot(dropItem.ItemPrefab, dropItem.Quantity);
        }
    }

    /// <summary> Возвращает название для тултипа. </summary>
    /// <returns> Название объекта. </returns>
    public string GetTooltipTitle() => Name;

    /// <summary> Возвращает описание для тултипа. </summary>
    /// <returns> Описание объекта. </returns>
    public string GetTooltipDescription() => Description;

    /// <summary> Возвращает позицию объекта в мировых координатах. </summary>
    /// <returns> Мировая позиция объекта. </returns>
    public Vector3 GetWorldPosition() => transform.position;
}