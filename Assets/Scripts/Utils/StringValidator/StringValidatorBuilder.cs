namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Builder для последовательной сборки валидаторов строки. </summary>
    public class StringValidatorBuilder
    {
        /// <summary> Внутренний композитный валидатор, аккумулирующий проверки. </summary>
        private readonly CompositeValidator _composite = new();

        /// <summary> Добавляет проверку на непустую строку. </summary>
        /// <returns> Текущий билдер для чейнинга. </returns>
        public StringValidatorBuilder NotEmpty()
        {
            _composite.Add(new NotEmptyValidator());
            return this;
        }

        /// <summary> Добавляет проверку на минимальную длину строки. </summary>
        /// <param name="min"> Минимально допустимая длина. </param>
        /// <returns> Текущий билдер для чейнинга. </returns>
        public StringValidatorBuilder MinLength(int min)
        {
            _composite.Add(new MinLengthValidator(min));
            return this;
        }

        /// <summary> Добавляет проверку на максимальную длину строки. </summary>
        /// <param name="max"> Максимально допустимая длина. </param>
        /// <returns> Текущий билдер для чейнинга. </returns>
        public StringValidatorBuilder MaxLength(int max)
        {
            _composite.Add(new MaxLengthValidator(max));
            return this;
        }

        /// <summary> Добавляет проверку на отсутствие запрещённых символов. </summary>
        /// <returns> Текущий билдер для чейнинга. </returns>
        public StringValidatorBuilder NoForbiddenCharacters()
        {
            _composite.Add(new ForbiddenCharactersValidator());
            return this;
        }

        /// <summary> Собирает итоговый валидатор. </summary>
        /// <returns> Композитный валидатор, объединяющий добавленные проверки. </returns>
        public IStringValidator Build() => _composite;
    }
}