using System;

namespace Survivalon.Runtime
{
    public sealed class CombatShellParticipant
    {
        public CombatShellParticipant(string displayName, CombatSide side)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            DisplayName = displayName;
            Side = side;
        }

        public string DisplayName { get; }

        public CombatSide Side { get; }
    }
}
