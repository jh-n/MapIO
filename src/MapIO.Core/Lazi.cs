using System;

namespace MapIO.Core
{

    /// <summary>
    /// Similar to Lazy&lt;T&gt; but can be reset and reassigned
    /// </summary>
    /// <typeparam name="T">Type of stored value</typeparam>
    public class Lazi<T> : IDisposable
    {
        public event Action<T> OnReset;
        public event Action<T, T> OnChange;
        public bool HasValue { get; private set; } = false;

        T _value;
        readonly Func<T> _valueFactory;
        readonly Func<T, T, bool> _cachePredicate;

        public Lazi(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <param name="cachePredicate">Determine whether to cache the result</param>
        public Lazi(Func<T> valueFactory, Func<T, T, bool> cachePredicate) : this(valueFactory)
        {
            _cachePredicate = cachePredicate;
        }

        public T Value
        {
            get
            {
                if (HasValue) return _value;
                lock (this)
                {
                    if (HasValue) return _value;
                    var newValue = _valueFactory();
                    var shouldCache = _cachePredicate == null || _cachePredicate(_value, newValue);
                    _value = newValue;
                    if (shouldCache) HasValue = true;
                    else Reset();
                }
                return _value;
            }
            set
            {
                _value = value;
                HasValue = true;
                OnChange?.Invoke(_value, value);
            }
        }

        public void Dispose()
        {
            Reset();
            _value = default;
        }

        public void Reset()
        {
            HasValue = false;
            OnReset?.Invoke(_value);
        }

        public void Observe<TOther>(Lazi<TOther> other, ObservationMode observationMode = ObservationMode.Reset)
        {
            var mode = (int)observationMode;
            if ((mode & 0b1) == 0b1) other.OnReset += v => Reset();
            if ((mode & 0b10) == 0b10) other.OnChange += (ov, nv) => Reset();
        }

    }

    public enum ObservationMode
    {
        Reset = 0b1,
        Change = 0b10,
    }
}