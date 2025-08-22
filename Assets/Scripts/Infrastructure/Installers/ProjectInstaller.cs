using FlavorfulStory.Audio;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TooltipSystem;
using FlavorfulStory.UI.Animation;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Installers
{
    /// <summary> Устанавливает зависимости, необходимые на уровне проекта. </summary>
    public class ProjectInstaller : MonoInstaller
    {
        /// <summary> Префаб компонента Fader, используемого для управления затемнением UI. </summary>
        [Header("UI")]
        [SerializeField] private GameObject _faderPrefab;

        /// <summary> Префаб всплывающей подсказки для кнопки. </summary>
        [SerializeField] private ButtonTooltipView _buttonTooltipPrefab;

        /// <summary> Префаб источника звука для воспроизведения звуковых эффектов (SFX). </summary>
        [Header("Audio")]
        [SerializeField] private AudioSource _sfxPrefab;

        /// <summary> Хранилище всех доступных SFX-данных. </summary>
        [SerializeField] private SfxDatabase _sfxDatabase;

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

            Container.Bind<ButtonTooltipView>().FromInstance(_buttonTooltipPrefab).AsSingle();


            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<SaveCompletedSignal>();
        }
    }
}