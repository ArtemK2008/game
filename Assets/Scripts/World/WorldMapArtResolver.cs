using System;
using Survivalon.Core;
using UnityEngine;

namespace Survivalon.World
{
    /// <summary>
    /// Resolves the current authored world-map background and node-state sprites.
    /// </summary>
    public sealed class WorldMapArtResolver
    {
        private readonly WorldMapArtRegistry artRegistry;

        public WorldMapArtResolver(WorldMapArtRegistry artRegistry = null)
        {
            this.artRegistry = artRegistry ?? WorldMapArtRegistry.LoadOrNull();
        }

        public Sprite ResolveBackgroundSpriteOrNull()
        {
            return artRegistry == null ? null : artRegistry.BackgroundSprite;
        }

        public bool TryResolveNodeSprite(WorldMapNodeOption nodeOption, out Sprite nodeSprite)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            if (artRegistry == null)
            {
                nodeSprite = null;
                return false;
            }

            return artRegistry.TryGetNodeSprite(ResolveNodeArtState(nodeOption), out nodeSprite);
        }

        private static WorldMapNodeArtStateId ResolveNodeArtState(WorldMapNodeOption nodeOption)
        {
            if (nodeOption.IsSelected || nodeOption.IsCurrentContext)
            {
                return WorldMapNodeArtStateId.Current;
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return WorldMapNodeArtStateId.Locked;
            }

            if (nodeOption.NodeState == NodeState.Cleared)
            {
                return WorldMapNodeArtStateId.Cleared;
            }

            return WorldMapNodeArtStateId.Available;
        }
    }
}
