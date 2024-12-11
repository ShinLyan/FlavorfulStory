using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Settings
{
    /// <summary> Настройки разрешения и режима окна.</summary>
    public class ScreenSettings : MonoBehaviour
    {
        /// <summary> Выпадающий список с типами окна.</summary>
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        /// <summary> Выпадающий список с разрешениями.</summary>
        [SerializeField] private TMP_Dropdown _screenModeDropdown;

        /// <summary> Список разрешений.</summary>
        private readonly List<Resolution> _resolutions = new();

        /// <summary> Список режимов окна.</summary>
        private readonly List<string> _screenModeOptions = new() {
            "Окно",
            "Полный экран",
        };

        /// <summary> Вызывается при включении объекта.</summary>
        private void OnEnable()
        {
            _resolutionDropdown.onValueChanged.AddListener(delegate
            {
                ResolutionOptionChanged();
            });

            _screenModeDropdown.onValueChanged.AddListener(delegate
            {
                ScreenModeChanged();
            });
        }

        /// <summary> Срабатывает при выборе пользователя в выпадающем списке разрещений.</summary>
        private void ResolutionOptionChanged()
        {
            Resolution resolution = _resolutions[_resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }

        /// <summary> Срабатывает при выборе пользователя в выпадающем списке режимов окна.</summary>
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

        /// <summary> Вызывается при отключении объекта.</summary>
        private void OnDisable()
        {
            _resolutionDropdown.onValueChanged.RemoveAllListeners();
            _screenModeDropdown.onValueChanged.RemoveAllListeners();
        }

        /// <summary> Инициализация разрешений и рижемов окна и обновление значений в выпадаюзих списках.</summary>
        private void Start()
        {
            InitializeResolutions();
            InitializeScreenModes();
            UpdateDropdownValues();
        }

        /// <summary> Получения всех поддерживаемых разрешений экрана и добавления их в соответствующий выпадающий список.</summary>
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

        /// <summary> Получение всех поддерживаемых режимов экрана и добавления их в соответствующий выпадающий список.</summary>
        private void InitializeScreenModes()
        {
            _screenModeDropdown.ClearOptions();

            List<string> _screenModeDropdownOptions = _screenModeOptions;
            _screenModeDropdown.AddOptions(_screenModeDropdownOptions);

            _screenModeDropdown.value = 1;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            _screenModeDropdown.RefreshShownValue();
        }

        /// <summary> Обновление значений выпадающих списков.</summary>
        private void UpdateDropdownValues()
        {
            _resolutionDropdown.RefreshShownValue();
            _screenModeDropdown.RefreshShownValue();
        }
    }
}