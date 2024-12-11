using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.Settings
{
    /// <summary> Сохранение настроек.</summary>
    public class SavingSettings
    {
        /// <summary> Общая громкость.</summary>
        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat("MasterVolume");
            private set => PlayerPrefs.SetFloat("MasterVolume", value);
        }

        /// <summary> Громкость звуков.</summary>
        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat("SfxVolume");
            private set => PlayerPrefs.SetFloat("SfxVolume", value);
        }

        /// <summary> Громкость музыки.</summary>
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume");
            private set => PlayerPrefs.SetFloat("MusicVolume", value);
        }

        public static void SetVolumeFromType(VolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case VolumeType.Master:
                    MasterVolume = value;
                    break;
                case VolumeType.SFX:
                    SFXVolume = value;
                    break;
                case VolumeType.Music:
                    MusicVolume = value;
                    break;
            }
        }
    }
}