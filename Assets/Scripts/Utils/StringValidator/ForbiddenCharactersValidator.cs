using System.Linq;

namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что строка не содержит запрещённых символов. </summary>
    public class ForbiddenCharactersValidator : IStringValidator
    {
        /// <summary> Массив запрещённых символов. </summary>
        private static readonly char[] ForbiddenCharacters =
        {
            '"', '\'', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+',
            '[', ']', '{', '}', '\\', '|', '/', '<', '>', '?', '`', '~'
        };

        /// <summary> Проверяет строку на корректность. </summary>
        /// <param name="input"> Входная строка. </param>
        /// <param name="error"> Сообщение об ошибке (если есть). </param>
        /// <returns> True — строка валидна, False — ошибка. </returns>
        public bool IsValid(string input, out string error)
        {
            if (input.Any(c => ForbiddenCharacters.Contains(c)))
            {
                error = "The text contains forbidden characters!";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}