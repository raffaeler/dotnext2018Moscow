using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Raf.Collections
{
    public partial class XList<T> : IList
    {
        int IList.Add(object value)
        {
            ((ICollection<T>)this).Add((T)value);
            return _internalCount - 1;
        }

        void IList.Clear() => ((IList<T>)this).Clear();

        bool IList.Contains(object value) => ((IList<T>)this).Contains((T)value);

        int IList.IndexOf(object value) => ((IList<T>)this).IndexOf((T)value);

        void IList.Insert(int index, object value) => ((IList<T>)this).Insert(index, (T)value);

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        void IList.Remove(object value) => ((IList<T>)this).Remove((T)value);

        void IList.RemoveAt(int index) => ((IList<T>)this).RemoveAt(index);

        object IList.this[int index]
        {
            get => ((IList<T>)this)[index];
            set => ((IList<T>)this)[index] = (T)value;
        }
    }

    public partial class XList<T> : IList<T>
    {
        public IComparer<T> Comparer = Comparer<T>.Default;

        int IList<T>.IndexOf(T item) => Span.BinarySearch(item, Comparer);

        void IList<T>.Insert(int index, T item)
        {
            ReserveSpace(index, 1);
            Span[index] = item;
        }

        void IList<T>.RemoveAt(int index) => RemoveAt(index);

        T IList<T>.this[int index]
        {
            get => Span[index];
            set => Span[index] = value;
        }
    }

}
