using Cysharp.Threading.Tasks;
using Zenject;

namespace FlavorfulStory
{
    public class WindowWarmupper : IInitializable
    {
        private readonly IWindowFactory _windowFactory;

        public WindowWarmupper(IWindowFactory windowFactory) => _windowFactory = windowFactory;

        public void Initialize()
        {
            _windowFactory.Initialize();
            _windowFactory.WarmUpAsync().Forget();
        }
    }
}