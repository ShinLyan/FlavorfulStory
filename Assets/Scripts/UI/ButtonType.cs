namespace FlavorfulStory.UI
{
    /// <summary> Перичесление для обозначения типов кнопок </summary>
    /// <remarks> Используется для возможного переназначения кнопок в настройках управления. </remarks>>
    public enum ButtonType
    {
        /// <summary> Открытие главной вкладки в меню. </summary>
        OpenMainTab, 

        /// <summary> Открытие вкладки инвентаря в меню. </summary>
        OpenInventoryTab,

        /// <summary> Открытие вкладки карты в меню. </summary>
        OpenMapTab,
        
        /// <summary> Открытие вкладки настроек в меню. </summary>
        OpenSettingsTab,
    }
}