using System;
using System.Collections.Generic;

namespace FlavorfulStory.Infrastructure
{
    /// <summary> Обобщённый пул объектов. </summary>
    public class ObjectPool<T> : IDisposable
    {
        /// <summary> Список неактивных объектов в пуле. </summary>
        private readonly List<T> _list;

        /// <summary> Функция создания нового объекта. </summary>
        private readonly Func<T> _createFunc;

        /// <summary> Функция инициализации объекта при получении. </summary>
        private readonly Action<T> _initFunc;

        /// <summary> Функция деинициализации объекта при возврате. </summary>
        private readonly Action<T> _deInitFunc;

        /// <summary> Действие, выполняемое при возврате объекта в пул. </summary>
        private readonly Action<T> _actionOnRelease;

        /// <summary> Действие, выполняемое при уничтожении объекта (если пул переполнен). </summary>
        private readonly Action<T> _actionOnDestroy;

        /// <summary> Максимальное количество объектов в пуле. </summary>
        private readonly int _maxSize;

        /// <summary> Проверка наличия дубликатов при возврате в пул. </summary>
        private readonly bool _collectionCheck;

        /// <summary> Общее количество созданных объектов. </summary>
        public int CountAll { get; private set; }

        /// <summary> Количество объектов, находящихся в пуле (неактивных). </summary>
        public int CountInactive => _list.Count;

        /// <summary> Количество объектов, находящихся вне пула (активных). </summary>
        public int CountActive => CountAll - CountInactive;

        /// <summary> Создаёт новый экземпляр пула объектов. </summary>
        /// <param name="createFunc"> Функция создания объекта. </param>
        /// <param name="initFunc"> Функция инициализации объекта. </param>
        /// <param name="deInitFunc"> Функция деинициализации объекта. </param>
        /// <param name="actionOnRelease"> Действие при возврате объекта. </param>
        /// <param name="actionOnDestroy"> Действие при уничтожении объекта. </param>
        /// <param name="collectionCheck"> Проверять ли дубликаты в пуле? </param>
        /// <param name="defaultCapacity"> Начальная ёмкость пула. </param>
        /// <param name="maxSize"> Максимальный размер пула. </param>
        public ObjectPool(Func<T> createFunc, Action<T> initFunc = null, Action<T> deInitFunc = null,
            Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true,
            int defaultCapacity = 10, int maxSize = 10000)
        {
            if (maxSize <= 0) throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));

            _list = new List<T>(defaultCapacity);
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _initFunc = initFunc;
            _deInitFunc = deInitFunc;
            _maxSize = maxSize;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _collectionCheck = collectionCheck;
        }

        /// <summary> Получить объект из пула или создать новый, если пул пуст. </summary>
        public T Get()
        {
            T obj;
            if (_list.Count == 0)
            {
                obj = _createFunc();
                CountAll++;
            }
            else
            {
                int index = _list.Count - 1;
                obj = _list[index];
                _list.RemoveAt(index);
            }

            _initFunc?.Invoke(obj);

            return obj;
        }

        /// <summary> Вернуть объект в пул. Будет уничтожен, если превышен лимит. </summary>
        /// <param name="element"> Объект, который нужно вернуть. </param>
        public void Release(T element)
        {
            if (_collectionCheck && _list.Count > 0)
                foreach (var t in _list)
                    if ((object)element == (object)t)
                        throw new InvalidOperationException(
                            "Trying to release an object that has already been released to the pool.");

            _deInitFunc?.Invoke(element);
            _actionOnRelease?.Invoke(element);

            if (CountInactive < _maxSize)
                _list.Add(element);
            else
                _actionOnDestroy?.Invoke(element);
        }

        /// <summary> Очистить пул. </summary>
        /// <remarks> Уничтожить все находящиеся в нём объекты. </remarks>
        public void Clear()
        {
            foreach (var obj in _list) _actionOnDestroy?.Invoke(obj);

            _list.Clear();
            CountAll = 0;
        }

        /// <summary> Удаляет все объекты и очищает пул. </summary>
        /// <remarks> Для unmanaged ресурсов. </remarks>
        public void Dispose() => Clear();
    }
}