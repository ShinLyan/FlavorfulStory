using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.Settings
{
    /// <summary> Управляет сохранением и загрузкой настроек громкости. </summary>
    public static class SavingSettings
    {
        /// <summary> Громкость основного канала (Master). </summary>
        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat("MasterVolume", 1f);
            private set => PlayerPrefs.SetFloat("MasterVolume", value);
        }

        /// <summary> Громкость звуковых эффектов (SFX). </summary>
        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat("SfxVolume", 1f);
            private set => PlayerPrefs.SetFloat("SfxVolume", value);
        }

        /// <summary> Громкость музыки (Music). </summary>
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 1f);
            private set => PlayerPrefs.SetFloat("MusicVolume", value);
        }

        /// <summary> Устанавливает громкость для указанного типа аудиоканала. </summary>
        /// <param name="mixerChannelType"> Тип громкости (Master, Music, SFX). </param>
        /// <param name="value"> Новое значение громкости (0.0 - 1.0). </param>
        public static void SetVolumeFromType(MixerChannelType mixerChannelType, float value)
        {
            switch (mixerChannelType)
            {
                case MixerChannelType.Master:
                    MasterVolume = value;
                    break;
                case MixerChannelType.SFX:
                    SFXVolume = value;
                    break;
                case MixerChannelType.Music:
                    MusicVolume = value;
                    break;
            }
        }
    }
}