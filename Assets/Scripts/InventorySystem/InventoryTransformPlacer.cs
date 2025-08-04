using System.Collections.Generic;
using FlavorfulStory.InventorySystem.PickupSystem;
using UnityEngine;

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

        /// <summary> Подписывается на обновление инвентаря. </summary>
        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _inventory.InventoryUpdated += RefreshVisuals;
        }

        /// <summary> При старте обновляет визуальное представление. </summary>
        private void Start() => RefreshVisuals();

        /// <summary> Отписывается от событий при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.InventoryUpdated -= RefreshVisuals;
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
        private void SetSlot(int index, GameObject instance)
        {
            if (index < 0 || index >= _slots.Count || instance == null)
            {
                Debug.LogWarning($"[InventoryTransformPlacer] Неверный индекс ({index}) или instance == null");
                return;
            }

            ClearSlot(index);

            instance.transform.SetParent(_slots[index], worldPositionStays: false);
            instance.transform.SetPositionAndRotation(_slots[index].position, _slots[index].rotation);

            _occupied[index] = instance;
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
            foreach (var obj in _occupied.Values)
                if (obj) Destroy(obj);

            _occupied.Clear();
        }
    }
}