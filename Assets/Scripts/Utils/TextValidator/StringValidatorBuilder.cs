namespace FlavorfulStory
{
    public class StringValidatorBuilder
    {
        private readonly CompositeValidator _composite = new();

        public StringValidatorBuilder NotEmpty()
        {
            _composite.Add(new NotEmptyValidator());
            return this;
        }

        public StringValidatorBuilder MinLength(int min)
        {
            _composite.Add(new MinLengthValidator(min));
            return this;
        }

        public StringValidatorBuilder MaxLength(int max)
        {
            _composite.Add(new MaxLengthValidator(max));
            return this;
        }

        public StringValidatorBuilder NoForbiddenCharacters(char[] forbiddenChars)
        {
            _composite.Add(new ForbiddenCharactersValidator(forbiddenChars));
            return this;
        }

        public IStringValidator Build() => _composite;
    }
}