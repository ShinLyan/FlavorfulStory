using FlavorfulStory.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace FlavorfulStory.Settings
{
    /// <summary> Значения слайдеров громкости.</summary>
    public class AudioSettings : MonoBehaviour
    {
        /// <summary> Аудиомикшер.</summary>
        [SerializeField] private AudioMixer _audioMixer;

        /// <summary> При старте выставляем значения слайдера.</summary>
        private void Start()
        {
            SetDefaultValues();
        }

        /// <summary> Установить значения по умолчанию.</summary>
        private void SetDefaultValues()
        {
            SetMixerValue(VolumeType.Master, GetVolumeValueFromType(VolumeType.Master));
            SetMixerValue(VolumeType.Music, GetVolumeValueFromType(VolumeType.Music));
            SetMixerValue(VolumeType.SFX, GetVolumeValueFromType(VolumeType.SFX));
        }

        /// <summary> Установка значения параметра громкости определенного аудио канала.</summary>
        /// <param name="name"> Название аудио канала.</param>
        /// <param name="value"> Новое значение, считанное со слайдера.</param>
        public void SetMixerValue(VolumeType volumeType, float value)
        {
            const int MinMixerValue = -80, MixerMultipliyer = 20;
            float mixerValue = value == 0 ? MinMixerValue : Mathf.Log10(value) * MixerMultipliyer;

            string channelName = volumeType.ToString();
            _audioMixer.SetFloat(channelName, mixerValue);

            SavingSettings.SetVolumeFromType(volumeType, value);
        }

        /// <summary> Получить значение громкости по типу.</summary>
        /// <param name="volumeType"> Тип громкости.</param>
        /// <returns> Возвращает значение громкости в зависимости от типа.</returns>
        public float GetVolumeValueFromType(VolumeType volumeType) => volumeType switch
        {
            VolumeType.Master => SavingSettings.MasterVolume,
            VolumeType.SFX => SavingSettings.SFXVolume,
            VolumeType.Music => SavingSettings.MusicVolume,
            _ => 0,
        };
    }
}