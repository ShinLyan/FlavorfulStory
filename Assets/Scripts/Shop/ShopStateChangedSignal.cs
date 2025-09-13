namespace FlavorfulStory.Shop
{
    /// <summary> Сигнал изменения состояния магазина. </summary>
    public class ShopStateChangedSignal
    {
        /// <summary> Состояние магазина (true — открыт, false — закрыт). </summary>
        public bool IsOpen { get; }

        /// <summary> Состояние изменено игроком. </summary>
        public bool ChangedByPlayer { get; }

        /// <summary> Создает новый сигнал изменения состояния магазина. </summary>
        /// <param name="isOpen"> Новое состояние магазина. </param>
        /// <param name="changedByPlayer"> Состояние изменено игроком. </param>
        public ShopStateChangedSignal(bool isOpen, bool changedByPlayer)
        {
            IsOpen = isOpen;
            ChangedByPlayer = changedByPlayer;
        }
    }
}