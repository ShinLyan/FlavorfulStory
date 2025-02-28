using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem.DropSystem;

//TODO: сделать сохранение (_hitsTaken). Зародителить и сделать 2 наследника ObjectSpawner(default & resourceContainer)?
[RequireComponent(typeof(ItemDropper), typeof(ObjectSwitcher))]
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
    private List<int> _hitsToGrades;

    private int HitsToDestroy => _hitsToGrades.Sum();

    private int _hitsTaken;

    public void TakeHit(ToolType toolType)
    {
        if (IsDestroyed || !_toolsToBeHit.Contains(toolType)) return;

        _hitsTaken++;
        if (_hitsTaken >= HitsToDestroy)
        {
            Destroy();
            return;
        }
        
        SwitchToCorrectGameobject(_hitsTaken);
    }

    private void SwitchToCorrectGameobject(int hitsTaken)
    {
        for (int i = _hitsToGrades.Count-1; i >= 0; i--)
        {
            if (_hitsToGrades.Take(i + 1).Sum() == hitsTaken)
            {
                _objectSwitcher.SwitchToGameobject(i + 1);
                DropResources();
            }
        }
    }

    #endregion

    /// <summary> Список предметов, которые выпадут при разрушении. </summary>
    [Tooltip("Список предметов, которые выпадут при разрушении."), SerializeField]
    private List<DropItem> _dropItems;

    /// <summary> Выбрасыватель предметов. </summary>
    private ItemDropper _itemDropper;

    private ObjectSwitcher _objectSwitcher;

    private void Awake()
    {
        _itemDropper = GetComponent<ItemDropper>();
        _objectSwitcher = GetComponent<ObjectSwitcher>();
        
        if (_objectSwitcher.GetObjectsCount() != _hitsToGrades.Count)
            Debug.LogError("Несоответствие между количеством грейдов и ударами!");
        
        _objectSwitcher.Initialize();
    }
    }