using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Raf.Collections
{
    public partial class XList<T> : IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

    public partial class XList<T> //: IEnumerable<T>
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return CanonicalEnumerator();
        }

        private IEnumerator<T> CanonicalEnumerator()
        {
            for (int i = 0; i < Span.Length; i++)
            {
                yield return Span[i];
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(_storage.AsSpan(0, _internalCount));
    }


    public partial class XList<T>
    {
        public ref struct Enumerator
        {
            /// <summary>
            /// The span being enumerated
            /// </summary>
            private readonly ReadOnlySpan<T> _span;

            /// <summary>
            /// The next index to yield
            /// </summary>
            private int _index;

            /// <summary>
            /// Initialize the enumerator
            /// </summary>
            /// <param name="span">The span to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlySpan<T> span)
            {
                _span = span;
                _index = -1;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the span
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _index + 1;
                if (index < _span.Length)
                {
                    _index = index;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Gets the reference to the element at the current position of the enumerator
            /// </summary>
            public ref readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _span[_index];
            }
        }
    }
}
