using System.Collections.Generic;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class StateContext
    {
        private readonly Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value) => _data[key] = value;

        public T Get<T>(string key) => _data.TryGetValue(key, out var value) ? (T)value : default;

        public bool TryGet<T>(string key, out T value)
        {
            if (_data.TryGetValue(key, out var objValue) && objValue is T)
            {
                value = (T)objValue;
                return true;
            }

            value = default;
            return false;
        }
    }
}