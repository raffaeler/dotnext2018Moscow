using System;
using System.Collections.Generic;
using System.Text;

namespace Raf.Collections
{
    public partial class XList<T>
    {
        /// <summary>
        /// Create the initial array (1K)
        /// </summary>
        private void CreateArray()
        {
            _storage = new T[1024];
            _internalCount = 0;
            //_main = new Memory<T>(_storage);
        }

        /// <summary>
        /// The growing array policy
        /// </summary>
        private Func<int, int> _growPolicy = x => x * 2;

        private int GetNewSize(int minimumFinalSize = 0)
        {
            int newSize = _growPolicy(_storage.Length);
            while (minimumFinalSize > newSize)
                newSize = _growPolicy(newSize);

            return newSize;
        }

        /// <summary>
        /// Expand array size
        /// Special case of a new single increment requested (add)
        /// </summary>
        private void GrowArrayOneElement()
        {
            T[] newStorage = new T[_growPolicy(_storage.Length)];
            if (_internalCount > 0)
                Array.Copy(_storage, newStorage, _internalCount);
            _storage = newStorage;
        }

        /// <summary>
        /// Expand array size
        /// </summary>
        private void GrowArray(int minimumFinalSize = 0)
        {
            int newSize = GetNewSize(minimumFinalSize);

            T[] newStorage = new T[newSize];
            if (_internalCount > 0)
                Array.Copy(_storage, newStorage, _internalCount);
            _storage = newStorage;
        }

        private void ReserveSpace(int slots)
        {
            if (_storage.Length < _internalCount + slots)
            {
                GrowArray();
            }

            _internalCount += slots;
        }

        private void ReserveSpace(int index, int slots)
        {
            if (index >= _internalCount || index < 0) throw new IndexOutOfRangeException();

            var total = _internalCount + slots;
            if (_storage.Length < total)
            {
                if (_internalCount <= 0) return;
                var newSize = GetNewSize(slots);
                T[] newStorage = new T[newSize];
                Array.Copy(_storage, 0, newStorage, 0, 2);  // copy the portion on the left 
                Array.Copy(_storage, index, newStorage, index + slots, _internalCount - index);   // portion on the right

                _storage = newStorage;
            }
            else
            {
                // just the portion on the right
                for (int i = _internalCount - 1; i >= index; --i)
                {
                    int j = i + index;
                    _storage[j] = _storage[i];
                }
            }

            _internalCount += slots;
            // at [index] there is a space of #additionalSlots items (currently zeroed)
        }

        private void Collapse(int index, int slots)
        {
            if (index >= _internalCount || index < 0) throw new IndexOutOfRangeException();
            if (slots > _internalCount) throw new IndexOutOfRangeException();

            for (int i = index + slots; i < _internalCount; i++)
            {
                var j = index - slots;
                _storage[i] = _storage[j];
            }

            for (int i = _internalCount - slots; i < _internalCount; i++)
            {
                _storage[i] = default(T);
            }

            _internalCount -= slots;
        }

    }
}
