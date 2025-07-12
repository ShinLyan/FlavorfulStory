using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class ShopLocation : Location
    {
        [field: SerializeField] public CashDesk CashDesk { get; private set; }

        [SerializeField] private ShopObject[] _shelves;

        [SerializeField] private ShopObject[] _furnitures;

        public Furniture GetAvailableFurniture() => (Furniture)GetRandomObject(_furnitures);

        public Shelf GetAvailableShelf() => (Shelf)GetRandomObject(_shelves);

        private static ShopObject[] GetAvailableObjects(ShopObject[] objects)
        {
            var availableObjects = new List<ShopObject>();
            foreach (var obj in objects)
                if (!obj.IsOccupied)
                    availableObjects.Add(obj);

            return availableObjects.ToArray();
        }

        private static ShopObject GetRandomObject(ShopObject[] objects)
        {
            var availableObjects = GetAvailableObjects(objects);

            if (availableObjects.Length == 0) return null;

            return availableObjects[Random.Range(0, availableObjects.Length)];
        }


        public bool AreAvailableShelvesEmpty()
        {
            var shelves = GetAvailableObjects(_shelves);
            // TODO: Реализация проверки, есть ли на полках товары
            return shelves.Length == 0;
        }

        public bool AreAllFurnitureOccupied()
        {
            var furnitures = GetAvailableObjects(_furnitures);
            return furnitures.Length == 0;
        }
    }
}