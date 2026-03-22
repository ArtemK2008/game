using System;
using Survivalon.Run;

namespace Survivalon.World
{
    public static class PostRunNextActionTextBuilder
    {
        public static string Build(PostRunNextActionState nextActionState)
        {
            if (nextActionState == null)
            {
                throw new ArgumentNullException(nameof(nextActionState));
            }

            string text =
                $"Recommended: {BuildRecommendation(nextActionState)}\n" +
                $"Replay: {BuildReplayLine(nextActionState)}\n" +
                $"Return: {BuildReturnLine(nextActionState)}\n" +
                $"Stop: {BuildStopLine(nextActionState)}";

            return text;
        }

        private static string BuildRecommendation(PostRunNextActionState nextActionState)
        {
            switch (nextActionState.RecommendedActionKind)
            {
                case PostRunRecommendedActionKind.Replay:
                    return BuildReplayLine(nextActionState);
                case PostRunRecommendedActionKind.ReturnToWorldPush:
                    return BuildPushRecommendation(nextActionState);
                case PostRunRecommendedActionKind.ReturnToWorldService:
                    return BuildServiceRecommendation(nextActionState);
                case PostRunRecommendedActionKind.Stop:
                    return BuildStopLine(nextActionState);
                default:
                    throw new InvalidOperationException(
                        $"Unknown recommended action kind '{nextActionState.RecommendedActionKind}'.");
            }
        }

        private static string BuildReplayLine(PostRunNextActionState nextActionState)
        {
            if (!nextActionState.CanReplayNode)
            {
                return "Unavailable.";
            }

            switch (nextActionState.ReplayReasonKind)
            {
                case PostRunReplayReasonKind.ContinueNodeProgress:
                    return $"Replay {nextActionState.NodeDisplayName} to keep pushing node progress.";
                case PostRunReplayReasonKind.FarmRegionMaterial:
                    return $"Replay {nextActionState.NodeDisplayName} for more Region material.";
                case PostRunReplayReasonKind.FarmRewards:
                    return $"Replay {nextActionState.NodeDisplayName} for another reward run.";
                case PostRunReplayReasonKind.RetryAttempt:
                    return $"Replay {nextActionState.NodeDisplayName} for another attempt.";
                case PostRunReplayReasonKind.None:
                    return $"Replay {nextActionState.NodeDisplayName} if you want another run here.";
                default:
                    throw new InvalidOperationException(
                        $"Unknown replay reason kind '{nextActionState.ReplayReasonKind}'.");
            }
        }

        private static string BuildReturnLine(PostRunNextActionState nextActionState)
        {
            if (!nextActionState.CanReturnToWorld)
            {
                return "Unavailable.";
            }

            if (nextActionState.HasForwardPushOpportunity && nextActionState.HasServiceOpportunity)
            {
                return
                    $"Return to world, then push to {nextActionState.ForwardTargetDisplayName} " +
                    $"or visit {nextActionState.ServiceHubDisplayName}.";
            }

            if (nextActionState.HasForwardPushOpportunity)
            {
                return $"Return to world, then push to {nextActionState.ForwardTargetDisplayName}.";
            }

            if (nextActionState.HasServiceOpportunity)
            {
                return BuildServiceRecommendation(nextActionState);
            }

            return "Return to world and choose another node.";
        }

        private static string BuildStopLine(PostRunNextActionState nextActionState)
        {
            return nextActionState.CanStopSession
                ? "Safe exit after this resolved run."
                : "Unavailable in this context.";
        }

        private static string BuildPushRecommendation(PostRunNextActionState nextActionState)
        {
            return nextActionState.HasForwardPushOpportunity
                ? $"Return to world, then push to {nextActionState.ForwardTargetDisplayName}."
                : "Return to world and choose the next push path.";
        }

        private static string BuildServiceRecommendation(PostRunNextActionState nextActionState)
        {
            string serviceHubDisplayName = string.IsNullOrWhiteSpace(nextActionState.ServiceHubDisplayName)
                ? "the service hub"
                : nextActionState.ServiceHubDisplayName;

            switch (nextActionState.ServiceOpportunityKind)
            {
                case PostRunServiceOpportunityKind.AffordableProject:
                    return $"Return to world, then visit {serviceHubDisplayName} to spend progression material.";
                case PostRunServiceOpportunityKind.ReadyRefinement:
                    return $"Return to world, then visit {serviceHubDisplayName} to refine Region material.";
                case PostRunServiceOpportunityKind.AffordableProjectAndReadyRefinement:
                    return $"Return to world, then visit {serviceHubDisplayName} to spend or refine.";
                case PostRunServiceOpportunityKind.None:
                    return $"Return to world, then visit {serviceHubDisplayName}.";
                default:
                    throw new InvalidOperationException(
                        $"Unknown service opportunity kind '{nextActionState.ServiceOpportunityKind}'.");
            }
        }
    }
}
