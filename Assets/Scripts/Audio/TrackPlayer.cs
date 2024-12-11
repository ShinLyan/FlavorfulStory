using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Audio
{
    /// <summary> Проигрыватель треков.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class TrackPlayer : MonoBehaviour
    {
        /// <summary> Список аудиоклипов для песен.</summary>
        [SerializeField] private List<AudioClip> _tracks;

        /// <summary> Аудиоисточник.</summary>
        private AudioSource _source;

        /// <summary> Инициализация полей.</summary>
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        /// <summary> Метод для старта воспроизведения музыки.</summary>
        private void Start()
        {
            StartCoroutine(PlayTracks(_tracks));
        }

        /// <summary> Метод для воспроизведения трека.</summary>
        private void PlayTrack(AudioClip audioClip)
        {
            _source.clip = audioClip;
            _source.Play();
        }

        /// <summary> Метод для воспроизведения всех треков.</summary>
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