using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using FlavorfulStory.Infrastructure.Factories.Window;
using FlavorfulStory.Infrastructure.Services.WindowService;
using FlavorfulStory.UI.Animation;
using FlavorfulStory.UI.Windows;

namespace FlavorfulStory.Infrastructure.Installers
{
    /// <summary> Инсталлер для систем UI-окон, включая фабрику, сервис и опциональные анимации. </summary>
    public class WindowSystemInstaller : MonoInstaller
    {
        /// <summary> Основной Canvas для всех окон. </summary>
        [Header("UI Root")]
        [SerializeField] private Canvas _canvas;
        /// <summary> EventSystem для обработки ввода. </summary>
        [SerializeField] private EventSystem _eventSystem;
        /// <summary> Raycaster для UI-интеракций. </summary>
        [SerializeField] private GraphicRaycaster _raycaster;
        
        /// <summary> Затемнение HUD'а (опционально). </summary>
        [Header("Optional Fade")]
        [SerializeField] private CanvasGroupFader _hudFader;
        /// <summary> Затемнение заднего фона UI (опционально). </summary>
        [SerializeField] private CanvasGroupFader _backgroundFader;

        /// <summary> Привязывает сервис окон, фабрику и все зависимости UI-системы. </summary>
        public override void InstallBindings()
        {
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.BindInterfacesTo<WindowWarmupper>().AsSingle();
            
            Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
            Container.Bind<EventSystem>().FromInstance(_eventSystem).AsSingle();
            Container.Bind<GraphicRaycaster>().FromInstance(_raycaster).AsSingle();
            
            if (_hudFader != null)
                Container.Bind<CanvasGroupFader>().WithId("HUD").FromInstance(_hudFader).AsCached();
            if (_backgroundFader != null)
                Container.Bind<CanvasGroupFader>().WithId("UIBackground").FromInstance(_backgroundFader).AsCached();
            if (_hudFader != null && _backgroundFader != null) 
                Container.BindInterfacesAndSelfTo<UIOverlayFadeCoordinator>().AsSingle().NonLazy();
        }
    }
}