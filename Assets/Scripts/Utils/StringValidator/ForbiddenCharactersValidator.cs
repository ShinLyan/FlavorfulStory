using System.Linq;

namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что строка не содержит запрещённых символов. </summary>
    public class ForbiddenCharactersValidator : IStringValidator
    {
        /// <summary> Массив запрещённых символов. </summary>
        private readonly char[] _forbidden;

        /// <summary> Конструктор. </summary>
        /// <param name="forbidden"> Запрещённые символы. </param>
        public ForbiddenCharactersValidator(char[] forbidden) => _forbidden = forbidden;

        /// <inheritdoc />
        public bool IsValid(string input, out string error)
        {
            if (input.Any(c => _forbidden.Contains(c)))
            {
                error = "The text contains forbidden characters!";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}