using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FlavorfulStory.InventorySystem.PickupSystem;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Визуализирует содержимое инвентаря, размещая PickupPrefab в указанных слотах. </summary>
    /// <remarks> Используется для прилавков магазина с фиксированным числом визуальных позиций. </remarks>
    [RequireComponent(typeof(Inventory))]
    public class InventoryTransformPlacer : MonoBehaviour
    {
        /// <summary> Слоты для размещения предметов. </summary>
        [Tooltip("Слоты для размещения предметов."), SerializeField]
        private List<Transform> _placementSlots;

        /// <summary> Инвентарь, содержимое которого визуализируется. </summary>
        private Inventory _inventory;

        /// <summary> Сопоставление индекса слота и размещённого объекта. </summary>
        private readonly Dictionary<int, GameObject> _itemsInSlots = new();

        /// <summary> Активные анимации слотов. </summary>
        private readonly List<Tween> _slotAnimations = new();

        /// <summary> Подписка на обновление инвентаря и валидация количества слотов. </summary>
        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _inventory.InventoryUpdated += UpdateView;

            if (_inventory.InventorySize != _placementSlots.Count)
                Debug.LogError("Несоответствие размера инвентаря и количества точек для размещения предметов.");
        }

        /// <summary> Запускает анимации и визуализирует содержимое инвентаря. </summary>
        private void Start()
        {
            AnimateSlots();
            UpdateView();
        }

        /// <summary> Очистка ссылок и анимаций при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            if (_inventory) _inventory.InventoryUpdated -= UpdateView;

            foreach (var tween in _slotAnimations) tween.Kill();

            _slotAnimations.Clear();
        }

        /// <summary> Обновляет визуальное представление на основе текущего инвентаря. </summary>
        private void UpdateView()
        {
            ClearSlots();

            for (int i = 0; i < _inventory.InventorySize; i++)
            {
                var itemStack = _inventory.GetItemStackInSlot(i);
                if (!itemStack.Item) continue;

                var instance = Instantiate(itemStack.Item.PickupPrefab.gameObject);
                PrepareObject(instance);
                PlaceInSlot(i, instance);
            }
        }

        /// <summary> Запускает анимации вращения и подпрыгивания для всех слотов. </summary>
        private void AnimateSlots()
        {
            const float RotationDuration = 5f;
            const float WiggleAmount = 0.05f;
            const float WiggleDuration = 1.5f;

            foreach (var slot in _placementSlots)
            {
                var rotationTween = slot
                    .DOLocalRotate(new Vector3(0f, 360f, 0f), RotationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

                var wiggleTween = slot.DOLocalMoveY(slot.localPosition.y + WiggleAmount, WiggleDuration)
                    .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

                _slotAnimations.Add(rotationTween);
                _slotAnimations.Add(wiggleTween);
            }
        }

        /// <summary> Подготавливает объект к визуальному размещению: отключает физику и взаимодействие. </summary>
        /// <param name="obj"> Игровой объект предмета. </param>
        private static void PrepareObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Collider collider)) collider.enabled = false;

            if (obj.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }

            if (obj.TryGetComponent(out Pickup pickup)) pickup.enabled = false;
        }

        /// <summary> Устанавливает объект в слот с указанным индексом. </summary>
        /// <param name="slotIndex"> Индекс слота. </param>
        /// <param name="obj"> Объект, который нужно разместить. </param>
        private void PlaceInSlot(int slotIndex, GameObject obj)
        {
            RemoveFromSlot(slotIndex);

            var placementSlot = _placementSlots[slotIndex];
            obj.transform.SetParent(placementSlot, false);
            obj.transform.SetPositionAndRotation(placementSlot.position, placementSlot.rotation);

            _itemsInSlots[slotIndex] = obj;
        }

        /// <summary> Удаляет объект из указанного слота. </summary>
        /// <param name="index"> Индекс слота. </param>
        private void RemoveFromSlot(int index)
        {
            if (_itemsInSlots.TryGetValue(index, out var obj) && obj) Destroy(obj);

            _itemsInSlots.Remove(index);
        }

        /// <summary> Удаляет все размещённые объекты из слотов. </summary>
        private void ClearSlots()
        {
            foreach (var go in _itemsInSlots.Values.Where(x => x)) Destroy(go);

            _itemsInSlots.Clear();
        }
    }
}