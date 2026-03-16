using System;
using Survivalon.Runtime.Run;

namespace Survivalon.Runtime.World
{
    public static class NodePlaceholderScreenStateResolver
    {
        public static NodePlaceholderScreenButtonState ResolveAdvanceButtonState(
            NodePlaceholderState placeholderState,
            RunLifecycleState lifecycleState,
            bool hasCombatEncounterState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            bool usesCombatShell = placeholderState.UsesCombatShell;

            switch (lifecycleState)
            {
                case RunLifecycleState.RunStart:
                    return usesCombatShell
                        ? new NodePlaceholderScreenButtonState("Combat Auto-Starting", false)
                        : new NodePlaceholderScreenButtonState("Start Placeholder Run", true);
                case RunLifecycleState.RunActive:
                    return hasCombatEncounterState
                        ? new NodePlaceholderScreenButtonState("Combat Auto-Running", false)
                        : new NodePlaceholderScreenButtonState("Resolve Placeholder Run", true);
                case RunLifecycleState.RunResolved:
                    return usesCombatShell
                        ? new NodePlaceholderScreenButtonState("Preparing Post-Run", false)
                        : new NodePlaceholderScreenButtonState("Enter Post-Run State", true);
                case RunLifecycleState.PostRun:
                    return new NodePlaceholderScreenButtonState("Run Lifecycle Complete", false);
                default:
                    throw new InvalidOperationException($"Unknown run lifecycle state '{lifecycleState}'.");
            }
        }

        public static bool ShouldShowCombatShell(RunLifecycleState lifecycleState, bool hasCombatEncounterState)
        {
            return hasCombatEncounterState &&
                (lifecycleState == RunLifecycleState.RunActive || lifecycleState == RunLifecycleState.RunResolved);
        }

        public static NodePlaceholderScreenPostRunPanelState ResolvePostRunPanelState(
            RunLifecycleState lifecycleState,
            PostRunStateController postRunStateController,
            bool hasStopSessionHandler)
        {
            bool isVisible = lifecycleState == RunLifecycleState.PostRun && postRunStateController != null;
            if (!isVisible)
            {
                return NodePlaceholderScreenPostRunPanelState.Hidden;
            }

            return new NodePlaceholderScreenPostRunPanelState(
                true,
                new NodePlaceholderScreenButtonState("Replay Node", postRunStateController.CanReplayNode),
                new NodePlaceholderScreenButtonState("Return To World Map", postRunStateController.CanReturnToWorld),
                new NodePlaceholderScreenButtonState(
                    hasStopSessionHandler ? "Stop Session" : "Stop Session Unavailable",
                    postRunStateController.CanStopSession && hasStopSessionHandler));
        }
    }

    public readonly struct NodePlaceholderScreenButtonState
    {
        public NodePlaceholderScreenButtonState(string label, bool isInteractable)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException("Button label cannot be null or whitespace.", nameof(label));
            }

            Label = label;
            IsInteractable = isInteractable;
        }

        public string Label { get; }

        public bool IsInteractable { get; }
    }

    public readonly struct NodePlaceholderScreenPostRunPanelState
    {
        public static readonly NodePlaceholderScreenPostRunPanelState Hidden =
            new NodePlaceholderScreenPostRunPanelState(
                false,
                new NodePlaceholderScreenButtonState("Replay Node", false),
                new NodePlaceholderScreenButtonState("Return To World Map", false),
                new NodePlaceholderScreenButtonState("Stop Session", false));

        public NodePlaceholderScreenPostRunPanelState(
            bool isVisible,
            NodePlaceholderScreenButtonState replayButton,
            NodePlaceholderScreenButtonState returnToWorldButton,
            NodePlaceholderScreenButtonState stopSessionButton)
        {
            IsVisible = isVisible;
            ReplayButton = replayButton;
            ReturnToWorldButton = returnToWorldButton;
            StopSessionButton = stopSessionButton;
        }

        public bool IsVisible { get; }

        public NodePlaceholderScreenButtonState ReplayButton { get; }

        public NodePlaceholderScreenButtonState ReturnToWorldButton { get; }

        public NodePlaceholderScreenButtonState StopSessionButton { get; }
    }
}
