using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Stores runtime-safe references to the current prototype's combat readability effect sprites.
    /// </summary>
    public sealed class CombatEffectSpriteRegistry : ScriptableObject
    {
        private const string ResourceName = "CombatEffectSpriteRegistry";

        [SerializeField] private Sprite basicImpactSprite;
        [SerializeField] private Sprite burstStrikeSprite;
        [SerializeField] private Sprite dangerPulseSprite;
        [SerializeField] private Sprite defeatSprite;

        public static CombatEffectSpriteRegistry LoadOrNull()
        {
            return Resources.Load<CombatEffectSpriteRegistry>(ResourceName);
        }

        public bool TryGetSprite(CombatEffectVisualId effectVisualId, out Sprite sprite)
        {
            switch (effectVisualId)
            {
                case CombatEffectVisualId.BasicImpact:
                    sprite = basicImpactSprite;
                    return sprite != null;
                case CombatEffectVisualId.BurstStrike:
                    sprite = burstStrikeSprite;
                    return sprite != null;
                case CombatEffectVisualId.DangerPulse:
                    sprite = dangerPulseSprite;
                    return sprite != null;
                case CombatEffectVisualId.Defeat:
                    sprite = defeatSprite;
                    return sprite != null;
                default:
                    sprite = null;
                    return false;
            }
        }
    }

    public enum CombatEffectVisualId
    {
        BasicImpact = 0,
        BurstStrike = 1,
        DangerPulse = 2,
        Defeat = 3,
    }
}
