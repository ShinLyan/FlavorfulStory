using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using FlavorfulStory.InventorySystem.PickupSystem;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Отображает содержимое инвентаря, размещая PickupPrefab в указанных слотах.
    /// Используется для сундуков, полок и других объектов с фиксированным числом визуальных позиций. </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryTransformPlacer : MonoBehaviour
    {
        /// <summary> Точки размещения предметов. </summary>
        [Tooltip("Точки размещения предметов")]
        [SerializeField] private List<Transform> _slots = new();

        /// <summary> Сопоставление индекса слота и размещённого объекта. </summary>
        private readonly Dictionary<int, GameObject> _occupied = new();

        /// <summary> Инвентарь, содержимое которого визуализируется. </summary>
        private Inventory _inventory;
        
        /// <summary> Активные tween-последовательности анимации слотов. </summary>
        private readonly List<Sequence> _slotTweens = new();

        /// <summary> Подписывается на обновление инвентаря. </summary>
        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _inventory.InventoryUpdated += RefreshVisuals;
        }

        /// <summary> При старте обновляет визуальное представление. </summary>
        private void Start()
        {
            AnimateSlots();
            RefreshVisuals();
        }
            

        /// <summary> Отписывается от событий при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.InventoryUpdated -= RefreshVisuals;
            
            foreach (var tween in _slotTweens)
                tween.Kill();
            
            _slotTweens.Clear();
        }

        /// <summary> Обновляет визуальное представление на основе текущего инвентаря. </summary>
        private void RefreshVisuals()
        {
            ClearAll();
            int placedCount = 0;

            for (int i = 0; i < _inventory.InventorySize && placedCount < _slots.Count; i++)
            {
                var stack = _inventory.GetItemStackInSlot(i);
                if (stack.Item == null || stack.Number <= 0 || stack.Item.PickupPrefab == null)
                    continue;

                var instance = Instantiate(stack.Item.PickupPrefab.gameObject);
                PrepareInstance(instance);
                SetSlot(placedCount, instance);
                placedCount++;
            }
        }

        /// <summary> Запускает анимации вращения и подпрыгивания для всех слотов. </summary>
        private void AnimateSlots()
        {
            float wiggleAmount = 0.05f;
            float wiggleDuration = 1.5f;
            float rotationDuration = 5f;

            foreach (var slot in _slots)
            {
                if (slot == null) continue;

                var sequence = DOTween.Sequence();
                
                sequence.Join(slot.DOLocalRotate(
                        new Vector3(0, 360, 0),
                        rotationDuration,
                        RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                );
                
                sequence.Join(slot.DOLocalMoveY(slot.localPosition.y + wiggleAmount, wiggleDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                );

                _slotTweens.Add(sequence);
            }
        }
        
        /// <summary> Отключает физику и компоненты подбора на объекте. </summary>
        private static void PrepareInstance(GameObject go)
        {
            if (go.TryGetComponent(out Collider collider)) collider.enabled = false;

            if (go.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }

            if (go.TryGetComponent(out Pickup pickup)) pickup.enabled = false;
        }

        /// <summary> Устанавливает объект в слот с указанным индексом. </summary>
        /// <param name="index"> Индекс слота. </param>
        /// <param name="product"> Товар(объект) который нужно положить в полку маггазина. </param>
        private void SetSlot(int index, GameObject product)
        {
            if (index < 0 || index >= _slots.Count || product == null)
            {
                Debug.LogWarning($"[InventoryTransformPlacer] Неверный индекс ({index}) или instance == null");
                return;
            }

            ClearSlot(index);

            product.transform.SetParent(_slots[index], worldPositionStays: false);
            product.transform.SetPositionAndRotation(_slots[index].position, _slots[index].rotation);

            _occupied[index] = product;
        }

        /// <summary> Удаляет объект из указанного слота. </summary>
        private void ClearSlot(int index)
        {
            if (_occupied.TryGetValue(index, out var go) && go)
                Destroy(go);

            _occupied.Remove(index);
        }

        /// <summary> Удаляет все размещённые объекты. </summary>
        private void ClearAll()
        {
            foreach (var go in _occupied.Values.Where(x => x)) 
                Destroy(go);

            _occupied.Clear();
        }
    }
}