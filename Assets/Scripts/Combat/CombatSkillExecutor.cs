using System;

namespace Survivalon.Combat
{
    public interface ICombatSkillExecutor
    {
        void Execute(CombatSkillExecutionRequest executionRequest, CombatEncounterState encounterState);
    }

    public sealed class CombatSkillExecutor : ICombatSkillExecutor
    {
        private readonly CombatPassiveSkillEffectResolver combatPassiveSkillEffectResolver;
        private readonly CombatDirectDamageSkillEffectResolver combatDirectDamageSkillEffectResolver;

        public CombatSkillExecutor(
            CombatPassiveSkillEffectResolver combatPassiveSkillEffectResolver = null,
            CombatDirectDamageSkillEffectResolver combatDirectDamageSkillEffectResolver = null)
        {
            this.combatPassiveSkillEffectResolver =
                combatPassiveSkillEffectResolver ?? new CombatPassiveSkillEffectResolver();
            this.combatDirectDamageSkillEffectResolver =
                combatDirectDamageSkillEffectResolver ?? new CombatDirectDamageSkillEffectResolver();
        }

        public void Execute(CombatSkillExecutionRequest executionRequest, CombatEncounterState encounterState)
        {
            if (executionRequest == null)
            {
                throw new ArgumentNullException(nameof(executionRequest));
            }

            if (encounterState == null)
            {
                throw new ArgumentNullException(nameof(encounterState));
            }

            CombatEntityRuntimeState sourceEntity = executionRequest.SourceEntity;
            CombatEntityRuntimeState targetEntity = executionRequest.TargetEntity;

            if (!sourceEntity.CanAct || !targetEntity.IsAlive)
            {
                return;
            }

            switch (executionRequest.SkillDefinition.EffectType)
            {
                case CombatSkillEffectType.DirectDamage:
                    ResolveDirectDamage(
                        executionRequest,
                        encounterState,
                        combatPassiveSkillEffectResolver,
                        combatDirectDamageSkillEffectResolver);
                    return;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported combat skill effect type '{executionRequest.SkillDefinition.EffectType}'.");
            }
        }

        private static void ResolveDirectDamage(
            CombatSkillExecutionRequest executionRequest,
            CombatEncounterState encounterState,
            CombatPassiveSkillEffectResolver combatPassiveSkillEffectResolver,
            CombatDirectDamageSkillEffectResolver combatDirectDamageSkillEffectResolver)
        {
            CombatEntityRuntimeState sourceEntity = executionRequest.SourceEntity;
            CombatEntityRuntimeState targetEntity = executionRequest.TargetEntity;
            float skillAttackPowerMultiplier = combatDirectDamageSkillEffectResolver.ResolveAttackPowerMultiplier(
                executionRequest.SkillDefinition);
            float directDamageMultiplier = combatPassiveSkillEffectResolver.ResolveOutgoingDirectDamageMultiplier(
                sourceEntity,
                executionRequest.SkillDefinition);
            float mitigatedDamage = CombatStatCalculator.CalculateMitigatedDamage(
                sourceEntity.CombatEntity.BaseStats.AttackPower * skillAttackPowerMultiplier * directDamageMultiplier,
                targetEntity.CombatEntity.BaseStats);

            targetEntity.ApplyDamage(mitigatedDamage);

            if (!targetEntity.IsAlive)
            {
                encounterState.Resolve(
                    sourceEntity.Side == CombatSide.Player
                        ? CombatEncounterOutcome.PlayerVictory
                        : CombatEncounterOutcome.EnemyVictory);
            }
        }
    }
}
