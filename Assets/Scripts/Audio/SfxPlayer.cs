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

        /// <summary> Источник аудио. </summary>
        private AudioSource _audioSource;

        /// <summary> Данные о звуках. </summary>
        private readonly Dictionary<SfxType, List<AudioClip>> _sfxData = new();

        /// <summary> Создать Источник аудио. Загрузить звуки. </summary>
        private void Awake()
        {
            if (!Instance)
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
            foreach (var resource in Resources.LoadAll<SfxData>(string.Empty))
                _sfxData.Add(resource.Type, new List<AudioClip>(resource.Clips));
        }

        /// <summary> Проиграть SFX. </summary>
        /// <param name="type"> Тип проигрываемого звука. </param>
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

        /// <summary> Проиграть случайный звук. </summary>
        /// <param name="clips"> Аудиоклипы. </param>
        public void PlayOneShot(IEnumerable<AudioClip> clips)
        {
            var audioClips = clips as AudioClip[] ?? clips.ToArray();
            _audioSource.PlayOneShot(audioClips.ElementAt(Random.Range(0, audioClips.Count())));
        }
    }
}