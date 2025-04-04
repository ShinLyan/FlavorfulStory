using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace FlavorfulStory.Audio
{
    /// <summary> Проигрыватель треков, обеспечивающий воспроизведение музыки из списка. </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AmbientSound : MonoBehaviour
    {
        /// <summary> Список треков для воспроизведения. </summary>
        [SerializeField] private List<AudioClip> _trackList;

        /// <summary> Источник звука для воспроизведения треков. </summary>
        private AudioSource _source;

        /// <summary> Экземпляр рандома. </summary>
        /// <remarks> Необходим для перемешивания треклиста. </remarks>
        private Random _random;

        /// <summary> Инициализация аудиоисточника. </summary>
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _random = new Random();
        }

        /// <summary> Запуск воспроизведения треков. </summary>
        private void Start() => StartCoroutine(PlayTracks());

        private IEnumerable<AudioClip> GetShuffledTracklist()
        {
            return _trackList.OrderBy(_ => _random.Next());
        }

        /// <summary> Воспроизведение указанного трека. </summary>
        /// <param name="audioClip"> Трек, который нужно воспроизвести. </param>
        private void PlayTrack(AudioClip audioClip)
        {
            _source.clip = audioClip;
            _source.Play();
        }

        /// <summary> Воспроизведение треков из списка в случайном порядке. </summary>
        /// <returns> Инструкции кода между блоками Yield. </returns>
        private IEnumerator PlayTracks()
        {
            while (true)
            {
                var tracks = GetShuffledTracklist();
                foreach (var track in tracks)
                {
                    PlayTrack(track);
                    yield return new WaitForSeconds(track.length);
                }
            }
        }
    }
}