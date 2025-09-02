using System.Collections.Generic;

namespace FlavorfulStory.Utils.StringValidator
{
    /// <summary> Композитный валидатор, объединяющий несколько проверок. </summary>
    public class CompositeValidator : IStringValidator
    {
        /// <summary> Список используемых валидаторов. </summary>
        private readonly List<IStringValidator> _validators = new();

        /// <summary> Добавляет новый валидатор в цепочку. </summary>
        /// <param name="validator"> Валидатор для добавления. </param>
        /// <returns> Текущий композитный валидатор (для чейнинга). </returns>
        public CompositeValidator Add(IStringValidator validator)
        {
            _validators.Add(validator);
            return this;
        }

        /// <inheritdoc />
        public bool IsValid(string input, out string error)
        {
            foreach (var validator in _validators)
                if (!validator.IsValid(input, out error))
                    return false;

            error = string.Empty;
            return true;
        }
    }
}