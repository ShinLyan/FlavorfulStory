using System;
using System.Linq;

namespace FlavorfulStory.UI
{
    /// <summary> Валидация ввода в текстовые поля. </summary>
    public static class InputFieldValidator
    {
        /// <summary> Запрещённые символы. </summary>
        private static readonly char[] ForbiddenCharacters =
        {
            '"', '\'', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+',
            '[', ']', '{', '}', '\\', '|', '/', '<', '>', '?', '`', '~'
        };

        /// <summary> Минимальная длина строки. </summary>
        private const int MinLength = 3;

        /// <summary> Максимальная длина строки. </summary>
        private const int MaxLength = 20;

        /// <summary> Ошибка: пустое поле. </summary>
        private const string EmptyInputError = "The field cannot be empty!";

        /// <summary> Ошибка: текст слишком короткий. </summary>
        private static readonly string TooShortError = $"The text must be at least {MinLength} characters long.";

        /// <summary> Ошибка: текст слишком длинный. </summary>
        private static readonly string TooLongError = $"The text must be no more than {MaxLength} characters long.";

        /// <summary> Ошибка: запрещённые символы. </summary>
        private const string ForbiddenCharactersError = "The text contains forbidden characters!";

        /// <summary> Проверка валидности строки. </summary>
        /// <param name="input"> Строка для проверки. </param>
        /// <param name="warningMessage"> Сообщение об ошибке, если строка невалидна. </param>
        /// <returns> True, если строка валидна; иначе False. </returns>
        public static bool IsValid(string input, out string warningMessage)
        {
            input = input.Trim();

            // Условия проверки и соответствующие ошибки
            var checks = new (Func<bool> Condition, string ErrorMessage)[]
            {
                (() => string.IsNullOrEmpty(input), EmptyInputError),
                (() => input.Length < MinLength, TooShortError),
                (() => input.Length > MaxLength, TooLongError),
                (() => ContainsForbiddenCharacters(input), ForbiddenCharactersError)
            };

            // Проверяем каждое условие
            foreach ((var condition, string errorMessage) in checks)
            {
                if (!condition()) continue;
                warningMessage = errorMessage;
                return false;
            }

            // Если ошибок нет
            warningMessage = string.Empty;
            return true;
        }

        /// <summary> Проверка, содержит ли строка запрещённые символы. </summary>
        /// <param name="input"> Строка для проверки. </param>
        /// <returns> True, если строка содержит запрещённые символы; иначе False. </returns>
        private static bool ContainsForbiddenCharacters(string input)
            => ForbiddenCharacters.Any(character => input.IndexOf(character) >= 0);
    }
}