using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary>
    /// Обработчик предметов для NPC, позволяющий визуализировать предметы в руках.
    /// Поддерживает простую систему экипировки/снятия предметов.
    /// </summary>
    public class ItemHandler : MonoBehaviour
    {
        [Header("References")] [Tooltip("Трансформ, куда будет помещаться экипированный предмет")] [SerializeField]
        private Transform _handSlot;

        [Header("Settings")] [Tooltip("Автоматически уничтожать старый предмет при экипировке нового")] [SerializeField]
        private bool _autoDestroyPreviousItem = true;

        private InventoryItem _currentItem;

        private GameObject _currentItemInstance;

        /// <summary> Событие при изменении экипированного предмета </summary>
        public event Action<InventoryItem> OnItemChanged;

        /// <summary> Экипировать указанный предмет. </summary>
        /// <param name="itemStack"> Предмет для экипировки. </param>
        public void EquipItem(ItemStack itemStack)
        {
            if (!itemStack.Item) return;

            UnequipItem();

            if (itemStack.Item.PickupPrefab)
            {
                _currentItemInstance = Instantiate(itemStack.Item.PickupPrefab.gameObject, _handSlot);
                _currentItem = itemStack.Item;
                OnItemChanged?.Invoke(_currentItem);
            }
            else
            {
                Debug.LogError(
                    $"Не удалось экипировать предмет {itemStack.Item.ItemName}: отсутствует PickupPrefab или ItemPrefab",
                    this);
            }
        }

        /// <summary> Снять текущий экипированный предмет. </summary>
        /// <param name="destroyItem"> Уничтожать ли экземпляр предмета. </param>
        public void UnequipItem(bool destroyItem = true)
        {
            if (!_currentItem) return;

            var previousItem = _currentItem;
            _currentItem = null;

            if (_currentItemInstance)
            {
                if (destroyItem || _autoDestroyPreviousItem)
                    Destroy(_currentItemInstance);
                else
                    _currentItemInstance.SetActive(false);
            }

            OnItemChanged?.Invoke(null);
        }
    }
}