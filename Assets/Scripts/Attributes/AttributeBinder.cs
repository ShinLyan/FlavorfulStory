namespace FlavorfulStory.Attributes
{
    /// <summary> Предоставляет методы для привязки и отвязки событий атрибута к представлению. </summary>
    public class AttributeBinder
    {
        /// <summary> Подписывает представление на события изменения атрибута. </summary>
        /// <param name="attribute"> Атрибут, на события которого нужно подписаться. </param>
        /// <param name="view"> Представление, реагирующее на изменения атрибута. </param>
        public static void Bind(IAttribute attribute, IAttributeView view)
        {
            attribute.OnValueChanged += view.HandleAttributeChange;
            attribute.OnReachedZero += view.HandleAttributeReachZero;
            attribute.OnMaxValueChanged += view.HandleAttributeMaxValueChanged;
        }

        /// <summary> Отписывает представление от событий изменения атрибута. </summary>
        /// <param name="attribute"> Атрибут, от событий которого нужно отписаться. </param>
        /// <param name="view"> Представление, ранее реагировавшее на изменения атрибута. </param>
        public static void Unbind(IAttribute attribute, IAttributeView view)
        {
            attribute.OnValueChanged -= view.HandleAttributeChange;
            attribute.OnReachedZero -= view.HandleAttributeReachZero;
            attribute.OnMaxValueChanged -= view.HandleAttributeMaxValueChanged;
        }
    }
}