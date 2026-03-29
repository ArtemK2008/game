using System;
using Survivalon.Core;
using UnityEngine;

namespace Survivalon.World
{
    /// <summary>
    /// Resolves the current authored world-map background and meaning-first node icon sprites.
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

            return artRegistry.TryGetNodeSprite(ResolveNodeIconKind(nodeOption), out nodeSprite);
        }

        public WorldMapNodeIconKind ResolveNodeIconKind(WorldMapNodeOption nodeOption)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return WorldMapNodeIconKind.Locked;
            }

            if (nodeOption.NodeType == NodeType.ServiceOrProgression)
            {
                return WorldMapNodeIconKind.Service;
            }

            if (nodeOption.NodeType == NodeType.BossOrGate)
            {
                return WorldMapNodeIconKind.BossGate;
            }

            if (!string.IsNullOrWhiteSpace(nodeOption.OptionalChallengeDisplayName))
            {
                return WorldMapNodeIconKind.Elite;
            }

            if (nodeOption.HasRegionMaterialYieldContent)
            {
                return WorldMapNodeIconKind.Farm;
            }

            return WorldMapNodeIconKind.OrdinaryCombat;
        }
    }
}
