using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Shop
{
    public class Showcase : ShopObject
    {
        public Inventory Inventory { get; private set; }

        private void Awake() => Inventory = GetComponent<Inventory>();
    }
}