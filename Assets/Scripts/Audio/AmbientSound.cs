using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        /// <summary> Инициализация аудиоисточника. </summary>
        private void Awake() => _source = GetComponent<AudioSource>();

        /// <summary> Запуск воспроизведения треков. </summary>
        private void Start()
        {
            if (_trackList == null || _trackList.Count == 0)
            {
                Debug.LogError("Не добавлены треки");
                return;
            }

            StartCoroutine(PlayTracks());
        }

        /// <summary> Воспроизведение треков из списка в случайном порядке. </summary>
        /// <returns> Инструкции кода между блоками Yield. </returns>
        private IEnumerator PlayTracks()
        {
            while (true)
            {
                var tracks = GetShuffledTrackList();
                foreach (var track in tracks)
                {
                    PlayTrack(track);
                    yield return new WaitForSeconds(track.length);
                }
            }
        }

        /// <summary> Получить перемешанный список треков. </summary>
        /// <returns> Перемешанный список треков. </returns>
        private IEnumerable<AudioClip> GetShuffledTrackList() =>
            _trackList.OrderBy(_ => Random.Range(0, _trackList.Count));

        /// <summary> Воспроизведение указанного трека. </summary>
        /// <param name="audioClip"> Трек, который нужно воспроизвести. </param>
        private void PlayTrack(AudioClip audioClip)
        {
            _source.clip = audioClip;
            _source.Play();
        }
    }
}