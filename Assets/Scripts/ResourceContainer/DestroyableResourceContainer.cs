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
        [Tooltip("Список предметов, которые выпадут при разрушении."), SerializeField]
        private List<DropItemsForGrade> _dropItems;

        /// <summary> Задержка перед окончательным уничтожением объекта. </summary>
        [Tooltip("Задержка перед окончательным уничтожением объекта."), SerializeField]
        private float _destroyDelay = 4f;

        /// <summary> Тип инструмента, необходимого для разрушения. </summary>
        [Tooltip("Тип инструмента, необходимого для разрушения."), SerializeField]
        private ToolType[] _toolsToBeHit;

        /// <summary> Количество ударов для разрушения объекта. </summary>
        [Tooltip("Количество ударов для каждой стадии объекта."), SerializeField, Range(1, 5)]
        private List<int> _hitsToGrades;

        /// <summary> Выбрасыватель предметов. </summary>
        private ItemDropper _itemDropper;

        /// <summary> Переключатель грейдов. </summary>
        private ObjectSwitcher _objectSwitcher;

        public bool IsDestroyed { get; private set; }

        /// <summary> Необходимое количество ударов для уничтожения. </summary>
        private int _hitsToDestroy;

        private int _hitsTaken;

        private int _currentGradeIndex;

        /// <summary> Количество нанесенных ударов. </summary>
        public int HitsTaken
        {
            get => _hitsTaken;
            private set
            {
                _hitsTaken = value;
                _currentGradeIndex = CountCurrentGradeIndex();
            }
        }

        /// <summary> Получить индекс текущего грейда. </summary>
        /// <returns> Индекс текущего грейда. </returns>
        private int CountCurrentGradeIndex()
        {
            for (int i = 0, cumulativeHits = 0; i < _hitsToGrades.Count; i++)
            {
                cumulativeHits += _hitsToGrades[i];
                if (HitsTaken < cumulativeHits)
                    return i;
            }

            return _hitsToGrades.Count - 1;
        }

        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Инициализировать добываемый объект. </summary>
        private void Awake() => Initialize();

        /// <summary> Инициализировать переключатель грейдов и обновить грейд. </summary>
        /// <param name="hitsTaken"> Количество полученных ударов. </param>
        public void Initialize(int hitsTaken = 0)
        {
            _hitsToDestroy = _hitsToGrades.Sum();
            _itemDropper = GetComponent<ItemDropper>();
            _objectSwitcher = GetComponent<ObjectSwitcher>();

            if (_objectSwitcher.ObjectsCount != _hitsToGrades.Count)
                Debug.LogError("Несоответствие между количеством грейдов и ударами!");

            _objectSwitcher.Initialize();
            HitsTaken = hitsTaken;
            _objectSwitcher.SwitchTo(_currentGradeIndex);
        }

        #region DestroyBehaviour

        public void Destroy()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;
            OnObjectDestroyed?.Invoke(this);
            StartCoroutine(DestroyGameObjectAfterDelay());
        }

        /// <summary> Уничтожить объект после задержки <see cref="_destroyDelay" />. </summary>
        /// <returns> Возвращает <see cref="IEnumerator" /> корутины. </returns>
        private IEnumerator DestroyGameObjectAfterDelay()
        {
            yield return new WaitForSeconds(_destroyDelay);
            DropResourcesForCurrentGrade();
            Destroy(gameObject);
        }

        /// <summary> Выбросить ресурсы для текущего грейда. </summary>
        private void DropResourcesForCurrentGrade()
        {
            foreach (var item in _dropItems[_currentGradeIndex].Items)
                _itemDropper.DropItem(item.ItemPrefab, item.Quantity);
        }

        #endregion

        #region HitBehaviour

        public void TakeHit(ToolType toolType)
        {
            if (IsDestroyed || !_toolsToBeHit.Contains(toolType)) return;

            HitsTaken++;
            if (HitsTaken >= _hitsToDestroy)
            {
                Destroy();
                return;
            }

            int hitsToGrade = _hitsToGrades.Take(_currentGradeIndex).Sum();
            if (hitsToGrade != HitsTaken) return;

            _objectSwitcher.SwitchTo(_currentGradeIndex);
            DropResourcesForCurrentGrade();
        }

        #endregion
    }
}