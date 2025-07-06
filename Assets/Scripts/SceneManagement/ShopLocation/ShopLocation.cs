using System;
using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    [Serializable]
    public class ShopLocation : Location
    {
        [field: SerializeField] public CashDesk CashDesk { get; private set; }

        [SerializeField] private Shelf[] _shelves;

        [SerializeField] private Furniture[] _furnitures;

        public Furniture[] GetAvailableFurniture()
        {
            return _furnitures; //TODO: реализовать метод
        }

        public Shelf[] GetAvailableShelves()
        {
            return _shelves; //TODO: реализовать метод
        }
    }
}