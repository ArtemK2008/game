using System;
using Survivalon.Core;

namespace Survivalon.World
{
    /// <summary>
    /// Выводит компактную readable-сводку из authored optional-challenge content.
    /// </summary>
    public static class OptionalChallengePresentationStateResolver
    {
        public static OptionalChallengePresentationState Resolve(WorldNode worldNode)
        {
            if (worldNode == null)
            {
                throw new ArgumentNullException(nameof(worldNode));
            }

            return Resolve(worldNode.NodeType, worldNode.OptionalChallengeContent);
        }

        public static OptionalChallengePresentationState Resolve(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            return Resolve(placeholderState.NodeType, placeholderState.OptionalChallengeContent);
        }

        private static OptionalChallengePresentationState Resolve(
            NodeType nodeType,
            OptionalChallengeContentDefinition optionalChallengeContent)
        {
            if (nodeType != NodeType.Combat || optionalChallengeContent == null)
            {
                return OptionalChallengePresentationState.None;
            }

            return new OptionalChallengePresentationState(
                true,
                optionalChallengeContent.ChallengeDisplayName);
        }
    }
}
