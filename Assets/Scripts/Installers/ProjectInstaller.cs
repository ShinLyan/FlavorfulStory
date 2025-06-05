using FlavorfulStory.SceneManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Installers
{
    /// <summary> Устанавливает зависимости, необходимые на уровне проекта. </summary>
    public class ProjectInstaller : MonoInstaller
    {
        /// <summary> Префаб компонента Fader, используемого для управления затемнением UI. </summary>
        [SerializeField] private GameObject _faderPrefab;

        /// <summary> Вызывает методы для привязки зависимостей. </summary>
        public override void InstallBindings()
        {
            BindSavingWrapper();
            BindUI();
        }

        /// <summary> Привязывает компонент SavingWrapper как singleton на новом GameObject. </summary>
        private void BindSavingWrapper()
        {
            Container.Bind<SavingWrapper>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("SavingWrapper")
                .AsSingle();
        }

        /// <summary> Привязывает компонент Fader из указанного префаба как singleton. </summary>
        private void BindUI()
        {
            Container.Bind<Fader>().FromComponentInNewPrefab(_faderPrefab).AsSingle().NonLazy();
        }
    }
}