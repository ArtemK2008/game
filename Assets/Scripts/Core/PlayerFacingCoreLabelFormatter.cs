using System;

namespace Survivalon.Core
{
    /// <summary>
    /// Форматирует общие player-facing labels для core enum и category значений.
    /// </summary>
    public static class PlayerFacingCoreLabelFormatter
    {
        public static string FormatResourceCategory(ResourceCategory resourceCategory)
        {
            switch (resourceCategory)
            {
                case ResourceCategory.SoftCurrency:
                    return "Soft currency";
                case ResourceCategory.RegionMaterial:
                    return "Region material";
                case ResourceCategory.PersistentProgressionMaterial:
                    return "Persistent progression material";
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(resourceCategory),
                        resourceCategory,
                        "Unknown resource category.");
            }
        }

        public static string FormatNodeType(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Combat:
                    return "Combat";
                case NodeType.BossOrGate:
                    return "Boss gate";
                case NodeType.ServiceOrProgression:
                    return "Service hub";
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, "Unknown node type.");
            }
        }

        public static string FormatNodeState(NodeState nodeState)
        {
            switch (nodeState)
            {
                case NodeState.Available:
                    return "Available";
                case NodeState.InProgress:
                    return "In progress";
                case NodeState.Cleared:
                    return "Cleared";
                case NodeState.Locked:
                    return "Locked";
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeState), nodeState, "Unknown node state.");
            }
        }
    }
}
