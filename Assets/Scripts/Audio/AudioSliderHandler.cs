using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Audio
{
    /// <summary> Класс, считывающий значение слайдера. </summary>
    [RequireComponent(typeof(Slider))]
    public class AudioSliderHandler : MonoBehaviour
    {
        /// <summary> Тип громкости. </summary>
        [SerializeField] private VolumeType _volumeType;

        /// <summary> Настройки звука. </summary>
        private Settings.AudioSettings _audioSettings;

        /// <summary> Ссылка на слайдер. </summary>
        private Slider _slider;

        /// <summary> Получение компонента слайдера. </summary>
        private void Awake()
        {
            _audioSettings = FindObjectOfType<Settings.AudioSettings>();
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(delegate { ValueChanged(); });
        }

        /// <summary> При старте инициализируем значения слайдеров. </summary>
        private void Start()
        {
            _slider.value = _audioSettings.GetVolumeValueFromType(_volumeType);
        }

        /// <summary> Вызывается при изменении значения слайдера. </summary>
        private void ValueChanged()
        {
            _audioSettings.SetMixerValue(_volumeType, _slider.value);
        }
    }
}