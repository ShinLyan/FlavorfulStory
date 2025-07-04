namespace FlavorfulStory.Core
{
    /// <summary> Интерфейс для объектов, которые могут оценивать предикаты условий. </summary>
    public interface IPredicateEvaluator
    {
        /// <summary> Оценивает заданный предикат с параметрами. </summary>
        /// <param name="predicate"> Имя предиката для проверки. </param>
        /// <param name="parameters"> Массив параметров для предиката. </param>
        /// <returns> True, если условие выполнено; false, если не выполнено;
        /// null, если предикат не поддерживается. </returns>
        bool? Evaluate(string predicate, string[] parameters);
    }
}