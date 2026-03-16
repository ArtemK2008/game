using System;
using UnityEngine;

namespace Survivalon.Runtime.Combat
{
    public static class CombatShellStateResolver
    {
        public static Color ResolveEntityCardColor(CombatSide side)
        {
            switch (side)
            {
                case CombatSide.Player:
                    return new Color(0.18f, 0.38f, 0.68f, 1f);
                case CombatSide.Enemy:
                    return new Color(0.62f, 0.22f, 0.22f, 1f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, "Combat shell only supports player and enemy card colors.");
            }
        }
    }
}
