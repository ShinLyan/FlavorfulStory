namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Интерфейс валидатора строки. </summary>
    public interface IStringValidator
    {
        /// <summary> Проверяет строку на корректность. </summary>
        /// <param name="input"> Входная строка. </param>
        /// <param name="error"> Сообщение об ошибке (если есть). </param>
        /// <returns> True — строка валидна, False — ошибка. </returns>
        bool IsValid(string input, out string error);
    }
}