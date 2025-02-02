using FlavorfulStory.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace FlavorfulStory.Settings
{
    /// <summary> Управляет настройками громкости аудиомикшера. </summary>
    public class AudioSettings : MonoBehaviour
    {
        /// <summary> Аудиомикшер, используемый для управления громкостью. </summary>
        [SerializeField] private AudioMixer _audioMixer;

        /// <summary> Устанавливает значения громкости при запуске. </summary>
        private void Start()
        {
            SetDefaultValues();
        }

        /// <summary> Устанавливает значения громкости из сохранённых настроек. </summary>
        private void SetDefaultValues()
        {
            SetMixerValue(VolumeType.Master, GetVolumeValueFromType(VolumeType.Master));
            SetMixerValue(VolumeType.Music, GetVolumeValueFromType(VolumeType.Music));
            SetMixerValue(VolumeType.SFX, GetVolumeValueFromType(VolumeType.SFX));
        }

        /// <summary> Устанавливает громкость указанного типа. </summary>
        /// <param name="volumeType"> Тип громкости (Master, Music, SFX). </param>
        /// <param name="value"> Новое значение громкости (0.0 - 1.0). </param>
        public void SetMixerValue(VolumeType volumeType, float value)
        {
            const int MinMixerValue = -80, MixerMultiplier = 20;
            float mixerValue = value == 0 ? MinMixerValue : Mathf.Log10(value) * MixerMultiplier;

            string channelName = volumeType.ToString();
            _audioMixer.SetFloat(channelName, mixerValue);

            SavingSettings.SetVolumeFromType(volumeType, value);
        }

        /// <summary> Получает сохранённое значение громкости по её типу. </summary>
        /// <param name="volumeType"> Тип громкости (Master, Music, SFX). </param>
        /// <returns> Возвращает сохранённое значение громкости. </returns>
        public float GetVolumeValueFromType(VolumeType volumeType) => volumeType switch
        {
            VolumeType.Master => SavingSettings.MasterVolume,
            VolumeType.SFX => SavingSettings.SFXVolume,
            VolumeType.Music => SavingSettings.MusicVolume,
            _ => 0,
        };
    }
}