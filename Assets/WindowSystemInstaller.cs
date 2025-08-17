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

        public override void InstallBindings()
        {
            Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.BindInterfacesTo<WindowWarmupper>().AsSingle(); // (Initialize + WarmUpAsync) Фабрики
        }
    }
}