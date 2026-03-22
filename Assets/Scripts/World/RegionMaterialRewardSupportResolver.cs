using Survivalon.Core;

namespace Survivalon.World
{
    internal static class RegionMaterialRewardSupportResolver
    {
        public static bool Supports(NodeType nodeType, ResourceCategory regionResourceCategory)
        {
            return nodeType == NodeType.Combat &&
                regionResourceCategory == ResourceCategory.RegionMaterial;
        }
    }
}
