using System;
using System.Collections;
using System.Collections.Generic;

namespace JiteLang.Utilities
{
    internal class NotifyAddList<T> : IList<T>, IReadOnlyList<T>
    {
        protected List<T> _list;
        public NotifyAddList(IEnumerable<T> values)
        {
            _list = new(values);
        }
        
        public NotifyAddList(int capacity)
        {
            _list = new(capacity);
        }

        public NotifyAddList()
        {
            _list = new();
        }
  
        public event Action<T>? OnAdd;

        public int Count => _list.Count;

        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        public void Add(T item)
        {
            _list.Add(item);
            OnAdd?.Invoke(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            OnAdd?.Invoke(item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
