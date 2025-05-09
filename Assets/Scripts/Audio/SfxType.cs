﻿namespace FlavorfulStory.Audio
{
    /// <summary> Типы звуковых эффектов (SFX) в игре. </summary>
    public enum SfxType
    {
        /// <summary> Звук удара по дереву. </summary>
        WoodHit,

        /// <summary> Звук удара по камню. </summary>
        StoneHit,

        /// <summary> Звук клика по элементу интерфейса. </summary>
        UIClick,

        /// <summary> Звук наведения курсора на элемент интерфейса. </summary>
        UIHover,

        /// <summary> Звук шагов при ходьбе. </summary>
        Walk,

        /// <summary> Звук шагов при беге. </summary>
        Run,

        /// <summary> Звук еды. </summary>
        Eat,

        /// <summary> Звук строительства. </summary>
        Build
    }
}