using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    [Serializable]
    public class ShopLocation : Location
    {
        [field: SerializeField] public CashDesk CashDesk { get; private set; }

        [SerializeField] private Shelf[] _shelves;

        [SerializeField] private Furniture[] _furnitures;

        public Furniture GetAvailableFurniture() => (Furniture)GetAvailableObjects(_furnitures);

        public Shelf GetAvailableShelf() => (Shelf)GetAvailableObjects(_shelves);

        private ShopObject GetAvailableObjects(ShopObject[] objects)
        {
            var availableObjects = new List<ShopObject>();
            foreach (var obj in objects)
                if (!obj.IsOccupied)
                    availableObjects.Add(obj);

            return availableObjects[Random.Range(0, availableObjects.Count)];
        }
    }
}