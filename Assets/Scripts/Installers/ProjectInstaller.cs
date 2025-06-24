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
        [SerializeField] private GameObject _faderPrefab;

        /// <summary> Вызывает методы для привязки зависимостей. </summary>
        public override void InstallBindings()
        {
            Container.Bind<SavingWrapper>().FromNewComponentOnNewGameObject()
                .WithGameObjectName("SavingWrapper").AsSingle();

            Container.Bind<CanvasGroupFader>().FromComponentInNewPrefab(_faderPrefab).AsSingle().NonLazy();
        }
    }
}