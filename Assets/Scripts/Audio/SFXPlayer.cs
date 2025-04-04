using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        /// <summary> Экземпляр синглтона. </summary>
        public static SfxPlayer Instance { get; private set; }

        /// <summary> Максимальность дальность, на которой слышен звук. </summary>
        [Range(10, 50)]
        [SerializeField] private float _maxDistance;

        /// <summary> Источник аудио. </summary>
        private AudioSource _audioSource;

        /// <summary> Данные о звуках. </summary>
        private readonly Dictionary<SfxType, List<AudioClip>> _sfxData = new();

        /// <summary> Создать Источник аудио. Загрузить звуки. </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _audioSource = GetComponent<AudioSource>();
            LoadSfxData();
        }

        /// <summary> Загрузить аудиоклипы. </summary>
        private void LoadSfxData()
        {
            foreach (var resource in Resources.LoadAll<SfxData>(string.Empty).ToList())
                _sfxData.Add(resource.Type, new List<AudioClip>(resource.Clips));
        }

        /// <summary> Проиграть SFX. </summary>
        /// <param name="type"> Тип проигрываемого звука. </param>
        /// <param name="otherTransform"> Transform источника звука. Используется для рассчета громкости в зависисмости от расстояния. </param>
        public void PlayOneShot(SfxType type)
        {
            if (_sfxData.TryGetValue(type, out var clips))
            {
                var clip = clips[Random.Range(0, clips.Count)];
                _audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError($"Не найден трек для типа: {type}");
            }
        }

        public void PlayOneShot(AudioClip clip) => _audioSource.PlayOneShot(clip);
    }
}