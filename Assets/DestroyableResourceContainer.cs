using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem.DropSystem;

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
        if (IsDestroyed) return;
        
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

    public int HitsTaken { get; protected set; }

    public void TakeHit(ToolType toolType)
    {
        if (IsDestroyed || !_toolsToBeHit.Contains(toolType)) return;

        HitsTaken++;
        
        if (HitsTaken >= HitsToDestroy)
        {
            Destroy();
            return;
        }

        UpdateVisualGrade(true);
    }

    private void UpdateVisualGrade( bool canDropResources)
    {
        for (int i = _hitsToGrades.Count - 1; i >= 0; i--)
        {
            int cumulativeHits = _hitsToGrades.Take(i + 1).Sum();
            if (cumulativeHits <= HitsTaken)
            {
                _objectSwitcher.SwitchTo(i + 1);
                if (canDropResources && cumulativeHits == HitsTaken) DropResources();
                break;
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

    private void Awake() => Initialize();

    public void Initialize(int hitsTaken = 0)
    {
        _itemDropper = GetComponent<ItemDropper>();
        _objectSwitcher = GetComponent<ObjectSwitcher>();

        if (_objectSwitcher.GetObjectsCount() != _hitsToGrades.Count)
            Debug.LogError("Несоответствие между количеством грейдов и ударами!");

        _objectSwitcher.Initialize();
        
        HitsTaken = hitsTaken;
        UpdateVisualGrade(false);
    }
}