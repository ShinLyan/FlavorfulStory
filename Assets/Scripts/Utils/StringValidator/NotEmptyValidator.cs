namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Валидатор: проверяет, что строка не пустая и не состоит только из пробелов. </summary>
    public class NotEmptyValidator : IStringValidator
    {
        /// <inheritdoc />
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