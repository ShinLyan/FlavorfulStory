namespace FlavorfulStory
{
    public class MaxLengthValidator : IStringValidator
    {
        private readonly int _max;

        public MaxLengthValidator(int max) => _max = max;

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