using System.Linq;

namespace FlavorfulStory
{
    public class ForbiddenCharactersValidator : IStringValidator
    {
        private readonly char[] _forbidden;

        public ForbiddenCharactersValidator(char[] forbidden) => _forbidden = forbidden;

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