using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Audio
{
    /// <summary> Проигрыватель треков, обеспечивающий цикличное воспроизведение случайных треков из списка. </summary> 
    public class AmbientPlayer
    {
        /// <summary> Источник звука для воспроизведения треков. </summary>
        private readonly AudioSource _audioSource;

        /// <summary> Список треков для воспроизведения. </summary>
        private readonly List<AudioClip> _trackList;

        /// <summary> Токен отмены для прерывания воспроизведения. </summary>
        private readonly CancellationTokenSource _cancellation;

        /// <summary> Конструктор, запускающий цикличное воспроизведение треков. </summary>
        /// <param name="audioSource"> Источник аудио. </param>
        /// <param name="trackList"> Список доступных треков. </param>
        public AmbientPlayer(AudioSource audioSource, List<AudioClip> trackList)
        {
            _audioSource = audioSource;
            _trackList = trackList;
            _cancellation = new CancellationTokenSource();

            if (_trackList == null || _trackList.Count == 0)
            {
                Debug.LogError("AmbientSound: Пустой список треков.");
                return;
            }

            PlayTracksAsync(_cancellation.Token).Forget();
        }

        /// <summary> Воспроизводит треки по очереди в случайном порядке до отмены. </summary>
        /// <param name="token"> Токен отмены задачи. </param>
        private async UniTaskVoid PlayTracksAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
                foreach (var track in GetShuffledTrackList())
                {
                    _audioSource.clip = track;
                    _audioSource.Play();
                    await UniTask.Delay(TimeSpan.FromSeconds(track.length), cancellationToken: token);
                }
        }

        /// <summary> Получить перемешанный список треков. </summary>
        /// <returns> Перемешанный список треков. </returns>
        private IEnumerable<AudioClip> GetShuffledTrackList() =>
            _trackList.OrderBy(_ => Random.Range(0, _trackList.Count));

        /// <summary> Останавливает воспроизведение и отменяет задачу. </summary>
        public void Stop()
        {
            _cancellation.Cancel();
            _audioSource.Stop();
        }
    }
}