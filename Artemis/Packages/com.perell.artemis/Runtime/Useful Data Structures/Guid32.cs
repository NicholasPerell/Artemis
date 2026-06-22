using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Perell.Artemis.Saving;

namespace Perell.Artemis
{
    /// <summary>
    /// 32-bit rendition of <see cref="System.Guid"/> so it can do easy comparisions with <see cref="float"/>
    /// </summary>
    public readonly struct Guid32 : IComparable<Guid32>, IEquatable<Guid32>, IFormattable
    {
        public static readonly Guid32 Empty;
        private readonly byte _a;
        private readonly byte _b;
        private readonly byte _c;
        private readonly byte _d;

        public Guid32(byte[] b) : this(new ReadOnlySpan<byte>(b ?? throw new ArgumentNullException(nameof(b))))
        {

        }

        public Guid32(ReadOnlySpan<byte> b)
        {
            if (b.Length != 4)
            {
                throw new ArgumentException(nameof(b));
            }

            _a = b[0];
            _b = b[1];
            _c = b[2];
            _d = b[3];
        }

        public Guid32(string g)
        {
            this = Parse(g);
        }

        public Guid32(byte a, byte b, byte c, byte d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public static Guid32 NewGuid()
        {
            var rnd = new RNGCryptoServiceProvider();
            var gen = new byte[4];
            rnd.GetBytes(gen);
            return new(gen);
        }

        public static Guid32 Parse(string g)
        {
            var b = new byte[4];

            if (g == null)
            {
                throw new ArgumentNullException(g);
            }
            else if (Regex.IsMatch(g, "[a-fA-F0-9]{2}-[a-fA-F0-9]{2}-[a-fA-F0-9]{2}-[a-fA-F0-9]{2}"))
            {
                for (int i = 0; i < 4; i++)
                {
                    b[i] = Convert.ToByte(g.Substring(i * 3, 2), 16);
                }
            }
            else if (Regex.IsMatch(g, "[a-fA-F0-9]{8}"))
            {
                for (int i = 0; i < 4; i++)
                {
                    b[i] = Convert.ToByte(g.Substring(i * 2, 2), 16);
                }
            }
            else
            {
                throw new ArgumentException(nameof(g));
            }

            return new(b);
        }

        public static Guid32 Parse(ReadOnlySpan<char> input) => throw new NotImplementedException();
        public static bool TryParse(string input, out Guid32 result) => throw new NotImplementedException();
        public static bool TryParse(ReadOnlySpan<char> input, out Guid32 result) => throw new NotImplementedException();

        public int CompareTo(Guid32 value)
        {
            var comp = _a.CompareTo(value._a);

            if (comp != 0)
            {
                return comp;
            }

            comp = _b.CompareTo(value._b);

            if (comp != 0)
            {
                return comp;
            }

            comp = _c.CompareTo(value._c);

            if (comp != 0)
            {
                return comp;
            }

            return _d.CompareTo(value._d);
        }

        public bool Equals(Guid32 g) => CompareTo(g) == 0;
        public override bool Equals(object o) => o is Guid32 g && Equals(g);

        public override int GetHashCode() => Convert.ToInt32(ToByteArray());

        public byte[] ToByteArray()
            => new byte[] { _a, _b, _c, _d };

        public override string ToString()
            => $"{_a:16}-{_b:16}-{_c:16}-{_d:16}";

        public string ToString(string format) => throw new NotImplementedException();
        public string ToString(string format, IFormatProvider provider) => throw new NotImplementedException();
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default) => throw new NotImplementedException();
        public bool TryWriteBytes(Span<byte> destination)
        {
            if (destination.Length != 4)
            {
                return false;
            }

            destination[0] = _a;
            destination[1] = _b;
            destination[2] = _c;
            destination[3] = _d;

            return true;
        }

        public static bool operator ==(Guid32 a, Guid32 b) => a.Equals(b);
        public static bool operator !=(Guid32 a, Guid32 b) => !(a == b);

        public static implicit operator float(Guid32 guid) => Convert.ToSingle(guid.ToByteArray());
    }
}