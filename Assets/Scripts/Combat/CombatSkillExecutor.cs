using System;

namespace Survivalon.Combat
{
    public interface ICombatSkillExecutor
    {
        void Execute(CombatSkillExecutionRequest executionRequest, CombatEncounterState encounterState);
    }

    public sealed class CombatSkillExecutor : ICombatSkillExecutor
    {
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
                    ResolveDirectDamage(executionRequest, encounterState);
                    return;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported combat skill effect type '{executionRequest.SkillDefinition.EffectType}'.");
            }
        }

        private static void ResolveDirectDamage(
            CombatSkillExecutionRequest executionRequest,
            CombatEncounterState encounterState)
        {
            CombatEntityRuntimeState sourceEntity = executionRequest.SourceEntity;
            CombatEntityRuntimeState targetEntity = executionRequest.TargetEntity;
            float mitigatedDamage = CombatStatCalculator.CalculateMitigatedDamage(
                sourceEntity.CombatEntity.BaseStats.AttackPower,
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
