using System;
using UnityEngine;
using Survivalon.Data.World;

namespace Survivalon.Combat
{
    public sealed class CombatLocationBackgroundResolver
    {
        private CombatLocationBackgroundRegistry backgroundRegistry;

        public CombatLocationBackgroundResolver(CombatLocationBackgroundRegistry backgroundRegistry = null)
        {
            this.backgroundRegistry = backgroundRegistry;
        }

        public Sprite Resolve(LocationIdentityDefinition locationIdentity)
        {
            if (locationIdentity == null)
            {
                throw new ArgumentNullException(nameof(locationIdentity));
            }

            CombatLocationBackgroundRegistry resolvedBackgroundRegistry = EnsureBackgroundRegistry();
            if (resolvedBackgroundRegistry == null)
            {
                throw new InvalidOperationException(
                    "Combat location background registry asset 'Assets/Resources/CombatLocationBackgroundRegistry.asset' is missing.");
            }

            if (!resolvedBackgroundRegistry.TryGetBackground(locationIdentity.LocationIdentityId, out Sprite backgroundSprite))
            {
                throw new InvalidOperationException(
                    $"No combat background is configured for location identity '{locationIdentity.LocationIdentityId}'.");
            }

            return backgroundSprite;
        }

        private CombatLocationBackgroundRegistry EnsureBackgroundRegistry()
        {
            if (backgroundRegistry != null)
            {
                return backgroundRegistry;
            }

            backgroundRegistry = CombatLocationBackgroundRegistry.LoadOrNull();
            return backgroundRegistry;
        }
    }
}
