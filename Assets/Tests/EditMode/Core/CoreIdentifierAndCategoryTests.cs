using System;
using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    public sealed class CoreIdentifierAndCategoryTests
    {
        [Test]
        public void ShouldCreateNodeIdFromNonEmptyValue()
        {
            NodeId nodeId = new NodeId("region_001_node_001");

            Assert.That(nodeId.Value, Is.EqualTo("region_001_node_001"));
            Assert.That(nodeId.ToString(), Is.EqualTo("region_001_node_001"));
        }

        [Test]
        public void ShouldCompareRegionIdsByValue()
        {
            RegionId left = new RegionId("region_001");
            RegionId right = new RegionId("region_001");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void ShouldRejectWhitespaceNodeId()
        {
            TestDelegate action = () => new NodeId(" ");

            Assert.That(action, Throws.ArgumentException);
        }

        [Test]
        public void ShouldRejectWhitespaceRegionId()
        {
            TestDelegate action = () => new RegionId(" ");

            Assert.That(action, Throws.ArgumentException);
        }

        [Test]
        public void ShouldExposeMinimumWorldCompatibleNodeTypes()
        {
            NodeType[] nodeTypes = (NodeType[])Enum.GetValues(typeof(NodeType));

            CollectionAssert.IsSupersetOf(
                nodeTypes,
                new[]
                {
                    NodeType.Combat,
                    NodeType.BossOrGate,
                    NodeType.ServiceOrProgression,
                });
        }

        [Test]
        public void ShouldExposeRequiredEconomicResourceCategories()
        {
            ResourceCategory[] resourceCategories = (ResourceCategory[])Enum.GetValues(typeof(ResourceCategory));

            CollectionAssert.IsSupersetOf(
                resourceCategories,
                new[]
                {
                    ResourceCategory.SoftCurrency,
                    ResourceCategory.RegionMaterial,
                    ResourceCategory.PersistentProgressionMaterial,
                });
        }
    }
}

