using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Raf.Collections
{
    public partial class XList<T> : ICollection
    {
        void ICollection.CopyTo(Array array, int index) => ((ICollection<T>)this).CopyTo((T[])array, index);

        int ICollection.Count => ((ICollection<T>)this).Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => _syncRoot;
    }

    public partial class XList<T> : ICollection<T>
    {
        void ICollection<T>.Add(T item) => Add(item);

        void ICollection<T>.Clear() => Clear();

        bool ICollection<T>.Contains(T item)
        {
            int index = Span.BinarySearch(item, Comparer);
            return index >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var destination = new Span<T>(array, arrayIndex, array.Length - arrayIndex);
            Span.CopyTo(destination);
        }

        int ICollection<T>.Count => _internalCount;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            var index = Span.BinarySearch(item, Comparer);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }
    }
}
