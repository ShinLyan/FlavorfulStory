namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что длина строки не превышает заданную. </summary>
    public class MaxLengthValidator : IStringValidator
    {
        /// <summary> Максимально допустимая длина строки. </summary>
        private readonly int _max;

        /// <summary> Конструктор. </summary>
        /// <param name="max"> Максимальная длина строки. </param>
        public MaxLengthValidator(int max) => _max = max;

        /// <inheritdoc />
        public bool IsValid(string input, out string error)
        {
            if (input.Length > _max)
            {
                error = $"The text must be no more than {_max} characters long.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}