using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.State
{
    public sealed class SessionContextStateTests
    {
        [Test]
        public void ShouldSeedRecentNodeFromWorldStateOnce()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(new NodeId("region_001_node_002"));
            SessionContextState sessionContext = new SessionContextState();

            sessionContext.SeedFromWorldState(worldState);
            sessionContext.RecordNodeEntry(new NodeId("region_002_node_001"));
            sessionContext.SeedFromWorldState(worldState);

            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
        }

        [Test]
        public void ShouldTrackRecentSelectionRecentNodeAndPushTargetPredictably()
        {
            SessionContextState sessionContext = new SessionContextState();
            NodeId pushNodeId = new NodeId("region_002_node_001");
            NodeId revisitNodeId = new NodeId("region_001_node_001");

            sessionContext.RecordSelection(pushNodeId, true);
            sessionContext.RecordNodeEntry(pushNodeId);
            sessionContext.RecordSelection(revisitNodeId, false);
            sessionContext.RecordReturnedToWorldContext(pushNodeId);

            Assert.That(sessionContext.HasLastSelectedNode, Is.True);
            Assert.That(sessionContext.LastSelectedNodeId, Is.EqualTo(revisitNodeId));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(pushNodeId));
            Assert.That(sessionContext.HasRecentPushTarget, Is.True);
            Assert.That(sessionContext.RecentPushTargetNodeId, Is.EqualTo(pushNodeId));
        }

        [Test]
        public void ShouldOfferAndConsumeReturnToWorldReentryWithoutChangingOrdinarySeedBehavior()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            NodeId currentNodeId = new NodeId("region_001_node_002");
            NodeId offeredNodeId = new NodeId("region_001_node_004");
            SessionContextState sessionContext = new SessionContextState();

            worldState.SetCurrentNode(currentNodeId);

            sessionContext.SeedFromWorldState(worldState);

            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(currentNodeId));

            sessionContext.OfferReturnToWorldReentry(offeredNodeId);

            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.True);
            Assert.That(sessionContext.ReturnToWorldReentryNodeId, Is.EqualTo(offeredNodeId));
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(offeredNodeId));

            sessionContext.RecordNodeEntry(offeredNodeId);

            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(offeredNodeId));
        }
    }
}

