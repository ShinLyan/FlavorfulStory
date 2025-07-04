using FlavorfulStory.Infrastructure.Factories;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Фабрика для создания кнопок квестов в списке квестов. </summary>
    public class QuestListButtonFactory : GameFactoryBase<QuestListButton>
    {
        /// <summary> Конструктор фабрики кнопок квестов. </summary>
        /// <param name="container"> DI контейнер Zenject для инстанцирования. </param>
        /// <param name="prefab"> Префаб кнопки квеста. </param>
        public QuestListButtonFactory(DiContainer container, QuestListButton prefab) : base(container, prefab) { }
    }
}