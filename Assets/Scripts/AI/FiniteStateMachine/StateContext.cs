using System.Collections.Generic;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контекст состояния для хранения и передачи данных между состояниями. </summary>
    public class StateContext
    {
        /// <summary> Словарь для хранения данных по ключам. </summary>
        private readonly Dictionary<ContextType, object> _data = new();

        /// <summary> Устанавливает значение по указанному ключу. </summary>
        /// <typeparam name="T"> Тип сохраняемого значения. </typeparam>
        /// <param name="key"> Ключ для сохранения значения. </param>
        /// <param name="value"> Значение для сохранения. </param>
        public void Set<T>(ContextType key, T value) => _data[key] = value;

        /// <summary> Пытается получить значение по указанному ключу с проверкой типа. </summary>
        /// <typeparam name="T"> Тип получаемого значения. </typeparam>
        /// <param name="key"> Ключ для получения значения. </param>
        /// <param name="value"> Выходной параметр для полученного значения. </param>
        /// <returns> true, если значение найдено и соответствует типу T; иначе false. </returns>
        public bool TryGet<T>(ContextType key, out T value)
        {
            if (_data.TryGetValue(key, out object objValue) && objValue is T value1)
            {
                value = value1;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary> Очищает весь контекст, удаляя все сохранённые данные. </summary>
        public void Clear() => _data.Clear(); // TODO: НИГДЕ НЕ ИСПОЛЬЗУЕТСЯ
    }
}