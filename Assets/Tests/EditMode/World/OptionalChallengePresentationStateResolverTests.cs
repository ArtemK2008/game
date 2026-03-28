using NUnit.Framework;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    /// <summary>
    /// Проверяет компактную readable-сводку для опционального challenge-контента.
    /// </summary>
    public sealed class OptionalChallengePresentationStateResolverTests
    {
        [Test]
        public void ShouldReturnNoneForOrdinaryCombatNodeWithoutChallengeContent()
        {
            OptionalChallengePresentationState presentationState =
                OptionalChallengePresentationStateResolver.Resolve(
                    NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(presentationState.IsOptionalChallenge, Is.False);
            Assert.That(presentationState.ChallengeDisplayName, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ShouldResolveEliteChallengeLabelForOptionalChallengeCombatNode()
        {
            OptionalChallengePresentationState presentationState =
                OptionalChallengePresentationStateResolver.Resolve(
                    NodePlaceholderTestData.CreateEliteChallengePlaceholderState());

            Assert.That(presentationState.IsOptionalChallenge, Is.True);
            Assert.That(presentationState.ChallengeDisplayName, Is.EqualTo("Elite challenge"));
        }

        [Test]
        public void ShouldReturnNoneForServiceNode()
        {
            WorldNode worldNode = new WorldNode(
                new NodeId("node_service"),
                new RegionId("region_service"),
                NodeType.ServiceOrProgression,
                NodeState.Available);

            OptionalChallengePresentationState presentationState =
                OptionalChallengePresentationStateResolver.Resolve(worldNode);

            Assert.That(presentationState.IsOptionalChallenge, Is.False);
            Assert.That(presentationState.ChallengeDisplayName, Is.EqualTo(string.Empty));
        }
    }
}
