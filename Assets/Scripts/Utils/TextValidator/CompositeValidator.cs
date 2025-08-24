using System.Collections.Generic;

namespace FlavorfulStory
{
    public class CompositeValidator : IStringValidator
    {
        private readonly List<IStringValidator> _validators = new();

        public CompositeValidator Add(IStringValidator validator)
        {
            _validators.Add(validator);
            return this;
        }

        public bool IsValid(string input, out string error)
        {
            foreach (var validator in _validators)
            {
                if (!validator.IsValid(input, out error))
                    return false;
            }

            error = string.Empty;
            return true;
        }
    }
}