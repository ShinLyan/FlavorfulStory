using System;

namespace FlavorfulStory.UI
{
    /// <summary> Проверка полей ввода.</summary>
    public static class InputFieldValidator
    {
        /// <summary> Запрещенные символы.</summary>
        private static readonly char[] ForbiddenCharacters =
            { '"', '\'', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+',
            '[', ']', '{', '}', '\\', '|', '/', '<', '>', '?', '`', '~' };

        /// <summary> Минимальная длина текста.</summary>
        private const int MinLength = 3;

        /// <summary> Максимальная длина текста.</summary>
        private const int MaxLength = 20;

        /// <summary> Сообщение об ошибке: пустое поле.</summary>
        private static readonly string EmptyInputError = "Поле не может быть пустым!";

        /// <summary> Сообщение об ошибке: минимальная длина.</summary>
        private static readonly string TooShortError = $"Текст должен быть не менее {MinLength} символов.";

        /// <summary> Сообщение об ошибке: максимальная длина.</summary>
        private static readonly string TooLongError = $"Текст должен быть не более {MaxLength} символов.";

        /// <summary> Сообщение об ошибке: запрещённые символы.</summary>
        private static readonly string ForbiddenCharactersError = "Текст содержит запрещённые символы!";

        /// <summary> Проверяет, валиден ли текст ввода.</summary>
        /// <param name="input"> Текст для проверки.</param>
        /// <param name="warningMessage"> Сообщение об ошибке, если ввод невалиден.</param>
        /// <returns> True, если ввод валиден; иначе False.</returns>
        public static bool IsValid(string input, out string warningMessage)
        {
            input = input.Trim();

            // Список проверок с соответствующими сообщениями об ошибках
            var checks = new (Func<bool> Condition, string ErrorMessage)[]
            {
                (() => string.IsNullOrEmpty(input), EmptyInputError),
                (() => input.Length < MinLength, TooShortError),
                (() => input.Length > MaxLength, TooLongError),
                (() => ContainsForbiddenCharacters(input), ForbiddenCharactersError)
            };

            // Проверка условий
            foreach (var (condition, errorMessage) in checks)
            {
                if (condition())
                {
                    warningMessage = errorMessage;
                    return false;
                }
            }

            // Все проверки пройдены
            warningMessage = string.Empty;
            return true;
        }

        /// <summary> Проверяет, содержит ли текст запрещённые символы.</summary>
        /// <param name="input"> Текст для проверки.</param>
        /// <returns> True, если есть запрещённые символы; иначе False.</returns>
        private static bool ContainsForbiddenCharacters(string input)
        {
            foreach (char c in ForbiddenCharacters)
            {
                if (input.Contains(c.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}