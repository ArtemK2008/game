using System;
using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Resolves a compact combat-effect overlay state for the current autobattle tick.
    /// </summary>
    public sealed class CombatEffectPresentationStateResolver
    {
        private const float HealthEpsilon = 0.001f;
        private const float TimerResetEpsilon = 0.0001f;
        private const float LowHealthDangerThresholdRatio = 0.35f;

        private CombatEffectSpriteRegistry spriteRegistry;

        public CombatEffectPresentationStateResolver(
            CombatEffectSpriteRegistry spriteRegistry = null)
        {
            this.spriteRegistry = spriteRegistry;
        }

        public CombatEffectPresentationState Resolve(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            if (!previousSnapshot.HasEncounter || !currentSnapshot.HasEncounter)
            {
                return CombatEffectPresentationState.None;
            }

            bool didEnemyTakeDamage = currentSnapshot.EnemyCurrentHealth + HealthEpsilon <
                previousSnapshot.EnemyCurrentHealth;
            bool didPlayerTakeDamage = currentSnapshot.PlayerCurrentHealth + HealthEpsilon <
                previousSnapshot.PlayerCurrentHealth;
            bool shouldShowBurstStrike = didEnemyTakeDamage && HasBurstStrike(previousSnapshot, currentSnapshot) &&
                DidTimerReset(
                    previousSnapshot.PlayerTriggeredActiveSkillTimerSeconds,
                    currentSnapshot.PlayerTriggeredActiveSkillTimerSeconds);
            bool shouldShowPlayerDefeat = previousSnapshot.PlayerIsAlive && !currentSnapshot.PlayerIsAlive;
            bool shouldShowEnemyDefeat = previousSnapshot.EnemyIsAlive && !currentSnapshot.EnemyIsAlive;
            bool shouldShowDangerLowHealth = !shouldShowPlayerDefeat &&
                previousSnapshot.PlayerHealthRatio > LowHealthDangerThresholdRatio &&
                currentSnapshot.PlayerHealthRatio <= LowHealthDangerThresholdRatio &&
                currentSnapshot.PlayerIsAlive;

            Sprite playerEffectSprite = shouldShowPlayerDefeat
                ? ResolveRequiredSprite(CombatEffectVisualId.Defeat)
                : ResolveOptionalSideImpactSprite(didPlayerTakeDamage);
            Sprite enemyEffectSprite = shouldShowEnemyDefeat
                ? ResolveRequiredSprite(CombatEffectVisualId.Defeat)
                : ResolveOptionalSideImpactSprite(didEnemyTakeDamage && !shouldShowBurstStrike);

            Sprite centerEffectSprite = null;
            if (!shouldShowPlayerDefeat && !shouldShowEnemyDefeat)
            {
                if (shouldShowBurstStrike)
                {
                    centerEffectSprite = ResolveRequiredSprite(CombatEffectVisualId.BurstStrike);
                }
                else if (shouldShowDangerLowHealth)
                {
                    centerEffectSprite = ResolveRequiredSprite(CombatEffectVisualId.DangerPulse);
                }
            }

            return new CombatEffectPresentationState(
                playerEffectSprite,
                enemyEffectSprite,
                centerEffectSprite);
        }

        private Sprite ResolveOptionalSideImpactSprite(bool shouldShowImpact)
        {
            return shouldShowImpact
                ? ResolveRequiredSprite(CombatEffectVisualId.BasicImpact)
                : null;
        }

        private static bool HasBurstStrike(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            return string.Equals(
                    previousSnapshot.PlayerTriggeredActiveSkillId,
                    CombatSkillCatalog.BurstStrike.SkillId,
                    StringComparison.Ordinal) ||
                string.Equals(
                    currentSnapshot.PlayerTriggeredActiveSkillId,
                    CombatSkillCatalog.BurstStrike.SkillId,
                    StringComparison.Ordinal);
        }

        private static bool DidTimerReset(float previousTimerSeconds, float currentTimerSeconds)
        {
            if (float.IsPositiveInfinity(previousTimerSeconds) || float.IsPositiveInfinity(currentTimerSeconds))
            {
                return false;
            }

            return currentTimerSeconds > previousTimerSeconds + TimerResetEpsilon;
        }

        private Sprite ResolveRequiredSprite(CombatEffectVisualId effectVisualId)
        {
            CombatEffectSpriteRegistry resolvedSpriteRegistry = EnsureSpriteRegistry();

            if (resolvedSpriteRegistry == null)
            {
                throw new InvalidOperationException(
                    "Combat effect sprite registry asset 'Assets/Resources/CombatEffectSpriteRegistry.asset' is missing.");
            }

            if (!resolvedSpriteRegistry.TryGetSprite(effectVisualId, out Sprite sprite))
            {
                throw new InvalidOperationException(
                    $"No combat effect sprite is configured for effect '{effectVisualId}'.");
            }

            return sprite;
        }

        private CombatEffectSpriteRegistry EnsureSpriteRegistry()
        {
            if (spriteRegistry != null)
            {
                return spriteRegistry;
            }

            spriteRegistry = CombatEffectSpriteRegistry.LoadOrNull();
            return spriteRegistry;
        }
    }

    public readonly struct CombatEffectPresentationState
    {
        public static readonly CombatEffectPresentationState None = new CombatEffectPresentationState(
            playerEffectSprite: null,
            enemyEffectSprite: null,
            centerEffectSprite: null);

        public CombatEffectPresentationState(
            Sprite playerEffectSprite,
            Sprite enemyEffectSprite,
            Sprite centerEffectSprite)
        {
            PlayerEffectSprite = playerEffectSprite;
            EnemyEffectSprite = enemyEffectSprite;
            CenterEffectSprite = centerEffectSprite;
        }

        public Sprite PlayerEffectSprite { get; }

        public Sprite EnemyEffectSprite { get; }

        public Sprite CenterEffectSprite { get; }
    }
}
