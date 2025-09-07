namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что строка не пустая и не состоит только из пробелов. </summary>
    public class NotEmptyValidator : IStringValidator
    {
        /// <summary> Проверяет строку на корректность. </summary>
        /// <param name="input"> Входная строка. </param>
        /// <param name="error"> Сообщение об ошибке (если есть). </param>
        /// <returns> True — строка валидна, False — ошибка. </returns>
        public bool IsValid(string input, out string error)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "The field cannot be empty!";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}