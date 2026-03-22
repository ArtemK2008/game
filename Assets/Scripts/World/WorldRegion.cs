using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.World;

namespace Survivalon.World
{
    public sealed class WorldRegion
    {
        private readonly List<NodeId> nodeIds;

        public WorldRegion(
            RegionId regionId,
            int progressionOrder,
            NodeId entryNodeId,
            IEnumerable<NodeId> nodeIds,
            ResourceCategory resourceCategory,
            string difficultyBand,
            LocationIdentityDefinition locationIdentity = null)
        {
            if (progressionOrder < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(progressionOrder), "Progression order must be zero or greater.");
            }

            if (nodeIds == null)
            {
                throw new ArgumentNullException(nameof(nodeIds));
            }

            this.nodeIds = new List<NodeId>(nodeIds);

            if (this.nodeIds.Count == 0)
            {
                throw new ArgumentException("World regions must contain at least one node.", nameof(nodeIds));
            }

            HashSet<NodeId> uniqueNodeIds = new HashSet<NodeId>(this.nodeIds);
            if (uniqueNodeIds.Count != this.nodeIds.Count)
            {
                throw new ArgumentException("World regions cannot contain duplicate node ids.", nameof(nodeIds));
            }

            if (!uniqueNodeIds.Contains(entryNodeId))
            {
                throw new ArgumentException("Entry node must belong to the region node set.", nameof(entryNodeId));
            }

            RegionId = regionId;
            ProgressionOrder = progressionOrder;
            EntryNodeId = entryNodeId;
            ResourceCategory = resourceCategory;
            DifficultyBand = difficultyBand ?? string.Empty;
            LocationIdentity = locationIdentity ?? LocationIdentityCatalog.CreateFallback(regionId);
        }

        public RegionId RegionId { get; }

        public int ProgressionOrder { get; }

        public NodeId EntryNodeId { get; }

        public IReadOnlyList<NodeId> NodeIds => nodeIds;

        public ResourceCategory ResourceCategory { get; }

        public string DifficultyBand { get; }

        public LocationIdentityDefinition LocationIdentity { get; }
    }
}

