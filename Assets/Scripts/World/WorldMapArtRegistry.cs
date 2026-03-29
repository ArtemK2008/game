using System;
using UnityEngine;

namespace Survivalon.World
{
    /// <summary>
    /// Stores runtime-safe references to the current authored world-map presentation art.
    /// </summary>
    public sealed class WorldMapArtRegistry : ScriptableObject
    {
        private const string ResourceName = "WorldMapArtRegistry";

        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite lockedNodeSprite;
        [SerializeField] private Sprite availableNodeSprite;
        [SerializeField] private Sprite currentNodeSprite;
        [SerializeField] private Sprite clearedNodeSprite;

        public Sprite BackgroundSprite => backgroundSprite;

        public static WorldMapArtRegistry LoadOrNull()
        {
            return Resources.Load<WorldMapArtRegistry>(ResourceName);
        }

        public bool TryGetNodeSprite(WorldMapNodeArtStateId artStateId, out Sprite nodeSprite)
        {
            switch (artStateId)
            {
                case WorldMapNodeArtStateId.Locked:
                    nodeSprite = lockedNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeArtStateId.Available:
                    nodeSprite = availableNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeArtStateId.Current:
                    nodeSprite = currentNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeArtStateId.Cleared:
                    nodeSprite = clearedNodeSprite;
                    return nodeSprite != null;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(artStateId),
                        artStateId,
                        "Unknown world-map node art state.");
            }
        }
    }

    public enum WorldMapNodeArtStateId
    {
        Locked = 0,
        Available = 1,
        Current = 2,
        Cleared = 3,
    }
}
