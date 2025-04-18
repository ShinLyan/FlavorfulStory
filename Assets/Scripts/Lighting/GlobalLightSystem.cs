using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.Lightning
{
    public class DynamicLighting : MonoBehaviour
    {
        [SerializeField] private Light _sunLight;
        [SerializeField] private Light _moonLight;

        [SerializeField] private WeatherLightSettings[] _weatherLightSettings;

        private LightSettings _currentWeatherLightSettings;


        private void OnEnable() => WorldTime.OnTimeUpdated += UpdateLighting;
        private void OnDisable() => WorldTime.OnTimeUpdated -= UpdateLighting;

        private void Start() { _currentWeatherLightSettings = _weatherLightSettings[0].LightSettings; }

        private void UpdateLighting(DateTime gameTime)
        {
            UpdateSun(gameTime);
            UpdateMoon(gameTime);
        }

        private void UpdateSun(DateTime gameTime)
        {
            float hour = gameTime.Hour + gameTime.Minute / 60f;

            bool isSunActive = hour >= 6 && hour < 18;
            _sunLight.enabled = isSunActive;

            if (!isSunActive) return;

            _sunLight.shadows = _currentWeatherLightSettings.SunShadowType;
            _sunLight.shadowStrength = _currentWeatherLightSettings.SunShadowStrength;
            _sunLight.shadowBias = 0.01f;

            float sunProgress = Mathf.InverseLerp(6, 18, hour);
            float sunAngleX;
            float sunAngleY;

            if (sunProgress < 0.5f)
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.x,
                    _currentWeatherLightSettings.SunAngleX.y, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYBefore12.x,
                    _currentWeatherLightSettings.SunAngleYBefore12.y, sunProgress);
            }
            else
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.y,
                    _currentWeatherLightSettings.SunAngleX.z, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYAfter12.x,
                    _currentWeatherLightSettings.SunAngleYAfter12.y, sunProgress);
            }

            _sunLight.transform.rotation = Quaternion.Euler(sunAngleX, sunAngleY, 0);

            _sunLight.color = _currentWeatherLightSettings.SunColorGradient.Evaluate(sunProgress);
            _sunLight.intensity =
                _currentWeatherLightSettings.SunIntensityCurve.Evaluate(sunProgress) *
                _currentWeatherLightSettings.MaxSunIntensity;
        }

        private void UpdateMoon(DateTime gameTime)
        {
            float hour = gameTime.Hour + gameTime.Minute / 60f;

            bool isMoonActive = hour >= 18 || hour < 6f;
            _moonLight.enabled = isMoonActive;

            if (!isMoonActive) return;

            _moonLight.shadows = _currentWeatherLightSettings.MoonShadowType;
            _moonLight.shadowStrength = _currentWeatherLightSettings.MoonShadowStrength;
            _moonLight.shadowBias = _currentWeatherLightSettings.MoonShadowBias;
            _moonLight.shadowNormalBias = 0.4f;

            float moonProgress = Mathf.InverseLerp(18, 26,
                hour >= 18 ? hour : hour + 24);
            float moonAngleY = Mathf.Lerp(-45, 45, moonProgress);


            _moonLight.transform.rotation = Quaternion.Euler(60, moonAngleY, 0);


            _moonLight.color = _currentWeatherLightSettings.MoonColorGradient.Evaluate(moonProgress);
            _moonLight.intensity =
                _currentWeatherLightSettings.MoonIntensityCurve.Evaluate(moonProgress) *
                _currentWeatherLightSettings.MaxMoonIntensity;
        }
    }
}