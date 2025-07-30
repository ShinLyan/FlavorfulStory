using FlavorfulStory.Audio;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Installers
{
    /// <summary> Устанавливает зависимости, связанные с аудиосистемой, такие как AmbientPlayer. </summary>
    public class AudioInstaller : MonoInstaller
    {
        /// <summary> Префаб аудиоисточника, предназначенного для фоновой музыки. </summary>
        [SerializeField] private AudioSource _ambientPrefab;

        /// <summary> Набор треков, воспроизводимых в качестве фоновой музыки. </summary>
        [SerializeField] private AmbientTrackSet _ambientTrackSet;

        /// <summary> Регистрирует зависимости в Zenject. </summary>
        public override void InstallBindings()
        {
            Container.Bind<AmbientPlayer>().FromMethod(ctx =>
            {
                var ambientSource = Container.InstantiatePrefabForComponent<AudioSource>(_ambientPrefab);
                ambientSource.gameObject.name = "Ambient";
                return new AmbientPlayer(ambientSource, _ambientTrackSet.Clips);
            }).AsSingle().NonLazy();
        }
    }
}