using System;

namespace Survivalon.World
{
    /// <summary>
    /// Описывает authored-маркер для одного опционального challenge-узла.
    /// </summary>
    public sealed class OptionalChallengeContentDefinition
    {
        public OptionalChallengeContentDefinition(string challengeDisplayName)
        {
            if (string.IsNullOrWhiteSpace(challengeDisplayName))
            {
                throw new ArgumentException(
                    "Challenge display name cannot be null or whitespace.",
                    nameof(challengeDisplayName));
            }

            ChallengeDisplayName = challengeDisplayName;
        }

        public string ChallengeDisplayName { get; }
    }
}
