namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Перечисление возможных состояний конечного автомата NPC. </summary>
    public enum StateName
    {
        /// <summary> Состояние бездействия. </summary>
        Idle,

        /// <summary> Состояние выполнения рутинных действий. </summary>
        Routine,

        /// <summary> Состояние ожидания. </summary>
        Waiting,

        /// <summary> Состояние перемещения. </summary>
        Movement,

        /// <summary> Состояние взаимодействия с объектами или персонажами. </summary>
        Interaction,

        /// <summary> Состояние воспроизведения анимации. </summary>
        Animation,

        /// <summary> Состояние выбора предмета. </summary>
        ItemPicker,

        /// <summary> Состояние выбора мебели. </summary>
        FurniturePicker,

        /// <summary> Состояние выбора случайной точки. </summary>
        RandomPointPicker,

        /// <summary> Состояние выбора прилавка. </summary>
        ShelfPicker,

        /// <summary> Состояние отказа от предмета. </summary>
        RefuseItem,

        /// <summary> Состояние оплаты. </summary>
        Payment,

        /// <summary> Последовательность действий для выбора случайной точки. </summary>
        RandomPointSequence,

        /// <summary> Последовательность действий с мебелью. </summary>
        FurnitureSequence,

        /// <summary> Последовательность действий покупки предмета. </summary>
        BuyItemSequence,

        /// <summary> Последовательность действий отказа от предмета. </summary>
        RefuseItemSequence
    }
}