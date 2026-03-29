using NUnit.Framework;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapArtResolverTests
    {
        private readonly WorldMapArtResolver artResolver = new WorldMapArtResolver();

        [Test]
        public void ResolveNodeIconKind_ShouldReturnLockedForLockedNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(NodeType.Combat, NodeState.Locked);

            Assert.That(artResolver.ResolveNodeIconKind(nodeOption), Is.EqualTo(WorldMapNodeIconKind.Locked));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldKeepMeaningFirstIconsForSelectedAndCurrentContextNodes()
        {
            WorldMapNodeOption selectedNode = CreateNodeOption(
                NodeType.Combat,
                NodeState.Available,
                isSelected: true,
                hasRegionMaterialYieldContent: true,
                isFarmReady: true);
            WorldMapNodeOption currentNode = CreateNodeOption(NodeType.ServiceOrProgression, NodeState.Available, isCurrentContext: true);

            Assert.That(artResolver.ResolveNodeIconKind(selectedNode), Is.EqualTo(WorldMapNodeIconKind.Farm));
            Assert.That(artResolver.ResolveNodeIconKind(currentNode), Is.EqualTo(WorldMapNodeIconKind.Service));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldReturnServiceForServiceNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(NodeType.ServiceOrProgression, NodeState.Available);

            Assert.That(artResolver.ResolveNodeIconKind(nodeOption), Is.EqualTo(WorldMapNodeIconKind.Service));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldReturnBossGateForUnlockedBossNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(NodeType.BossOrGate, NodeState.InProgress);

            Assert.That(artResolver.ResolveNodeIconKind(nodeOption), Is.EqualTo(WorldMapNodeIconKind.BossGate));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldReturnEliteForOptionalChallengeCombatNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(
                NodeType.Combat,
                NodeState.Available,
                optionalChallengeDisplayName: "Elite challenge");

            Assert.That(artResolver.ResolveNodeIconKind(nodeOption), Is.EqualTo(WorldMapNodeIconKind.Elite));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldReturnFarmForExplicitYieldCombatNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(
                NodeType.Combat,
                NodeState.Cleared,
                hasRegionMaterialYieldContent: true,
                isFarmReady: true);

            Assert.That(artResolver.ResolveNodeIconKind(nodeOption), Is.EqualTo(WorldMapNodeIconKind.Farm));
        }

        [Test]
        public void ResolveNodeIconKind_ShouldReturnOrdinaryCombatForNonEliteNonFarmCombatNodes()
        {
            WorldMapNodeOption nodeOption = CreateNodeOption(NodeType.Combat, NodeState.Available);

            Assert.That(
                artResolver.ResolveNodeIconKind(nodeOption),
                Is.EqualTo(WorldMapNodeIconKind.OrdinaryCombat));
        }

        private static WorldMapNodeOption CreateNodeOption(
            NodeType nodeType,
            NodeState nodeState,
            bool isCurrentContext = false,
            bool isSelected = false,
            bool hasRegionMaterialYieldContent = false,
            bool isFarmReady = false,
            string optionalChallengeDisplayName = null)
        {
            return new WorldMapNodeOption(
                new NodeId("test_node"),
                new RegionId("test_region"),
                nodeType,
                nodeState,
                isSelectable: nodeState != NodeState.Locked,
                isCurrentContext: isCurrentContext,
                isSelected: isSelected,
                locationDisplayName: "Test Region",
                nodeDisplayName: "Test Node",
                pathRole: WorldMapPathRole.ForwardRoute,
                hasRegionMaterialYieldContent: hasRegionMaterialYieldContent,
                isFarmReady: isFarmReady,
                optionalChallengeDisplayName: optionalChallengeDisplayName);
        }
    }
}
