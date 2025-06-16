using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory
{
    public class ItemDropService : IItemDropService, ISaveable
    {
        private readonly PickupFactory _pickupFactory;
        private readonly List<Pickup> _spawnedPickups = new();

        public ItemDropService(PickupFactory pickupFactory)
        {
            _pickupFactory = pickupFactory;
        }

        private Pickup Spawn(InventoryItem item, int quantity, Vector3 position)
        {
            var pickup = _pickupFactory.Create(item, position, quantity);
            if (pickup != null)
                _spawnedPickups.Add(pickup);
            return pickup;
        }

        private void ApplyForce(Pickup pickup, Vector3 force)
        {
            var rb = pickup.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(force, ForceMode.Impulse);
        }

        #region Drop Methods

        public void Drop(InventoryItem item, int quantity, Vector3 position)
        {
            Spawn(item, quantity, position);
        }

        public void Drop(InventoryItem item, int quantity, Vector3 position, Vector3 force)
        {
            var pickup = Spawn(item, quantity, position);
            if (pickup) ApplyForce(pickup, force);
        }

        public void DropFromInventory(Inventory inventory, InventoryItem item, int quantity, Vector3 position)
        {
            if (inventory.GetItemNumber(item) < quantity)
            {
                Debug.LogError($"[ItemDropService] Недостаточно предметов {item.name} для выброса.");
                return;
            }

            inventory.RemoveItem(item, quantity);
            Drop(item, quantity, position);
        }

        public void DropFromInventory(Inventory inventory, InventoryItem item, int quantity, Vector3 position, Vector3 force)
        {
            if (inventory.GetItemNumber(item) < quantity)
            {
                Debug.LogError($"[ItemDropService] Недостаточно предметов {item.name} для выброса.");
                return;
            }

            inventory.RemoveItem(item, quantity);
            Drop(item, quantity, position, force);
        }

        #endregion

        #region Saving

        [Serializable]
        private struct DropSaveData
        {
            public string ItemID;
            public SerializableVector3 Position;
            public int Quantity;
        }

        public object CaptureState()
        {
            _spawnedPickups.RemoveAll(p => p == null);
            return _spawnedPickups.Select(p => new DropSaveData
            {
                ItemID = p.Item.ItemID,
                Position = new SerializableVector3(p.transform.position),
                Quantity = p.Number
            }).ToList();
        }

        public void RestoreState(object state)
        {
            if (state is not List<DropSaveData> records) return;

            foreach (var record in records)
            {
                var item = ItemDatabase.GetItemFromID(record.ItemID);
                if (item != null)
                    Spawn(item, record.Quantity, record.Position.ToVector());
            }
        }

        #endregion
    }
}