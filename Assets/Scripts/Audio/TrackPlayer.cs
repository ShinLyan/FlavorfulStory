using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Проигрыватель треков, обеспечивающий воспроизведение музыки из списка. </summary>
    [RequireComponent(typeof(AudioSource))]
    public class TrackPlayer : MonoBehaviour
    {
        /// <summary> Список треков для воспроизведения. </summary>
        [SerializeField] private List<AudioClip> _tracks;

        /// <summary> Источник звука для воспроизведения треков. </summary>
        private AudioSource _source;

        /// <summary> Инициализация аудиоисточника. </summary>
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        /// <summary> Запуск воспроизведения треков. </summary>
        private void Start()
        {
            StartCoroutine(PlayTracks(_tracks));
        }

        /// <summary> Воспроизведение указанного трека. </summary>
        /// <param name="audioClip"> Трек, который нужно воспроизвести. </param>
        private void PlayTrack(AudioClip audioClip)
        {
            _source.clip = audioClip;
            _source.Play();
        }

        /// <summary> Воспроизведение треков из списка в случайном порядке. </summary>
        /// <param name="tracks"> Список треков для воспроизведения. </param>
        /// <returns> Объект корутины. </returns>
        private IEnumerator PlayTracks(List<AudioClip> tracks)
        {
            while (true)
            {
                int randomTrackIndex = Random.Range(0, tracks.Count);
                PlayTrack(tracks[randomTrackIndex]);
                yield return new WaitForSeconds(tracks[randomTrackIndex].length);
            }
        }
    }
}