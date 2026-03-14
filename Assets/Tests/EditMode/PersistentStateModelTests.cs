using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class PersistentStateModelTests
    {
        [Test]
        public void ShouldMoveNodeToInProgressWhenProgressIsAppliedBelowThreshold()
        {
            PersistentNodeState nodeState = new PersistentNodeState(
                new NodeId("region_001_node_001"),
                5,
                NodeState.Available);

            nodeState.ApplyUnlockProgress(2);

            Assert.That(nodeState.State, Is.EqualTo(NodeState.InProgress));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(2));
            Assert.That(nodeState.IsReplayAvailable, Is.True);
            Assert.That(nodeState.IsCompleted, Is.False);
        }

        [Test]
        public void ShouldMarkNodeClearedWhenProgressReachesThreshold()
        {
            PersistentNodeState nodeState = new PersistentNodeState(
                new NodeId("region_001_node_001"),
                3,
                NodeState.Available);

            nodeState.ApplyUnlockProgress(3);

            Assert.That(nodeState.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(3));
            Assert.That(nodeState.IsCompleted, Is.True);
        }

        [Test]
        public void ShouldRejectProgressOnLockedNode()
        {
            PersistentNodeState nodeState = new PersistentNodeState(
                new NodeId("region_001_node_001"),
                3);

            TestDelegate action = () => nodeState.ApplyUnlockProgress(1);

            Assert.That(action, Throws.InvalidOperationException);
        }

        [Test]
        public void ShouldRejectProgressOnUntrackedNodeState()
        {
            PersistentNodeState nodeState = new PersistentNodeState(
                new NodeId("region_001_node_003"),
                0,
                NodeState.Available);

            TestDelegate action = () => nodeState.ApplyUnlockProgress(1);

            Assert.That(action, Throws.InvalidOperationException);
        }

        [Test]
        public void ShouldAddAndSpendResourceBalances()
        {
            ResourceBalancesState resourceBalances = new ResourceBalancesState();

            resourceBalances.Add(ResourceCategory.SoftCurrency, 25);
            bool spent = resourceBalances.TrySpend(ResourceCategory.SoftCurrency, 10);

            Assert.That(spent, Is.True);
            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(15));
        }

        [Test]
        public void ShouldRejectSpendWhenResourceBalanceIsTooLow()
        {
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            resourceBalances.Add(ResourceCategory.RegionMaterial, 2);

            bool spent = resourceBalances.TrySpend(ResourceCategory.RegionMaterial, 3);

            Assert.That(spent, Is.False);
            Assert.That(resourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
        }
    }
}
