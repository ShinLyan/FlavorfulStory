using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Audio;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.ObjectManagement;
using FlavorfulStory.Tools;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Добываемый объект. </summary>
    [RequireComponent(typeof(ObjectSwitcher))]
    public class DestroyableResourceContainer : MonoBehaviour, IDestroyable, IHitable
    {
        #region Fields and Properties

        /// <summary> Список стадий. </summary>
        [Tooltip("Список стадий."), SerializeField]
        private List<Stage> _stages;

        /// <summary> Задержка перед окончательным уничтожением объекта. </summary>
        [Tooltip("Задержка перед окончательным уничтожением объекта."), SerializeField]
        private float _destroyDelay;

        /// <summary> Сервис выброса предметов в мир. </summary>
        private IItemDropService _itemDropService;

        /// <summary> Переключатель грейдов. </summary>
        private ObjectSwitcher _objectSwitcher;

        /// <summary> Разрушен ли объект? </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary> Необходимое количество ударов для полного разрушения. </summary>
        private int _hitsToDestroy;

        /// <summary> Количество нанесенных ударов. </summary>
        private int _hitsTaken;

        /// <summary> Текущий индекс стадии объекта. </summary>
        private int _currentGradeIndex;

        /// <summary> Количество нанесенных ударов. </summary>
        public int HitsTaken
        {
            get => _hitsTaken;
            private set
            {
                _hitsTaken = value;
                _currentGradeIndex = CalculateCurrentGradeIndex();
            }
        }

        /// <summary> Событие, вызываемое при полном разрушении объекта. </summary>
        public event Action<IDestroyable> OnObjectDestroyed;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="itemDropService"> Сервис выброса предметов в мир. </param>
        [Inject]
        private void Construct(IItemDropService itemDropService) => _itemDropService = itemDropService;

        /// <summary> Инициализация объекта. </summary>
        private void Awake() => Initialize();

        /// <summary> Инициализировать переключатель стадий и обновить состояние объекта. </summary>
        /// <param name="hitsTaken"> Количество полученных ударов. </param>
        public void Initialize(int hitsTaken = 0)
        {
            foreach (var stage in _stages) _hitsToDestroy += stage.RequiredHits;

            _objectSwitcher = GetComponent<ObjectSwitcher>();

            if (_objectSwitcher.ObjectsCount != _stages.Count)
                Debug.LogError("Несоответствие количества грейдов и ударов!");

            _objectSwitcher.Initialize();
            HitsTaken = hitsTaken;
            _objectSwitcher.SwitchTo(_currentGradeIndex);
        }

        /// <summary> Рассчитать индекс текущей стадии объекта. </summary>
        /// <returns> Индекс текущей стадии. </returns>
        private int CalculateCurrentGradeIndex()
        {
            for (int i = 0, cumulativeHits = 0; i < _stages.Count; i++)
            {
                cumulativeHits += _stages[i].RequiredHits;
                if (HitsTaken < cumulativeHits) return i;
            }

            return _stages.Count - 1;
        }

        #region DestroyBehaviour

        /// <summary> Уничтожить объект. </summary>
        /// <param name="destroyDelay"> Задержка перед уничтожением. </param>
        public void Destroy(float destroyDelay = 0)
        {
            if (IsDestroyed) return;

            IsDestroyed = true;
            OnObjectDestroyed?.Invoke(this);
            StartCoroutine(DestroyGameObjectAfterDelay(destroyDelay));
        }

        /// <summary> Уничтожить объект после задержки. </summary>
        /// <param name="destroyDelay"> Задержка перед уничтожением. </param>
        /// <returns> Возвращает <see cref="IEnumerator" /> корутины. </returns>
        private IEnumerator DestroyGameObjectAfterDelay(float destroyDelay)
        {
            yield return new WaitForSeconds(destroyDelay);
            DropResourcesForCurrentGrade();
            Destroy(gameObject);
        }

        /// <summary> Выбросить ресурсы для текущей стадии объекта. </summary>
        private void DropResourcesForCurrentGrade()
        {
            const float ResourceDropForce = 5f;
            foreach (var itemStack in _stages[_currentGradeIndex].Items)
                _itemDropService.Drop(itemStack, GetDropPosition(), Vector3.up * ResourceDropForce);
        }

        /// <summary> Получить позицию дропа. </summary>
        /// <returns> Позиция дропа. </returns>
        private Vector3 GetDropPosition()
        {
            const float DropOffsetRange = 2f;
            float offsetX = Random.Range(-DropOffsetRange, DropOffsetRange);
            float offsetZ = Random.Range(-DropOffsetRange, DropOffsetRange);
            return transform.position + new Vector3(offsetX, 1, offsetZ);
        }

        #endregion

        #region IHitable

        /// <summary> Тип инструмента, необходимого для разрушения. </summary>
        [field: Tooltip("Тип инструмента, необходимого для разрушения."), SerializeField]
        public ToolType RequiredToolType { get; private set; }

        /// <summary> Минимальный уровень инструмента, необходимый для нанесения урона. </summary>
        [field: Tooltip("Минимальный уровень инструмента для нанесения урона."), SerializeField, Range(1f, 3f)]
        public int RequiredToolLevel { get; private set; }

        /// <summary> Тип звукового эффекта, проигрываемого при ударе. </summary>
        [field: Tooltip("Тип звукового эффекта, проигрываемого при ударе."), SerializeField]
        public SfxType SfxType { get; private set; }

        /// <summary> Может ли указанный инструмент нанести урон этому объекту? </summary>
        /// <param name="toolType"> Тип инструмента. </param>
        /// <param name="toolLevel"> Уровень инструмента. </param>
        /// <returns> <c>true</c>, если удар возможен; иначе <c>false</c>. </returns>
        public bool CanBeHitBy(ToolType toolType, int toolLevel) =>
            toolType == RequiredToolType && toolLevel >= RequiredToolLevel;

        /// <summary> Применяет удар к объекту. </summary>
        public void TakeHit()
        {
            if (IsDestroyed) return;

            HitsTaken++;
            if (HitsTaken >= _hitsToDestroy)
            {
                Destroy(_destroyDelay);
                return;
            }

            int hitsToGrade = _stages.Take(_currentGradeIndex).Sum(stage => stage.RequiredHits);
            if (hitsToGrade != HitsTaken) return;

            _objectSwitcher.SwitchTo(_currentGradeIndex);
            DropResourcesForCurrentGrade();
        }

        #endregion
    }
}