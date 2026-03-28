using System;
using UnityEngine;

namespace Survivalon.Combat
{
    public sealed class CombatShellPresentationStateResolver
    {
        private CombatEntitySpriteRegistry spriteRegistry;

        public CombatShellPresentationStateResolver(CombatEntitySpriteRegistry spriteRegistry = null)
        {
            this.spriteRegistry = spriteRegistry;
        }

        public CombatShellPresentationState Resolve(
            CombatEncounterState combatEncounterState,
            CombatShellVisualState visualState)
        {
            if (combatEncounterState == null)
            {
                throw new ArgumentNullException(nameof(combatEncounterState));
            }

            return new CombatShellPresentationState(
                ResolveRequiredSprite(
                    combatEncounterState.PlayerEntity.EntityId.Value,
                    visualState.PlayerVisualState),
                ResolveRequiredSprite(
                    combatEncounterState.EnemyEntity.EntityId.Value,
                    visualState.EnemyVisualState));
        }

        private Sprite ResolveRequiredSprite(
            string combatEntityId,
            CombatEntityVisualStateId visualStateId)
        {
            CombatEntitySpriteRegistry resolvedSpriteRegistry = EnsureSpriteRegistry();

            if (resolvedSpriteRegistry == null)
            {
                throw new InvalidOperationException(
                    "Combat entity sprite registry asset 'Assets/Resources/CombatEntitySpriteRegistry.asset' is missing.");
            }

            if (!resolvedSpriteRegistry.TryGetSprite(combatEntityId, visualStateId, out Sprite sprite))
            {
                throw new InvalidOperationException(
                    $"No combat sprite is configured for entity '{combatEntityId}' in state '{visualStateId}'.");
            }

            return sprite;
        }

        private CombatEntitySpriteRegistry EnsureSpriteRegistry()
        {
            if (spriteRegistry != null)
            {
                return spriteRegistry;
            }

            spriteRegistry = CombatEntitySpriteRegistry.LoadOrNull();
            return spriteRegistry;
        }
    }

    public readonly struct CombatShellPresentationState
    {
        public CombatShellPresentationState(Sprite playerSprite, Sprite enemySprite)
        {
            PlayerSprite = playerSprite ? playerSprite : throw new ArgumentNullException(nameof(playerSprite));
            EnemySprite = enemySprite ? enemySprite : throw new ArgumentNullException(nameof(enemySprite));
        }

        public Sprite PlayerSprite { get; }

        public Sprite EnemySprite { get; }
    }
}
