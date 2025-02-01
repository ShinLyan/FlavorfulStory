using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Audio
{
    /// <summary> Считывание значения слайдера для управления громкостью. </summary>
    [RequireComponent(typeof(Slider))]
    public class AudioSliderHandler : MonoBehaviour
    {
        /// <summary> Тип громкости, связанный с этим слайдером. </summary>
        [SerializeField] private VolumeType _volumeType;

        /// <summary> Ссылка на настройки звука. </summary>
        private Settings.AudioSettings _audioSettings;

        /// <summary> Компонент слайдера. </summary>
        private Slider _slider;

        /// <summary> Инициализация ссылки на слайдер и подписка на событие изменения значения. </summary>
        private void Awake()
        {
            _audioSettings = FindAnyObjectByType<Settings.AudioSettings>();
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(delegate { ValueChanged(); });
        }

        /// <summary> Установка начального значения слайдера на основе текущих настроек звука. </summary>
        private void Start()
        {
            _slider.value = _audioSettings.GetVolumeValueFromType(_volumeType);
        }

        /// <summary> Изменение значения в настройках звука при взаимодействии со слайдером. </summary>
        private void ValueChanged()
        {
            _audioSettings.SetMixerValue(_volumeType, _slider.value);
        }
    }
}