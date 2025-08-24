namespace FlavorfulStory
{
    public class InputFieldValidatorBuilder
    {
        private readonly CompositeValidator _composite = new();

        public InputFieldValidatorBuilder NotEmpty()
        {
            _composite.Add(new NotEmptyValidator());
            return this;
        }

        public InputFieldValidatorBuilder MinLength(int min)
        {
            _composite.Add(new MinLengthValidator(min));
            return this;
        }

        public InputFieldValidatorBuilder MaxLength(int max)
        {
            _composite.Add(new MaxLengthValidator(max));
            return this;
        }

        public InputFieldValidatorBuilder NoForbiddenCharacters(char[] forbiddenChars)
        {
            _composite.Add(new ForbiddenCharactersValidator(forbiddenChars));
            return this;
        }

        public IStringValidator Build() => _composite;
    }
}