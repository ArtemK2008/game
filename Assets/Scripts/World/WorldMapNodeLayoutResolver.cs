using System;
using System.Collections.Generic;
using Survivalon.Core;
using UnityEngine;

namespace Survivalon.World
{
    /// <summary>
    /// Keeps authored world-map node placement separate from the screen view.
    /// </summary>
    public sealed class WorldMapNodeLayoutResolver
    {
        private static readonly Vector2 AuthoredMapReferenceSize = new Vector2(1672f, 940f);

        private static readonly IReadOnlyDictionary<string, Vector2> AuthoredNormalizedPositionsByNodeId =
            new Dictionary<string, Vector2>(StringComparer.Ordinal)
            {
                { BootstrapWorldScenario.ForestEntryNodeId.Value, new Vector2(0.10f, 0.60f) },
                { BootstrapWorldScenario.ForestPushNodeId.Value, new Vector2(0.22f, 0.54f) },
                { BootstrapWorldScenario.ForestFarmNodeId.Value, new Vector2(0.17f, 0.78f) },
                { BootstrapWorldScenario.ForestEliteNodeId.Value, new Vector2(0.30f, 0.32f) },
                { BootstrapWorldScenario.ForestGateNodeId.Value, new Vector2(0.35f, 0.51f) },
                { BootstrapWorldScenario.CavernServiceNodeId.Value, new Vector2(0.49f, 0.66f) },
                { BootstrapWorldScenario.CavernPushNodeId.Value, new Vector2(0.60f, 0.57f) },
                { BootstrapWorldScenario.CavernFarmNodeId.Value, new Vector2(0.55f, 0.80f) },
                { BootstrapWorldScenario.CavernApproachNodeId.Value, new Vector2(0.70f, 0.45f) },
                { BootstrapWorldScenario.CavernGateNodeId.Value, new Vector2(0.78f, 0.50f) },
                { BootstrapWorldScenario.SunscorchEntryNodeId.Value, new Vector2(0.88f, 0.58f) },
                { BootstrapWorldScenario.SunscorchPushNodeId.Value, new Vector2(0.94f, 0.36f) },
                { BootstrapWorldScenario.SunscorchFarmNodeId.Value, new Vector2(0.88f, 0.79f) },
            };

        public WorldMapSurfaceLayout Resolve(IReadOnlyList<WorldMapNodeOption> nodeOptions)
        {
            if (nodeOptions == null)
            {
                throw new ArgumentNullException(nameof(nodeOptions));
            }

            Dictionary<NodeId, Vector2> authoredPositions = new Dictionary<NodeId, Vector2>();
            for (int index = 0; index < nodeOptions.Count; index++)
            {
                WorldMapNodeOption nodeOption = nodeOptions[index];
                if (!AuthoredNormalizedPositionsByNodeId.TryGetValue(nodeOption.NodeId.Value, out Vector2 position))
                {
                    return CreateFallbackLayout(nodeOptions);
                }

                authoredPositions[nodeOption.NodeId] = position;
            }

            return new WorldMapSurfaceLayout(
                usesNormalizedPositions: true,
                referenceSize: AuthoredMapReferenceSize,
                nodePositions: authoredPositions);
        }

        private static WorldMapSurfaceLayout CreateFallbackLayout(IReadOnlyList<WorldMapNodeOption> nodeOptions)
        {
            Dictionary<NodeId, Vector2> fallbackPositions = new Dictionary<NodeId, Vector2>();
            const float initialOffsetY = 96f;
            const float nodeSpacingY = 136f;

            for (int index = 0; index < nodeOptions.Count; index++)
            {
                fallbackPositions[nodeOptions[index].NodeId] = new Vector2(0f, initialOffsetY + index * nodeSpacingY);
            }

            float contentHeight = Mathf.Max(720f, initialOffsetY + nodeOptions.Count * nodeSpacingY);
            return new WorldMapSurfaceLayout(
                usesNormalizedPositions: false,
                referenceSize: new Vector2(0f, contentHeight),
                nodePositions: fallbackPositions);
        }
    }

    public readonly struct WorldMapSurfaceLayout
    {
        public WorldMapSurfaceLayout(
            bool usesNormalizedPositions,
            Vector2 referenceSize,
            IReadOnlyDictionary<NodeId, Vector2> nodePositions)
        {
            UsesNormalizedPositions = usesNormalizedPositions;
            ReferenceSize = referenceSize;
            NodePositions = nodePositions ?? throw new ArgumentNullException(nameof(nodePositions));
        }

        public bool UsesNormalizedPositions { get; }

        public Vector2 ReferenceSize { get; }

        public IReadOnlyDictionary<NodeId, Vector2> NodePositions { get; }
    }
}
