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

        private const float DayStartTime = 6f;
        private const float NightStartTime = 18f;
        private const float MoonStartTime = NightStartTime - 1;

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
            float currentTimeInHours = gameTime.Hour;

            bool isSunActive = currentTimeInHours >= DayStartTime && currentTimeInHours < NightStartTime;
            _sunLight.enabled = isSunActive;

            if (!isSunActive) return;

            _sunLight.shadows = _currentWeatherLightSettings.SunShadowType;
            _sunLight.shadowBias = 0.01f;


            if (currentTimeInHours >= 17.5f) // убираем тени если время 17:30 - 18:00
            {
                float shadowProgress = Mathf.InverseLerp(17.5f, NightStartTime, currentTimeInHours);
                _sunLight.shadowStrength =
                    Mathf.Lerp(_currentWeatherLightSettings.SunShadowStrength, 0f, shadowProgress);
            }
            else
            {
                _sunLight.shadowStrength = _currentWeatherLightSettings.SunShadowStrength;
            }

            float sunProgress = Mathf.InverseLerp(DayStartTime, NightStartTime, currentTimeInHours);
            float sunAngleX;
            float sunAngleY;

            if (sunProgress < 0.5f) // поворачиваем до 12:00
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.x,
                    _currentWeatherLightSettings.SunAngleX.y, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYBefore12.x,
                    _currentWeatherLightSettings.SunAngleYBefore12.y, sunProgress);
            }
            else // поворачиваем после 12:00
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.y,
                    _currentWeatherLightSettings.SunAngleX.z, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYAfter12.x,
                    _currentWeatherLightSettings.SunAngleYAfter12.y, sunProgress);
            }

            _sunLight.transform.rotation = Quaternion.Euler(sunAngleX, sunAngleY, 0f);

            _sunLight.color = _currentWeatherLightSettings.SunColorGradient.Evaluate(sunProgress);
            _sunLight.intensity =
                _currentWeatherLightSettings.SunIntensityCurve.Evaluate(sunProgress) *
                _currentWeatherLightSettings.MaxSunIntensity;
        }

        private void UpdateMoon(DateTime gameTime)
        {
            float currentTimeInHours = gameTime.Hour;

            bool isMoonActive = currentTimeInHours >= MoonStartTime || currentTimeInHours < DayStartTime;
            _moonLight.enabled = isMoonActive;

            if (!isMoonActive) return;

            _moonLight.shadows = _currentWeatherLightSettings.MoonShadowType;
            _moonLight.shadowNormalBias = 0.4f;

            if (DayStartTime < currentTimeInHours && currentTimeInHours < NightStartTime)
            {
                // 17:00 - 18:00 теней от луны нету
                _moonLight.shadowStrength = 0f;
            }
            else if (currentTimeInHours >= NightStartTime && currentTimeInHours < 19f)
            {
                // плавно включаем тени если время 18:00 - 19:00
                float shadowProgress = Mathf.InverseLerp(NightStartTime, 19f, currentTimeInHours);
                _moonLight.shadowStrength =
                    Mathf.Lerp(0f, _currentWeatherLightSettings.MoonShadowStrength, shadowProgress);
            }
            else
            {
                _moonLight.shadowStrength = _currentWeatherLightSettings.MoonShadowStrength;
            }

            float moonProgress = Mathf.InverseLerp(MoonStartTime, 26f,
                currentTimeInHours >= MoonStartTime ? currentTimeInHours : currentTimeInHours + 24f);
            float moonAngleY = Mathf.Lerp(-45f, 45f, moonProgress);


            _moonLight.transform.rotation = Quaternion.Euler(30f, moonAngleY, 0f);


            _moonLight.color = _currentWeatherLightSettings.MoonColorGradient.Evaluate(moonProgress);
            _moonLight.intensity =
                _currentWeatherLightSettings.MoonIntensityCurve.Evaluate(moonProgress) *
                _currentWeatherLightSettings.MaxMoonIntensity;
        }
    }
}