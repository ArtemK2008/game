using System;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class PersistentSafeResumeState
    {
        [SerializeField]
        private SafeResumeTargetType targetType = SafeResumeTargetType.None;

        [SerializeField]
        private string resumeNodeIdValue = string.Empty;

        public SafeResumeTargetType TargetType => targetType;

        public bool HasSafeResumeTarget =>
            targetType != SafeResumeTargetType.None &&
            !string.IsNullOrWhiteSpace(resumeNodeIdValue);

        public NodeId ResumeNodeId => new NodeId(resumeNodeIdValue);

        public void MarkWorldMap(NodeId nodeId)
        {
            targetType = SafeResumeTargetType.WorldMap;
            resumeNodeIdValue = nodeId.Value;
        }

        public void MarkTownService(NodeId nodeId)
        {
            targetType = SafeResumeTargetType.TownService;
            resumeNodeIdValue = nodeId.Value;
        }

        public void Clear()
        {
            targetType = SafeResumeTargetType.None;
            resumeNodeIdValue = string.Empty;
        }
    }
}

