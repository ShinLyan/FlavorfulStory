using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Shop
{
    /// <summary> Витрина магазина с инвентарем. </summary>
    public class Showcase : ShopObject
    {
        /// <summary> Инвентарь витрины. </summary>
        public Inventory Inventory { get; private set; }

        /// <summary> Инициализирует инвентарь при создании. </summary>
        private void Awake() => Inventory = GetComponent<Inventory>();
    }
}