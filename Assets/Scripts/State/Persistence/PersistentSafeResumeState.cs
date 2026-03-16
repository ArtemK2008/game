using System;
using UnityEngine;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
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

        public void Clear()
        {
            targetType = SafeResumeTargetType.None;
            resumeNodeIdValue = string.Empty;
        }
    }
}
