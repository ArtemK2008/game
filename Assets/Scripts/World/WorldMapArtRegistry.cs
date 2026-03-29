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
        [SerializeField] private Sprite ordinaryCombatNodeSprite;
        [SerializeField] private Sprite farmNodeSprite;
        [SerializeField] private Sprite eliteNodeSprite;
        [SerializeField] private Sprite bossGateNodeSprite;
        [SerializeField] private Sprite serviceNodeSprite;
        [SerializeField] private Sprite lockedNodeSprite;
        [SerializeField] private Sprite currentNodeSprite;
        [SerializeField] private Sprite regionTransitionNodeSprite;

        public Sprite BackgroundSprite => backgroundSprite;

        public static WorldMapArtRegistry LoadOrNull()
        {
            return Resources.Load<WorldMapArtRegistry>(ResourceName);
        }

        public bool TryGetNodeSprite(WorldMapNodeIconKind iconKind, out Sprite nodeSprite)
        {
            switch (iconKind)
            {
                case WorldMapNodeIconKind.OrdinaryCombat:
                    nodeSprite = ordinaryCombatNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.Farm:
                    nodeSprite = farmNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.Elite:
                    nodeSprite = eliteNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.BossGate:
                    nodeSprite = bossGateNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.Service:
                    nodeSprite = serviceNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.Locked:
                    nodeSprite = lockedNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.Current:
                    nodeSprite = currentNodeSprite;
                    return nodeSprite != null;
                case WorldMapNodeIconKind.RegionTransition:
                    nodeSprite = regionTransitionNodeSprite;
                    return nodeSprite != null;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(iconKind),
                        iconKind,
                        "Unknown world-map node icon kind.");
            }
        }
    }

    public enum WorldMapNodeIconKind
    {
        OrdinaryCombat = 0,
        Farm = 1,
        Elite = 2,
        BossGate = 3,
        Service = 4,
        Locked = 5,
        Current = 6,
        RegionTransition = 7,
    }
}
