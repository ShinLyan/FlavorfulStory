using Cysharp.Threading.Tasks;
using Zenject;

namespace FlavorfulStory.Windows
{
    /// <summary> Инициализатор фабрики окон: задаёт корень и запускает прогрев префабов. </summary>
    public class WindowWarmupper : IInitializable
    {
        /// <summary> Фабрика UI-окон. </summary>
        private readonly IWindowFactory _windowFactory;

        /// <summary> Внедрение зависимости фабрики. </summary>
        public WindowWarmupper(IWindowFactory windowFactory) => _windowFactory = windowFactory;

        /// <summary> Инициализация и прогрев фабрики. </summary>
        public void Initialize()
        {
            _windowFactory.Initialize();
            _windowFactory.WarmUpAsync().Forget();
        }
    }
}