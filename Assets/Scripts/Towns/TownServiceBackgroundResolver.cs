using System;
using Survivalon.Data.Towns;
using UnityEngine;

namespace Survivalon.Towns
{
    public sealed class TownServiceBackgroundResolver
    {
        private TownServiceBackgroundRegistry backgroundRegistry;

        public TownServiceBackgroundResolver(TownServiceBackgroundRegistry backgroundRegistry = null)
        {
            this.backgroundRegistry = backgroundRegistry;
        }

        public Sprite Resolve(TownServiceContextDefinition serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            TownServiceBackgroundRegistry resolvedBackgroundRegistry = EnsureBackgroundRegistry();
            if (resolvedBackgroundRegistry == null)
            {
                throw new InvalidOperationException(
                    "Town service background registry asset 'Assets/Resources/TownServiceBackgroundRegistry.asset' is missing.");
            }

            if (!resolvedBackgroundRegistry.TryGetBackground(serviceContext.ContextId, out Sprite backgroundSprite))
            {
                throw new InvalidOperationException(
                    $"No town/service background is configured for context '{serviceContext.ContextId}'.");
            }

            return backgroundSprite;
        }

        private TownServiceBackgroundRegistry EnsureBackgroundRegistry()
        {
            if (backgroundRegistry != null)
            {
                return backgroundRegistry;
            }

            backgroundRegistry = TownServiceBackgroundRegistry.LoadOrNull();
            return backgroundRegistry;
        }
    }
}
