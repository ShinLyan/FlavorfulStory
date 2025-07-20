using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Audio
{
    /// <summary> Компонент для воспроизведения звуковых эффектов (SFX) в игре. </summary>
    public class SfxPlayer : IDisposable
    {
        /// <summary> Аудиоисточник, через который воспроизводятся звуки. </summary>
        private readonly AudioSource _audioSource;

        /// <summary> Список данных о звуковых эффектах. </summary>
        private readonly List<SfxData> _sfxDataList;

        /// <summary> Событие запроса на воспроизведение звука определённого типа. </summary>
        private static event Action<SfxType> OnPlayRequested;

        /// <summary> Конструктор. Подписывается на событие воспроизведения звука. </summary>
        /// <param name="audioSource"> Источник звука. </param>
        /// <param name="sfxDataList"> Список доступных звуков. </param>
        public SfxPlayer(AudioSource audioSource, List<SfxData> sfxDataList)
        {
            _audioSource = audioSource;
            _sfxDataList = sfxDataList;

            OnPlayRequested += HandlePlay;
        }

        /// <summary> Отписывается от событий при уничтожении объекта. </summary>
        public void Dispose() => OnPlayRequested -= HandlePlay;

        /// <summary> Проигрывает один случайный звуковой эффект указанного типа. </summary>
        /// <param name="type"> Тип проигрываемого звука. </param>
        public static void Play(SfxType type) => OnPlayRequested?.Invoke(type);

        /// <summary> Обработчик события воспроизведения. </summary>
        /// <param name="type"> Тип звука, который требуется воспроизвести. </param>
        private void HandlePlay(SfxType type)
        {
            var data = _sfxDataList.FirstOrDefault(sfxData => sfxData.Type == type);
            if (!data || data.Clips.Count == 0) return;

            var clip = data.Clips[Random.Range(0, data.Clips.Count)];
            if (clip) _audioSource.PlayOneShot(clip);
        }
    }
}