using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem.DropSystem;

[RequireComponent(typeof(ItemDropper))]
public class DestroyableResourceContainer : MonoBehaviour, IHitable, IDestroyable
{
    #region DestroyBehaviour

    public bool IsDestroyed { get; private set; }

    public event Action<IDestroyable> OnObjectDestroyed;

    /// <summary> Задержка перед окончательным уничтожением объекта. </summary>
    private const float DestroyDelay = 4f;

    public void Destroy()
    {
        IsDestroyed = true;
        gameObject.AddComponent(typeof(Rigidbody));
        OnObjectDestroyed?.Invoke(this);
        StartCoroutine(DestroyGameobjectAfterDelay());
    }

    private IEnumerator DestroyGameobjectAfterDelay()
    {
        yield return new WaitForSeconds(DestroyDelay);
        DropResources();
        Destroy(gameObject);
    }

    private void DropResources() => _dropItems.ForEach(item => _itemDropper.DropItem(item.ItemPrefab, item.Quantity));

    #endregion

    #region HitBehaviour

    /// <summary> Тип инструмента, необходимого для разрушения. </summary>
    [Tooltip("Тип инструмента, необходимого для разрушения."), SerializeField]
    private ToolType[] _toolsToBeHit;

    /// <summary> Количество ударов для разрушения объекта. </summary>
    [Tooltip("Количество ударов для каждой стадии объекта."), Range(1, 5), SerializeField]
    private int _hitsToDestroy;

    /// <summary> Текущее количество ударов по объекту. </summary>
    private int _hitsTaken;

    public void TakeHit(ToolType toolType)
    {
        if (IsDestroyed || !_toolsToBeHit.Contains(toolType)) return;

        _hitsTaken++;
        if (_hitsTaken >= _hitsToDestroy) Destroy();
    }

    #endregion

    /// <summary> Список предметов, которые выпадут при разрушении. </summary>
    [Tooltip("Список предметов, которые выпадут при разрушении."), SerializeField]
    private List<DropItem> _dropItems;

    /// <summary> Выбрасыватель предметов. </summary>
    private ItemDropper _itemDropper;

    private void Awake()
    {
        _itemDropper = GetComponent<ItemDropper>();
    }
}