using FlavorfulStory.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory
{
    public class WindowSystemInstaller : MonoInstaller
    {
        [Header("UI Root")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private GraphicRaycaster _raycaster;
        
        [SerializeField] private CanvasGroupFader _hudFader;
        [SerializeField] private CanvasGroupFader _backgroundFader;

        public override void InstallBindings()
        {
            Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.BindInterfacesTo<WindowWarmupper>().AsSingle();
            Container.Bind<CanvasGroupFader>().WithId("HUD").FromInstance(_hudFader).AsCached();
            Container.Bind<CanvasGroupFader>().WithId("UIBackground").FromInstance(_backgroundFader).AsCached();
            Container.BindInterfacesAndSelfTo<UIOverlayFadeCoordinator>().AsSingle().NonLazy();
        }
    }
}