namespace FlavorfulStory.Audio
{
    /// <summary> Тип громкости для управления уровнями звука. </summary>
    public enum MixerChannelType
    {
        /// <summary> Общая громкость. </summary>
        Master,

        /// <summary> Громкость звуковых эффектов. </summary>
        SFX,

        /// <summary> Громкость музыки. </summary>
        Music
    }
}