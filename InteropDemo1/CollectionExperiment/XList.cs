using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

// https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Collections/Generic/List.cs

namespace Raf.Collections
{
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public partial class XList<T> :
        IList<T>,
        ICollection<T>,
        IEnumerable<T>
    {
        private object _syncRoot = new object();
        private T[] _storage;
        private int _internalCount;

        public XList()
        {
            CreateArray();
        }

        public Span<T> Span => _storage.AsSpan(0, _internalCount);
        public ReadOnlySpan<T> ReadOnlySpan => Span;
        public int Count => _internalCount;
        public ref T this[int index] => ref _storage[index];
        public void RemoveAt(int index) => Collapse(index, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                for (int i = 0; i < _internalCount; i++)
                {
                    _storage[i] = default(T);
                }
            }

            _internalCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (_internalCount >= _storage.Length)
            {
                GrowArrayOneElement();
            }

            _storage[_internalCount] = item;
            _internalCount++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items is ICollection<T> coll)
            {
                var newIndex = _internalCount;
                ReserveSpace(coll.Count);
                foreach (var item in items)
                {
                    _storage[newIndex] = item;
                    newIndex++;
                }

                return;
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}
