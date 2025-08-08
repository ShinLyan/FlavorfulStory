namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Сигнал, отправляемый при изменении ближайшего интерактивного объекта. </summary>
    public struct ClosestInteractableChangedSignal
    {
        /// <summary> Новый ближайший объект, доступный для взаимодействия. </summary>
        public IInteractable ClosestInteractable;
    }
}