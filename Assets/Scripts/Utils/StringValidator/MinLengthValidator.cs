namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что длина строки не меньше заданной. </summary>
    public class MinLengthValidator : IStringValidator
    {
        /// <summary> Минимально допустимая длина строки. </summary>
        private readonly int _min;

        /// <summary> Конструктор. </summary>
        /// <param name="min"> Минимальная длина строки. </param>
        public MinLengthValidator(int min) => _min = min;

        /// <inheritdoc />
        public bool IsValid(string input, out string error)
        {
            if (input.Length < _min)
            {
                error = $"The text must be at least {_min} characters long.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}