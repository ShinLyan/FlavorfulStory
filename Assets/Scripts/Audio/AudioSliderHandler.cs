using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Audio
{
    /// <summary> Считывание значения слайдера для управления громкостью. </summary>
    [RequireComponent(typeof(Slider))]
    public class AudioSliderHandler : MonoBehaviour
    {
        /// <summary> Тип микшера, связанный с этим слайдером. </summary>
        [SerializeField] private MixerChannelType _mixerChannelType;

        /// <summary> Ссылка на настройки звука. </summary>
        private AudioSettings _audioSettings;

        /// <summary> Компонент слайдера. </summary>
        private Slider _slider;

        /// <summary> Инициализация ссылки на слайдер и подписка на событие изменения значения. </summary>
        private void Awake()
        {
            // TODO: ZENJECT
            _audioSettings = FindAnyObjectByType<AudioSettings>();
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(HandleSliderChanged);
        }

        /// <summary> Установка начального значения слайдера на основе текущих настроек звука. </summary>
        private void Start() => _slider.value = AudioSettings.GetVolumeValueFromType(_mixerChannelType);

        /// <summary> Обработчик звукового слайдера. </summary>
        /// <param name="sliderValue"> Значение слайдера. </param>
        private void HandleSliderChanged(float sliderValue)
        {
            _audioSettings.SetMixerValue(_mixerChannelType, sliderValue);
        }
    }
}