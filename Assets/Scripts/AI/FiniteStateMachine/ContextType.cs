namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Типы контекстных данных, используемых для передачи между состояниями конечного автомата. </summary>
    public enum ContextType
    {
        /// <summary> Точка кассы для совершения платежа. </summary>
        CashDeskPoint,

        /// <summary> Покупаемый предмет. </summary>
        PurchaseItem,

        /// <summary> Тип анимации для воспроизведения. </summary>
        AnimationType,

        /// <summary> Время продолжительности анимации. </summary>
        AnimationTime,

        /// <summary> Выбранный объект для взаимодействия. </summary>
        SelectedObject
    }
}