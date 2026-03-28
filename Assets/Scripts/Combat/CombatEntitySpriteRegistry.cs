using System;
using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Stores runtime-safe references to the current combat entity sprite sets.
    /// </summary>
    public sealed class CombatEntitySpriteRegistry : ScriptableObject
    {
        private const string ResourceName = "CombatEntitySpriteRegistry";

        [SerializeField] private CombatEntitySpriteSet[] spriteSets = Array.Empty<CombatEntitySpriteSet>();

        public static CombatEntitySpriteRegistry LoadOrNull()
        {
            return Resources.Load<CombatEntitySpriteRegistry>(ResourceName);
        }

        public bool TryGetSprite(
            string combatEntityId,
            CombatEntityVisualStateId visualStateId,
            out Sprite sprite)
        {
            if (string.IsNullOrWhiteSpace(combatEntityId))
            {
                sprite = null;
                return false;
            }

            for (int index = 0; index < spriteSets.Length; index++)
            {
                CombatEntitySpriteSet spriteSet = spriteSets[index];
                if (spriteSet == null || !spriteSet.Matches(combatEntityId))
                {
                    continue;
                }

                return spriteSet.TryGetSprite(visualStateId, out sprite);
            }

            sprite = null;
            return false;
        }
    }

    [Serializable]
    public sealed class CombatEntitySpriteSet
    {
        [SerializeField] private string combatEntityId;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite attackSprite;
        [SerializeField] private Sprite hitSprite;
        [SerializeField] private Sprite defeatSprite;

        public bool Matches(string otherCombatEntityId)
        {
            if (string.Equals(combatEntityId, otherCombatEntityId, StringComparison.Ordinal))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(otherCombatEntityId) &&
                otherCombatEntityId.EndsWith($"_{combatEntityId}", StringComparison.Ordinal);
        }

        public bool TryGetSprite(CombatEntityVisualStateId visualStateId, out Sprite sprite)
        {
            switch (visualStateId)
            {
                case CombatEntityVisualStateId.Idle:
                    sprite = idleSprite;
                    return sprite != null;
                case CombatEntityVisualStateId.Attack:
                    sprite = attackSprite;
                    return sprite != null;
                case CombatEntityVisualStateId.Hit:
                    sprite = hitSprite;
                    return sprite != null;
                case CombatEntityVisualStateId.Defeat:
                    sprite = defeatSprite;
                    return sprite != null;
                default:
                    sprite = null;
                    return false;
            }
        }
    }
}
