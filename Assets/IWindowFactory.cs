using Cysharp.Threading.Tasks;

namespace FlavorfulStory
{
    public interface IWindowFactory
    {
        void Initialize();
        UniTask WarmUpAsync();
        T CreateWindow<T>() where T : BaseWindow;
    }
}