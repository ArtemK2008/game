using System;

namespace Survivalon.World
{
    /// <summary>
    /// Хранит компактную player-facing сводку об опциональном challenge-контенте.
    /// </summary>
    public sealed class OptionalChallengePresentationState
    {
        public static readonly OptionalChallengePresentationState None =
            new OptionalChallengePresentationState(false, string.Empty);

        public OptionalChallengePresentationState(
            bool isOptionalChallenge,
            string challengeDisplayName)
        {
            if (isOptionalChallenge && string.IsNullOrWhiteSpace(challengeDisplayName))
            {
                throw new ArgumentException(
                    "Challenge display name cannot be null or whitespace when optional challenge presentation is active.",
                    nameof(challengeDisplayName));
            }

            if (!isOptionalChallenge && !string.IsNullOrEmpty(challengeDisplayName))
            {
                throw new ArgumentException(
                    "Non-challenge presentation cannot expose a challenge display name.",
                    nameof(challengeDisplayName));
            }

            IsOptionalChallenge = isOptionalChallenge;
            ChallengeDisplayName = challengeDisplayName ?? string.Empty;
        }

        public bool IsOptionalChallenge { get; }

        public string ChallengeDisplayName { get; }
    }
}
