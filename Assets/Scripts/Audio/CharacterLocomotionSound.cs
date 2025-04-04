using UnityEngine;

namespace FlavorfulStory.Audio
{
    public class CharacterLocomotionSound : MonoBehaviour
    {
        /// <summary> Источник проигрывания шагов. </summary>
        private AudioSource _audioSource;

        /// <summary> Звук шагов. </summary>
        [SerializeField] private AudioClip _walkClip, _runClip;

        /// <summary> Получить ссылку на источник аудио. </summary>
        private void Awake() => _audioSource = GetComponent<AudioSource>();

        /// <summary> Включить источник звуков передвижения. </summary>
        public void Enable()
        {
            _audioSource.mute = false;
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }

        /// <summary> Выключить источник звуков передвижения. </summary>
        public void Disable()
        {
            _audioSource.Stop();
            _audioSource.mute = true;
        }

        /// <summary> Установить режим ходьбы. </summary>
        public void SetWalkingState() => _audioSource.clip = _walkClip;

        /// <summary> Установить режим бега. </summary>
        public void SetRunningState() => _audioSource.clip = _runClip;
    }
}