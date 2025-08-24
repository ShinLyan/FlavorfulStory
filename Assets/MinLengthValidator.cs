namespace FlavorfulStory
{
    public class MinLengthValidator : IStringValidator
    {
        private readonly int _min;

        public MinLengthValidator(int min) => _min = min;

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