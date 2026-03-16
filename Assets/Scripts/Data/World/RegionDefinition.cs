using System.Collections.Generic;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.Data.World
{
    [CreateAssetMenu(
        fileName = "RegionDefinition",
        menuName = "Survivalon/Data/World/Region Definition")]
    public sealed class RegionDefinition : ScriptableObject
    {
        [SerializeField]
        private string regionIdValue = string.Empty;

        [SerializeField]
        private int progressionOrder;

        [SerializeField]
        private NodeDefinition entryNode;

        [SerializeField]
        private List<NodeDefinition> nodes = new List<NodeDefinition>();

        [SerializeField]
        private ResourceCategory resourceCategory = ResourceCategory.RegionMaterial;

        [SerializeField]
        private string difficultyBand = string.Empty;

        public RegionId RegionId => new RegionId(regionIdValue);

        public int ProgressionOrder => progressionOrder;

        public NodeDefinition EntryNode => entryNode;

        public IReadOnlyList<NodeDefinition> Nodes => nodes;

        public ResourceCategory ResourceCategory => resourceCategory;

        public string DifficultyBand => difficultyBand;
    }
}

