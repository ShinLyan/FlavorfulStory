namespace FlavorfulStory.Shop
{
    /// <summary> Сигнал изменения состояния магазина. </summary>
    public class ShopStateChangedSignal
    {
        /// <summary> Состояние магазина (true — открыт, false — закрыт). </summary>
        public bool IsOpen { get; }

        /// <summary> Создает новый сигнал изменения состояния магазина. </summary>
        /// <param name="isOpen"> Новое состояние магазина. </param>
        public ShopStateChangedSignal(bool isOpen) => IsOpen = isOpen;
    }
}