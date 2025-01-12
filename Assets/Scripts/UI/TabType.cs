namespace FlavorfulStory.UI
{
    /// <summary> Перичесление для обозначения типов кнопок </summary>
    /// <remarks> Используется для возможного переназначения кнопок в настройках управления. </remarks>>
    public enum TabType
    {
        /// <summary> Открытие главной вкладки в меню. </summary>
        MainTab, 

        /// <summary> Открытие вкладки инвентаря в меню. </summary>
        InventoryTab,
        
        /// <summary> Открытие вкладки настроек в меню. </summary>
        SettingsTab,
    }
}