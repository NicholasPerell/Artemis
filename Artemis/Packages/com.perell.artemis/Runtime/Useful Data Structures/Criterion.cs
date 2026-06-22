using System;
using UnityEngine;

namespace Perell.Artemis
{
    [Serializable]
    public struct Criterion : IComparable<Criterion>, IEquatable<Criterion>
    {
        [SerializeField]
        private Guid32 _key;
        public Guid32 Key => _key;

        [SerializeField]
        float _a;

        [SerializeField]
        float _b;

        private Criterion(Guid32 key, float a, float b)
        {
            _key = key;
            _a = a;
            _b = b;
        }

        public bool Matches(float x) => _a <= x && x <= _b;

        public static Criterion Equals(Guid32 key, float value) => new(key, value, value);

        public static Criterion Equals(Guid32 key, bool value) => new(key, value ? 1 : 0, value ? 1 : 0);

        public static Criterion LessThan(Guid32 key, float value) => new(key, float.NegativeInfinity, BitDecrement(value));

        public static Criterion LessThanOrEquals(Guid32 key, float value) => new(key, float.NegativeInfinity, value);

        public static Criterion GreaterThan(Guid32 key, float value) => new(key, BitIncrement(value), float.PositiveInfinity);

        public static Criterion GreaterThanOrEquals(Guid32 key, float value) => new(key, value, float.PositiveInfinity);

        public static Criterion RangeOpen(Guid32 key, float a, float b) => new(key, BitIncrement(a), BitDecrement(b));

        public static Criterion RangeClosed(Guid32 key, float a, float b) => new(key, a, b);

        public static Criterion RangeLeftOpen(Guid32 key, float a, float b) => new(key, BitIncrement(a), b);

        public static Criterion RangeRightOpen(Guid32 key, float a, float b) => new(key, a, BitDecrement(b));

        private static float BitIncrement(float f)
        {
            if (float.IsNaN(f) || float.IsPositiveInfinity(f))
                return f;

            if (float.IsNegativeInfinity(f))
                return float.MinValue;

            int bits = BitConverter.SingleToInt32Bits(f);

            if ((bits & 0x7FFFFFFF) == 0)
                return float.Epsilon;

            return BitConverter.Int32BitsToSingle(
                f > 0 ? bits + 1 : bits - 1
            );
        }

        private static float BitDecrement(float f)
        {
            if (float.IsNaN(f) || float.IsNegativeInfinity(f))
                return f;

            if (float.IsPositiveInfinity(f))
                return float.MaxValue;

            int bits = BitConverter.SingleToInt32Bits(f);

            if ((bits & 0x7FFFFFFF) == 0)
                return -float.Epsilon;

            return BitConverter.Int32BitsToSingle(
                f > 0 ? bits - 1 : bits + 1
            );
        }

        public int CompareTo(Criterion other)
        {
            return _key.CompareTo(other._key);
        }

        public bool Equals(Criterion other)
        {
            return CompareTo(other) == 0;
        }
    }
}
