using FlavorfulStory.UI.Animation;
using FlavorfulStory.Windows;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Installers
{
    /// <summary> Инсталлер для систем UI-окон, включая фабрику, сервис и опциональные анимации. </summary>
    public class WindowSystemInstaller : MonoInstaller
    {
        /// <summary> Основной Canvas для всех окон. </summary>
        [Header("UI Root")]
        [SerializeField] private Canvas _canvas;

        /// <summary> Затемнение HUD'а (опционально). </summary>
        [Header("Optional Fade")]
        [SerializeField] private CanvasGroupFader _hudFader;

        /// <summary> Затемнение заднего фона UI (опционально). </summary>
        [SerializeField] private CanvasGroupFader _backgroundFader;

        /// <summary> Привязывает сервис окон, фабрику и все зависимости UI-системы. </summary>
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<WindowService>().AsSingle();
            Container.BindInterfacesTo<WindowFactory>().AsSingle();

            Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();

            if (_hudFader) Container.Bind<CanvasGroupFader>().WithId("HUD").FromInstance(_hudFader).AsCached();
            if (_backgroundFader)
                Container.Bind<CanvasGroupFader>().WithId("UIBackground").FromInstance(_backgroundFader).AsCached();
            if (_hudFader && _backgroundFader)
                Container.BindInterfacesAndSelfTo<UIOverlayFadeCoordinator>().AsSingle().NonLazy();
        }
    }
}