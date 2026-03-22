using System;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PostRunNextActionResolver
    {
        private readonly PostRunReplayOpportunityResolver postRunReplayOpportunityResolver;
        private readonly PostRunForwardOpportunityResolver postRunForwardOpportunityResolver;
        private readonly PostRunServiceOpportunityResolver postRunServiceOpportunityResolver;

        public PostRunNextActionResolver(
            PostRunReplayOpportunityResolver postRunReplayOpportunityResolver = null,
            PostRunForwardOpportunityResolver postRunForwardOpportunityResolver = null,
            PostRunServiceOpportunityResolver postRunServiceOpportunityResolver = null)
        {
            this.postRunReplayOpportunityResolver =
                postRunReplayOpportunityResolver ?? new PostRunReplayOpportunityResolver();
            this.postRunForwardOpportunityResolver =
                postRunForwardOpportunityResolver ?? new PostRunForwardOpportunityResolver();
            this.postRunServiceOpportunityResolver =
                postRunServiceOpportunityResolver ?? new PostRunServiceOpportunityResolver();
        }

        public PostRunNextActionState Resolve(
            PostRunStateController postRunStateController,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState,
            ResourceBalancesState resourceBalancesState,
            PersistentProgressionState persistentProgressionState)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            RunResult runResult = postRunStateController.RunResult;
            PostRunReplayReasonKind replayReasonKind = postRunReplayOpportunityResolver.Resolve(postRunStateController);
            PostRunForwardOpportunityState forwardOpportunityState = postRunForwardOpportunityResolver.Resolve(
                runResult,
                worldGraph,
                persistentWorldState);
            PostRunServiceOpportunityState serviceOpportunityState = postRunServiceOpportunityResolver.Resolve(
                worldGraph,
                persistentWorldState,
                resourceBalancesState,
                persistentProgressionState);

            return new PostRunNextActionState(
                postRunStateController.NodeContext.NodeDisplayName,
                postRunStateController.CanReplayNode,
                postRunStateController.CanReturnToWorld,
                postRunStateController.CanStopSession,
                ResolveRecommendedActionKind(
                    postRunStateController,
                    replayReasonKind,
                    forwardOpportunityState,
                    serviceOpportunityState),
                replayReasonKind,
                forwardOpportunityState.TargetDisplayName,
                serviceOpportunityState.ServiceHubDisplayName,
                serviceOpportunityState.OpportunityKind,
                forwardOpportunityState.OpportunityKind);
        }

        private static PostRunRecommendedActionKind ResolveRecommendedActionKind(
            PostRunStateController postRunStateController,
            PostRunReplayReasonKind replayReasonKind,
            PostRunForwardOpportunityState forwardOpportunityState,
            PostRunServiceOpportunityState serviceOpportunityState)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (postRunStateController.CanReturnToWorld &&
                forwardOpportunityState != null &&
                forwardOpportunityState.OpportunityKind == PostRunForwardOpportunityKind.NewlyUnlockedPushTarget)
            {
                return PostRunRecommendedActionKind.ReturnToWorldPush;
            }

            if (postRunStateController.CanReturnToWorld &&
                serviceOpportunityState != null &&
                serviceOpportunityState.HasOpportunity)
            {
                return PostRunRecommendedActionKind.ReturnToWorldService;
            }

            if (postRunStateController.CanReplayNode &&
                replayReasonKind != PostRunReplayReasonKind.None)
            {
                return PostRunRecommendedActionKind.Replay;
            }

            if (postRunStateController.CanReturnToWorld &&
                forwardOpportunityState != null &&
                forwardOpportunityState.HasOpportunity)
            {
                return PostRunRecommendedActionKind.ReturnToWorldPush;
            }

            return PostRunRecommendedActionKind.Stop;
        }
    }
}
