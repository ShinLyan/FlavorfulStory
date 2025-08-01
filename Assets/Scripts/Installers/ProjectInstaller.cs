using FlavorfulStory.Audio;
using FlavorfulStory.LocalizationSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI.Animation;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Installers
{
    /// <summary> Устанавливает зависимости, необходимые на уровне проекта. </summary>
    public class ProjectInstaller : MonoInstaller
    {
        /// <summary> Префаб компонента Fader, используемого для управления затемнением UI. </summary>
        [Header("UI")] [SerializeField] private GameObject _faderPrefab;

        /// <summary> Префаб источника звука для воспроизведения звуковых эффектов (SFX). </summary>
        [Header("Audio")] [SerializeField] private AudioSource _sfxPrefab;

        /// <summary> Хранилище всех доступных SFX-данных. </summary>
        [SerializeField] private SfxDatabase _sfxDatabase;

        [SerializeField] private LocalizationDatabase _localizationDatabase;

        /// <summary> Вызывает методы для привязки зависимостей. </summary>
        public override void InstallBindings()
        {
            Container.Bind<SavingWrapper>().FromNewComponentOnNewGameObject()
                .WithGameObjectName("SavingWrapper").AsSingle();

            Container.Bind<CanvasGroupFader>().FromComponentInNewPrefab(_faderPrefab).AsSingle().NonLazy();

            Container.Bind<SfxPlayer>().FromMethod(_ =>
            {
                var sfxSource = Container.InstantiatePrefabForComponent<AudioSource>(_sfxPrefab);
                sfxSource.gameObject.name = "SFX";
                return new SfxPlayer(sfxSource, _sfxDatabase.SfxList);
            }).AsSingle().NonLazy();

            Container
                .Bind<LocalizationService>()
                .AsSingle()
                .WithArguments(_localizationDatabase, "EN");
            Container.Resolve<LocalizationService>();
        }
    }
}