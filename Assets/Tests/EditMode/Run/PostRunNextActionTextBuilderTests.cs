using NUnit.Framework;
using Survivalon.Run;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunNextActionTextBuilderTests
    {
        [Test]
        public void ShouldBuildReadableReplayGuidance()
        {
            PostRunNextActionState nextActionState = new PostRunNextActionState(
                "Raider Trail",
                canReplayNode: true,
                canReturnToWorld: true,
                canStopSession: true,
                PostRunRecommendedActionKind.Replay,
                PostRunReplayReasonKind.ContinueNodeProgress);

            string text = PostRunNextActionTextBuilder.Build(nextActionState);

            Assert.That(text, Does.Contain("Best next step: Replay Raider Trail to keep pushing node progress."));
            Assert.That(text, Does.Contain("Replay here: Replay Raider Trail to keep pushing node progress."));
            Assert.That(text, Does.Contain("World map: Return to the world map and choose another node."));
            Assert.That(text, Does.Contain("End session: Safe to exit after this resolved run."));
        }

        [Test]
        public void ShouldBuildReadablePushAndServiceGuidanceWithFriendlyNames()
        {
            PostRunNextActionState nextActionState = new PostRunNextActionState(
                "Frontier Gate",
                canReplayNode: true,
                canReturnToWorld: true,
                canStopSession: true,
                PostRunRecommendedActionKind.ReturnToWorldPush,
                PostRunReplayReasonKind.FarmRewards,
                forwardTargetDisplayName: "Cavern Gate",
                serviceHubDisplayName: "Cavern Service Hub",
                serviceOpportunityKind: PostRunServiceOpportunityKind.AffordableProject,
                forwardOpportunityKind: PostRunForwardOpportunityKind.NewlyUnlockedPushTarget);

            string text = PostRunNextActionTextBuilder.Build(nextActionState);

            Assert.That(text, Does.Contain("Best next step: Return to the world map, then push to Cavern Gate."));
            Assert.That(text, Does.Contain("World map: Return to the world map, then push to Cavern Gate or visit Cavern Service Hub."));
            Assert.That(text, Does.Not.Contain("region_002_node_002"));
        }

        [Test]
        public void ShouldBuildReadableServiceRecommendationForRefinement()
        {
            PostRunNextActionState nextActionState = new PostRunNextActionState(
                "Forest Farm",
                canReplayNode: true,
                canReturnToWorld: true,
                canStopSession: true,
                PostRunRecommendedActionKind.ReturnToWorldService,
                PostRunReplayReasonKind.FarmRegionMaterial,
                serviceHubDisplayName: "Cavern Service Hub",
                serviceOpportunityKind: PostRunServiceOpportunityKind.ReadyRefinement);

            string text = PostRunNextActionTextBuilder.Build(nextActionState);

            Assert.That(text, Does.Contain("Best next step: Return to the world map, then visit Cavern Service Hub to refine Region material."));
            Assert.That(text, Does.Contain("Replay here: Replay Forest Farm for more Region material."));
        }
    }
}
