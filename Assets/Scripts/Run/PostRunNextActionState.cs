using System;

namespace Survivalon.Run
{
    public enum PostRunRecommendedActionKind
    {
        Replay = 0,
        ReturnToWorldPush = 1,
        ReturnToWorldService = 2,
        Stop = 3,
    }

    public enum PostRunReplayReasonKind
    {
        None = 0,
        RetryAttempt = 1,
        ContinueNodeProgress = 2,
        FarmRegionMaterial = 3,
        FarmRewards = 4,
    }

    public enum PostRunForwardOpportunityKind
    {
        None = 0,
        NewlyUnlockedPushTarget = 1,
        AvailablePushTarget = 2,
    }

    public enum PostRunServiceOpportunityKind
    {
        None = 0,
        AffordableProject = 1,
        ReadyRefinement = 2,
        AffordableProjectAndReadyRefinement = 3,
    }

    public sealed class PostRunNextActionState
    {
        public PostRunNextActionState(
            string nodeDisplayName,
            bool canReplayNode,
            bool canReturnToWorld,
            bool canStopSession,
            PostRunRecommendedActionKind recommendedActionKind,
            PostRunReplayReasonKind replayReasonKind,
            string forwardTargetDisplayName = null,
            string serviceHubDisplayName = null,
            PostRunServiceOpportunityKind serviceOpportunityKind = PostRunServiceOpportunityKind.None,
            PostRunForwardOpportunityKind forwardOpportunityKind = PostRunForwardOpportunityKind.None)
        {
            if (string.IsNullOrWhiteSpace(nodeDisplayName))
            {
                throw new ArgumentException("Node display name cannot be null or whitespace.", nameof(nodeDisplayName));
            }

            NodeDisplayName = nodeDisplayName;
            CanReplayNode = canReplayNode;
            CanReturnToWorld = canReturnToWorld;
            CanStopSession = canStopSession;
            RecommendedActionKind = recommendedActionKind;
            ReplayReasonKind = replayReasonKind;
            ForwardTargetDisplayName = forwardTargetDisplayName;
            ServiceHubDisplayName = serviceHubDisplayName;
            ServiceOpportunityKind = serviceOpportunityKind;
            ForwardOpportunityKind = forwardOpportunityKind;
        }

        public string NodeDisplayName { get; }

        public bool CanReplayNode { get; }

        public bool CanReturnToWorld { get; }

        public bool CanStopSession { get; }

        public PostRunRecommendedActionKind RecommendedActionKind { get; }

        public PostRunReplayReasonKind ReplayReasonKind { get; }

        public string ForwardTargetDisplayName { get; }

        public string ServiceHubDisplayName { get; }

        public PostRunServiceOpportunityKind ServiceOpportunityKind { get; }

        public PostRunForwardOpportunityKind ForwardOpportunityKind { get; }

        public bool HasForwardPushOpportunity =>
            ForwardOpportunityKind != PostRunForwardOpportunityKind.None &&
            !string.IsNullOrWhiteSpace(ForwardTargetDisplayName);

        public bool HasServiceOpportunity =>
            !string.IsNullOrWhiteSpace(ServiceHubDisplayName) &&
            ServiceOpportunityKind != PostRunServiceOpportunityKind.None;
    }
}
