using Cysharp.Threading.Tasks;
using FlavorfulStory.UI.Windows;

namespace FlavorfulStory.Infrastructure.Factories.Window
{
    /// <summary> Фабрика окон: инициализация, прогрев, создание окон по типу. </summary>
    public interface IWindowFactory
    {
        /// <summary> Привязка к корневому канвасу. </summary>
        void Initialize();
        /// <summary> Прогрев фабрики: регистрация всех префабов в словарь. </summary>
        UniTask WarmUpAsync();
        /// <summary> Создает окно по типу и внедряет зависимости. </summary>
        /// <typeparam name="T"> Тип окна. </typeparam>
        T CreateWindow<T>() where T : BaseWindow;
    }
}