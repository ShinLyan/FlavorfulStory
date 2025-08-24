namespace FlavorfulStory
{
    public class NotEmptyValidator : IStringValidator
    {
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