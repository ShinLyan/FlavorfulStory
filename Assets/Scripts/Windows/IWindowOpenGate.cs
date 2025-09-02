using System;
using FlavorfulStory.Windows.UI;

namespace FlavorfulStory.Windows
{
    /// <summary> Гейт, который гарантирует фейды перед открытием первого окна. </summary>
    public interface IWindowOpenGate
    {
        /// <summary> Запросить открытие окна: либо сразу, либо после фейдов (если это первое окно). </summary>
        void RequestOpen(BaseWindow window, Action openAction);
    }
}