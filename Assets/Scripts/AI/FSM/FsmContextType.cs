namespace FlavorfulStory.AI.FSM
{
    /// <summary> Типы контекстных данных, используемых для передачи между состояниями конечного автомата. </summary>
    public enum FsmContextType
    {
        /// <summary> Покупаемый предмет. </summary>
        PurchaseItem,

        /// <summary> Тип анимации для воспроизведения. </summary>
        AnimationType,

        /// <summary> Время продолжительности анимации. </summary>
        AnimationTime,

        /// <summary> Выбранный объект для взаимодействия. </summary>
        SelectedObject,

        /// <summary> Точка назначения, к которой должен направляться персонаж. </summary>
        DestinationPoint
    }
}