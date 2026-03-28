using System;
using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Resolves a compact combat-effect overlay state for the current autobattle tick.
    /// </summary>
    public sealed class CombatEffectPresentationStateResolver
    {
        private CombatEffectSpriteRegistry spriteRegistry;
        private readonly CombatFeedbackSoundStateResolver feedbackSoundStateResolver;

        public CombatEffectPresentationStateResolver(
            CombatEffectSpriteRegistry spriteRegistry = null,
            CombatFeedbackSoundStateResolver feedbackSoundStateResolver = null)
        {
            this.spriteRegistry = spriteRegistry;
            this.feedbackSoundStateResolver = feedbackSoundStateResolver ?? new CombatFeedbackSoundStateResolver();
        }

        public CombatEffectPresentationState Resolve(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            if (!previousSnapshot.HasEncounter || !currentSnapshot.HasEncounter)
            {
                return CombatEffectPresentationState.None;
            }

            CombatFeedbackSoundState feedbackSoundState = feedbackSoundStateResolver.Resolve(
                previousSnapshot,
                currentSnapshot);

            bool shouldShowPlayerDefeat = feedbackSoundState.ShouldPlayPlayerDefeat;
            bool shouldShowEnemyDefeat = feedbackSoundState.ShouldPlayEnemyDefeat;

            Sprite playerEffectSprite = shouldShowPlayerDefeat
                ? ResolveRequiredSprite(CombatEffectVisualId.Defeat)
                : ResolveOptionalSideImpactSprite(
                    feedbackSoundState.ShouldPlayEnemyAttack || feedbackSoundState.ShouldPlayPlayerHit);
            Sprite enemyEffectSprite = shouldShowEnemyDefeat
                ? ResolveRequiredSprite(CombatEffectVisualId.Defeat)
                : ResolveOptionalSideImpactSprite(
                    feedbackSoundState.ShouldPlayPlayerAttack || feedbackSoundState.ShouldPlayEnemyHit);

            Sprite centerEffectSprite = null;
            if (!shouldShowPlayerDefeat && !shouldShowEnemyDefeat)
            {
                if (feedbackSoundState.ShouldPlayBurstStrike)
                {
                    centerEffectSprite = ResolveRequiredSprite(CombatEffectVisualId.BurstStrike);
                }
                else if (feedbackSoundState.ShouldPlayDangerLowHealth)
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
