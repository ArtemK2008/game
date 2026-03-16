using System;

namespace Survivalon.Runtime.Core
{
    public readonly struct RegionId : IEquatable<RegionId>
    {
        public RegionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Region id value cannot be null or whitespace.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        public bool Equals(RegionId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is RegionId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        public static bool operator ==(RegionId left, RegionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RegionId left, RegionId right)
        {
            return !left.Equals(right);
        }
    }
}
