using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Runtime.Run;

namespace Survivalon.Runtime.Combat
{
    public readonly struct CombatStatBlock : IEquatable<CombatStatBlock>
    {
        public CombatStatBlock(
            float maxHealth,
            float attackPower,
            float attackRate,
            float defense)
        {
            if (maxHealth <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), maxHealth, "Max health must be greater than zero.");
            }

            if (attackPower < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(attackPower), attackPower, "Attack power cannot be negative.");
            }

            if (attackRate <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(attackRate), attackRate, "Attack rate must be greater than zero.");
            }

            if (defense < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(defense), defense, "Defense cannot be negative.");
            }

            MaxHealth = maxHealth;
            AttackPower = attackPower;
            AttackRate = attackRate;
            Defense = defense;
        }

        public float MaxHealth { get; }

        public float AttackPower { get; }

        public float AttackRate { get; }

        public float Defense { get; }

        public bool Equals(CombatStatBlock other)
        {
            return MaxHealth.Equals(other.MaxHealth) &&
                AttackPower.Equals(other.AttackPower) &&
                AttackRate.Equals(other.AttackRate) &&
                Defense.Equals(other.Defense);
        }

        public override bool Equals(object obj)
        {
            return obj is CombatStatBlock other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaxHealth, AttackPower, AttackRate, Defense);
        }

        public static bool operator ==(CombatStatBlock left, CombatStatBlock right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CombatStatBlock left, CombatStatBlock right)
        {
            return !left.Equals(right);
        }
    }
}
