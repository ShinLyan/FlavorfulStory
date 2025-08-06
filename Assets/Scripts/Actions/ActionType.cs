namespace FlavorfulStory.Actions
{
    /// <summary> Тип действия, которое может выполнить игрок. </summary>
    public enum ActionType
    {
        /// <summary> Сбор предметов или ресурсов. </summary>
        Gather,

        /// <summary> Разговор с персонажем. </summary>
        Talk,

        /// <summary> Сон или переход ко дню. </summary>
        Sleep,

        /// <summary> Выброс предмета или ресурса. </summary>
        Drop,

        /// <summary> Строительство или установка объекта. </summary>
        Build,

        /// <summary> Открыть сундук / прилавок. </summary>
        Open
    }
}