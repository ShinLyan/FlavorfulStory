using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Settings
{
    /// <summary> Управляет настройками разрешения экрана и режимами отображения окна. </summary>
    public class ScreenSettings : MonoBehaviour
    {
        /// <summary> Выпадающий список выбора разрешения экрана. </summary>
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        /// <summary> Выпадающий список выбора режима окна. </summary>
        [SerializeField] private TMP_Dropdown _screenModeDropdown;

        /// <summary> Список доступных разрешений экрана. </summary>
        private readonly List<Resolution> _resolutions = new();

        /// <summary> Список доступных режимов окна. </summary>
        private readonly List<string> _screenModeOptions = new()
        {
            "Windowed", // Оконный режим
            "Full screen" // Полноэкранный режим
        };

        /// <summary> Подписывается на события изменения настроек экрана при активации объекта. </summary>
        private void OnEnable()
        {
            _resolutionDropdown.onValueChanged.AddListener(delegate { ResolutionOptionChanged(); });
            _screenModeDropdown.onValueChanged.AddListener(delegate { ScreenModeChanged(); });
        }

        /// <summary> Устанавливает выбранное пользователем разрешение экрана. </summary>
        private void ResolutionOptionChanged()
        {
            var resolution = _resolutions[_resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }

        /// <summary> Устанавливает выбранный пользователем режим окна. </summary>
        private void ScreenModeChanged()
        {
            switch (_screenModeDropdown.value)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }
        }

        /// <summary> Отписывается от событий при отключении объекта. </summary>
        private void OnDisable()
        {
            _resolutionDropdown.onValueChanged.RemoveAllListeners();
            _screenModeDropdown.onValueChanged.RemoveAllListeners();
        }

        /// <summary> Инициализирует настройки экрана при запуске. </summary>
        private void Start()
        {
            InitializeResolutions();
            InitializeScreenModes();
            UpdateDropdownValues();
        }

        /// <summary> Получает все доступные разрешения экрана и заполняет выпадающий список. </summary>
        private void InitializeResolutions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            double maxRefreshRate = allResolutions[^1].refreshRateRatio.value;
            var uniqueResolutions = new List<string>();

            foreach (var resolution in allResolutions)
            {
                if (resolution.refreshRateRatio.value == maxRefreshRate)
                {
                    uniqueResolutions.Add($"{resolution.width}x{resolution.height}");
                    _resolutions.Add(resolution);
                }
            }

            _resolutionDropdown.ClearOptions();
            _resolutionDropdown.AddOptions(uniqueResolutions);
            _resolutionDropdown.value = uniqueResolutions.Count - 1;
            _resolutionDropdown.RefreshShownValue();
        }

        /// <summary> Заполняет выпадающий список режимов экрана. </summary>
        private void InitializeScreenModes()
        {
            _screenModeDropdown.ClearOptions();
            _screenModeDropdown.AddOptions(_screenModeOptions);

            _screenModeDropdown.value = 1;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            _screenModeDropdown.RefreshShownValue();
        }

        /// <summary> Обновляет отображаемые значения в выпадающих списках. </summary>
        private void UpdateDropdownValues()
        {
            _resolutionDropdown.RefreshShownValue();
            _screenModeDropdown.RefreshShownValue();
        }
    }
}