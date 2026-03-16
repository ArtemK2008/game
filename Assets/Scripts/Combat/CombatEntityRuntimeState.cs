using System;

namespace Survivalon.Combat
{
    public sealed class CombatEntityRuntimeState
    {
        public CombatEntityRuntimeState(CombatEntityState combatEntity)
        {
            CombatEntity = combatEntity ?? throw new ArgumentNullException(nameof(combatEntity));

            if (!combatEntity.IsAlive)
            {
                throw new InvalidOperationException("Combat runtime requires an initially living combat entity.");
            }

            if (!combatEntity.IsActive)
            {
                throw new InvalidOperationException("Combat runtime requires an initially active combat entity.");
            }

            CurrentHealth = combatEntity.BaseStats.MaxHealth;
            IsAlive = true;
            IsActive = true;
            TimeUntilNextAttackSeconds = CombatStatCalculator.CalculateAttackIntervalSeconds(combatEntity.BaseStats);
        }

        public CombatEntityState CombatEntity { get; }

        public CombatEntityId EntityId => CombatEntity.EntityId;

        public string DisplayName => CombatEntity.DisplayName;

        public CombatSide Side => CombatEntity.Side;

        public float MaxHealth => CombatEntity.BaseStats.MaxHealth;

        public float CurrentHealth { get; private set; }

        public bool IsAlive { get; private set; }

        public bool IsActive { get; private set; }

        public bool CanAct => IsAlive && IsActive;

        public float TimeUntilNextAttackSeconds { get; private set; }

        public void AdvanceAttackTimer(float elapsedSeconds)
        {
            if (elapsedSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), elapsedSeconds, "Elapsed time cannot be negative.");
            }

            if (!CanAct)
            {
                return;
            }

            TimeUntilNextAttackSeconds = Math.Max(0f, TimeUntilNextAttackSeconds - elapsedSeconds);
        }

        public void ResetAttackTimer()
        {
            TimeUntilNextAttackSeconds = CombatStatCalculator.CalculateAttackIntervalSeconds(CombatEntity.BaseStats);
        }

        public void ApplyDamage(float damage)
        {
            if (damage < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), damage, "Damage cannot be negative.");
            }

            if (!IsAlive || damage <= 0f)
            {
                return;
            }

            CurrentHealth = Math.Max(0f, CurrentHealth - damage);
            if (CurrentHealth <= 0f)
            {
                IsAlive = false;
                IsActive = false;
                TimeUntilNextAttackSeconds = 0f;
            }
        }
    }
}

