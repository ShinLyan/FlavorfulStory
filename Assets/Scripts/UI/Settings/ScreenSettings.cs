using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace FlavorfulStory.UI.Settings
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

        /// <summary> Инициализирует настройки экрана при запуске. </summary>
        private void Awake()
        {
            InitializeResolutions();
            InitializeScreenModes();
            UpdateDropdownValues();
        }
        
        /// <summary> Подписывается на события изменения настроек экрана при активации объекта. </summary>
        private void OnEnable()
        {
            _resolutionDropdown.onValueChanged.AddListener(ResolutionOptionChanged);
            _screenModeDropdown.onValueChanged.AddListener(ScreenModeChanged);
        }
        
        /// <summary> Отписывается от событий при отключении объекта. </summary>
        private void OnDisable()
        {
            _resolutionDropdown.onValueChanged.RemoveAllListeners();
            _screenModeDropdown.onValueChanged.RemoveAllListeners();
        }
        
        /// <summary> Устанавливает выбранное пользователем разрешение экрана. </summary>
        private void ResolutionOptionChanged(int index)
        {
            var resolution = _resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }

        /// <summary> Устанавливает выбранный пользователем режим окна. </summary>
        private static void ScreenModeChanged(int index)
        {
            Screen.fullScreenMode = index switch
            {
                0 => FullScreenMode.Windowed,
                1 => FullScreenMode.FullScreenWindow,
                _ => Screen.fullScreenMode
            };
        }

        /// <summary> Получает все доступные разрешения экрана и заполняет выпадающий список. </summary>
        private void InitializeResolutions()
        {
            var allResolutions = Screen.resolutions;
            double maxRefreshRate = allResolutions.Last().refreshRateRatio.value;
            var uniqueResolutions = new List<string>();

            const double epsilon = 0.1f;
            foreach (var resolution in allResolutions)
                if (Math.Abs(resolution.refreshRateRatio.value - maxRefreshRate) < epsilon)
                {
                    uniqueResolutions.Add($"{resolution.width}x{resolution.height}");
                    _resolutions.Add(resolution);
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