using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FlavorfulStory.TimeManagement
{
    public class GlobalLightSystem : MonoBehaviour
    {
        [Tooltip("Настройки для освещения.")] [SerializeField]
        private TimeSettings _settings;

        private TimeService _timeService;

        [SerializeField] private Light _sun;
        [SerializeField] private Light _moon;
        [SerializeField] private AnimationCurve _lightIntensityCurve;
        [SerializeField] private float _maxSunIntensity = 1;
        [SerializeField] private float _maxMoonIntensity = 0.5f;

        [SerializeField] private Color _dayAmbientLight;

        [SerializeField] private Color _nightAmbientLight;

        // [SerializeField] private Material _skyboxMaterial;
        private float _initialDialRotation;

        private ColorAdjustments _colorAdjustments;

        private void OnEnable()
        {
            WorldTime.OnTick += UpdateLight;
        }

        /// <summary> Отписка от события изменения времени при деактивации объекта. </summary>
        private void OnDisable()
        {
            WorldTime.OnTick -= UpdateLight;
        }

        private void Start()
        {
            _timeService = new TimeService(_settings);
        }

        private void UpdateLight()
        {
            RotateSun();
            UpdateLightSettings();
        }

        private void UpdateLightSettings()
        {
            float dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.down);
            float lightIntensity = _lightIntensityCurve.Evaluate(dotProduct);

            _sun.intensity = Mathf.Lerp(0, _maxSunIntensity, lightIntensity);
            _moon.intensity = Mathf.Lerp(_maxMoonIntensity, 0, lightIntensity);
        }

        private void RotateSun()
        {
            float rotation = _timeService.CalculateSunAngle();
            float secondRotation;
            if (rotation <= 90)
                secondRotation = Mathf.Lerp(-30f, 0f, rotation / 90);
            else
                secondRotation = Mathf.Lerp(0f, 30f, (rotation - 90f) / 90f);


            _sun.transform.rotation = Quaternion.Euler(rotation, secondRotation, 0f);
        }
    }
}