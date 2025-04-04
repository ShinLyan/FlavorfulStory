using FlavorfulStory.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace FlavorfulStory.Audio
{
    /// <summary> Управляет настройками громкости аудиомикшера. </summary>
    public class AudioSettings : MonoBehaviour
    {
        /// <summary> Аудиомикшер, используемый для управления громкостью. </summary>
        [SerializeField] private AudioMixer _audioMixer;

        /// <summary> Устанавливает значения громкости при запуске. </summary>
        private void Start() => SetDefaultValues();

        /// <summary> Устанавливает значения громкости из сохранённых настроек. </summary>
        private void SetDefaultValues()
        {
            SetMixerValue(MixerChannelType.Master, GetVolumeValueFromType(MixerChannelType.Master));
            SetMixerValue(MixerChannelType.Music, GetVolumeValueFromType(MixerChannelType.Music));
            SetMixerValue(MixerChannelType.SFX, GetVolumeValueFromType(MixerChannelType.SFX));
        }

        /// <summary> Устанавливает громкость указанного типа. </summary>
        /// <param name="mixerChannelType"> Тип громкости (Master, Music, SFX). </param>
        /// <param name="value"> Новое значение громкости (0.0 - 1.0). </param>
        public void SetMixerValue(MixerChannelType mixerChannelType, float value)
        {
            const int MinMixerValue = -80, MixerMultiplier = 20;
            float mixerValue = value == 0 ? MinMixerValue : Mathf.Log10(value) * MixerMultiplier;

            string channelName = mixerChannelType.ToString();
            _audioMixer.SetFloat(channelName, mixerValue);

            SavingSettings.SetVolumeFromType(mixerChannelType, value);
        }

        /// <summary> Получает сохранённое значение громкости по её типу. </summary>
        /// <param name="mixerChannelType"> Тип громкости (Master, Music, SFX). </param>
        /// <returns> Возвращает сохранённое значение громкости. </returns>
        public float GetVolumeValueFromType(MixerChannelType mixerChannelType) => mixerChannelType switch
        {
            MixerChannelType.Master => SavingSettings.MasterVolume,
            MixerChannelType.SFX => SavingSettings.SFXVolume,
            MixerChannelType.Music => SavingSettings.MusicVolume,
            _ => 0
        };
    }
}