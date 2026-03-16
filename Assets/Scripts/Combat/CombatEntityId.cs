using System;

namespace Survivalon.Runtime.Combat
{
    public readonly struct CombatEntityId : IEquatable<CombatEntityId>
    {
        public CombatEntityId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Combat entity id value cannot be null or whitespace.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        public bool Equals(CombatEntityId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is CombatEntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        public static bool operator ==(CombatEntityId left, CombatEntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CombatEntityId left, CombatEntityId right)
        {
            return !left.Equals(right);
        }
    }
}
