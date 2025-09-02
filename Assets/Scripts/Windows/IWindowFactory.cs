using FlavorfulStory.Windows.UI;

namespace FlavorfulStory.Windows
{
    /// <summary> Фабрика окон: инициализация, прогрев, создание окон по типу. </summary>
    public interface IWindowFactory
    {
        /// <summary> Создает окно по типу и внедряет зависимости. </summary>
        /// <typeparam name="T"> Тип окна. </typeparam>
        T CreateWindow<T>() where T : BaseWindow;
    }
}