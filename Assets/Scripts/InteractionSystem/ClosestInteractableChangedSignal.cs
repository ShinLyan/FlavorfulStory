namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Сигнал, отправляемый при изменении ближайшего интерактивного объекта. </summary>
    public readonly struct ClosestInteractableChangedSignal
    {
        /// <summary> Новый ближайший объект, доступный для взаимодействия. </summary>
        public IInteractable ClosestInteractable { get; }

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="closestInteractable"> Ближайший интерактивный объект. </param>
        public ClosestInteractableChangedSignal(IInteractable closestInteractable) =>
            ClosestInteractable = closestInteractable;
    }
}