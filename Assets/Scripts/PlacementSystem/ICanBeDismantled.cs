namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Интерфейс для объектов, которые можно разрушить (демонтировать). </summary>
    public interface ICanBeDismantled
    {
        /// <summary> Можно ли в данный момент разрушить объект? </summary>
        bool CanBeDismantled { get; }

        /// <summary> Причина, по которой объект нельзя разрушить. </summary>
        /// <remarks> Используется для отображения уведомлений игроку. </remarks>
        string DismantleDeniedReason { get; }
    }
}