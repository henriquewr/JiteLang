using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JiteLang.Utilities
{
    [DebuggerDisplay("{GetDebuggerDisplay()}")]
    internal class ControllableArray<T> where T : IEquatable<T>
    {
        public ControllableArray(T[] values, T invalidValue)
        {
            InvalidValue = invalidValue;
            Array = values;
            Position = 0;
        }
        public ControllableArray(IEnumerable<T> values, T invalidValue)
        {
            InvalidValue = invalidValue;
            Array = values.ToArray();
            Position = 0;
        }

        protected virtual string GetDebuggerDisplay()
        {
            var at = Array[Position].ToString();
            var display = $"At: {Position}, Value: {at}";
            return display;
        }

        public readonly T InvalidValue;
        public T[] Array { get; init; }
        public int Position { get; protected set; }

        public T Current 
        { 
            get 
            {
                if (CurrentHasValue())
                {
                    return Array[Position];
                }

                return InvalidValue;
            } 
        }

        public bool AdvanceUntil(T item, out ReadOnlySpan<T> value)
        {
            var values = Array.AsSpan(Position);
            var nextIndex = values.IndexOf(item);
            if (nextIndex > -1)
            {
                Position += nextIndex;
                value = values.Slice(0, nextIndex);
                return true;
            }

            value = default(ReadOnlySpan<T>);
            return false;
        }

        public bool AdvanceIf(T expected)
        {
            return AdvanceIf(expected, out _);
        }

        public bool AdvanceIf(T expected, out T peeked)
        {
            peeked = Current;
            if (IsLast())
            {
                return false;
            }

            if (peeked.Equals(expected))
            {
                ++Position;
                return true;
            }

            return false;
        }

        public bool PeekNext(out T value, int pos = 1)
        {
            var desiredPos = Position + pos;

            var notPossible = desiredPos >= Array.Length - 1;

            if (!notPossible)
            {
                value = Array[desiredPos];
                return true;
            }

            value = InvalidValue;
            return false;
        }

        public ReadOnlySpan<T> Slice(int start, int length)
        {
            var slice = Array.AsSpan(start, length);
            return slice;
        }

        public bool Advance()
        {
            return Advance(out _);
        }

        public bool Advance(out T value)
        {
            if (CurrentHasValue())
            {
                value = Array[Position++];
                return true;
            }

            value = InvalidValue;
            return false;
        }

        public bool CurrentHasValue()
        {
            var hasValue = Array.Length > Position;
            return hasValue;
        }

        public bool IsLast()
        {
            var isLast = Position >= Array.Length - 1;
            return isLast;
        }

        public bool HasMore()
        {
            var hasMore = Array.Length - 1 > Position;
            return hasMore;
        }
    }
}
