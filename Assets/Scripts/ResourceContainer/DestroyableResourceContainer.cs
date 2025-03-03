using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.ObjectManagement;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Добываемый объект. </summary>
    /// <remarks> Реализует интерфейсы <see cref="IHitable" /> и <see cref="IDestroyable" />. </remarks>
    [RequireComponent(typeof(ItemDropper), typeof(ObjectSwitcher))]
    public class DestroyableResourceContainer : MonoBehaviour, IHitable, IDestroyable
    {
        /// <summary> Список предметов, которые выпадут при разрушении. </summary>
        [Tooltip("Список предметов, которые выпадут при разрушении.")] [SerializeField]
        private List<DropItemsForGrade> _dropItems;

        /// <summary> Выбрасыватель предметов. </summary>
        private ItemDropper _itemDropper;

        /// <summary> Переключатель грейдов.  </summary>
        private ObjectSwitcher _objectSwitcher;

        /// <summary> Инициализировать добываемый объект. </summary>
        /// <remarks> Коллбэк из UnityAPI. </remarks>
        private void Awake()
        {
            Initialize();
        }

        /// <summary> Инициализировать переключатель грейдов и обновить грейд. </summary>
        /// <param name="hitsTaken"> Количество полученных ударов. </param>
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

        #region DestroyBehaviour

        public bool IsDestroyed { get; private set; }

        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Задержка перед окончательным уничтожением объекта. </summary>
        [SerializeField] private float _destroyDelay = 4f;

        public void Destroy()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;
            OnObjectDestroyed?.Invoke(this);
            StartCoroutine(DestroyGameobjectAfterDelay());
        }

        /// <summary> Уничтожить объект после задержки <see cref="_destroyDelay" />. </summary>
        /// <returns> Возвращает <see cref="IEnumerator" /> корутины. </returns>
        private IEnumerator DestroyGameobjectAfterDelay()
        {
            yield return new WaitForSeconds(_destroyDelay);
            DropResourcesForCurrentGrade();
            Destroy(gameObject);
        }

        /// <summary> Выбросить ресурсы для текущего грейда. </summary>
        private void DropResourcesForCurrentGrade()
        {
            //_dropItems.ForEach(item => _itemDropper.DropItem(item.ItemPrefab, item.Quantity));
            foreach (var item in _dropItems[GetCurrentGradeIndex()].Items)
                _itemDropper.DropItem(item.ItemPrefab, item.Quantity);
        }

        #endregion

        #region HitBehaviour

        /// <summary> Тип инструмента, необходимого для разрушения. </summary>
        [Tooltip("Тип инструмента, необходимого для разрушения.")] [SerializeField]
        private ToolType[] _toolsToBeHit;

        /// <summary> Количество ударов для разрушения объекта. </summary>
        [Tooltip("Количество ударов для каждой стадии объекта.")] [Range(1, 5)] [SerializeField]
        private List<int> _hitsToGrades;

        /// <summary> Необходимое количество ударов для уничтожения. </summary>
        private int HitsToDestroy => _hitsToGrades.Sum();

        /// <summary> Количество нанесенных ударов. </summary>
        public int HitsTaken { get; private set; }

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

        /// <summary> Обновить визуальный грейд. </summary>
        /// <remarks> Дропает ресурсы при смене грейда, если флаг (аргумент) позволяет. </remarks>
        /// <param name="canDropResources"> Флаг возможности дропнуть ресурсы. </param>
        private void UpdateVisualGrade(bool canDropResources)
        {
            for (var i = _hitsToGrades.Count - 1; i >= 0; i--)
            {
                var cumulativeHits = _hitsToGrades.Take(i + 1).Sum();
                if (cumulativeHits <= HitsTaken)
                {
                    _objectSwitcher.SwitchTo(i + 1);
                    if (canDropResources && cumulativeHits == HitsTaken) DropResourcesForCurrentGrade();
                    break;
                }
            }
        }

        /// <summary> Получить индекс текущего грейда. </summary>
        /// <returns> Индекс текущего грейда. </returns>
        private int GetCurrentGradeIndex()
        {
            for (var i = _hitsToGrades.Count - 1; i >= 0; i--)
            {
                var cumulativeHits = _hitsToGrades.Take(i + 1).Sum();
                if (cumulativeHits <= HitsTaken) return i;
            }

            return 0;
        }

        #endregion
    }
}