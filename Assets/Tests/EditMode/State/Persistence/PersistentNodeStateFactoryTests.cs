using System;
using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class PersistentNodeStateFactoryTests
    {
        [Test]
        public void ShouldRejectNegativeInitialProgress()
        {
            Assert.That(
                () => PersistentNodeStateFactory.Create(
                    new NodeId("region_001_node_001"),
                    3,
                    NodeState.Available,
                    initialProgress: -1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("initialProgress"));
        }

        [Test]
        public void ShouldRejectInitialProgressAboveThreshold()
        {
            Assert.That(
                () => PersistentNodeStateFactory.Create(
                    new NodeId("region_001_node_001"),
                    3,
                    NodeState.Available,
                    initialProgress: 4),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("initialProgress"));
        }

        [Test]
        public void ShouldCreateInProgressStateWhenSeededWithAvailableProgress()
        {
            PersistentNodeState nodeState = PersistentNodeStateFactory.Create(
                new NodeId("region_001_node_001"),
                3,
                NodeState.InProgress,
                initialProgress: 1);

            Assert.That(nodeState.State, Is.EqualTo(NodeState.InProgress));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldCreateMasteredStateWhenSeededFromMasteredNode()
        {
            PersistentNodeState nodeState = PersistentNodeStateFactory.Create(
                new NodeId("region_001_node_001"),
                3,
                NodeState.Mastered,
                initialProgress: 3);

            Assert.That(nodeState.State, Is.EqualTo(NodeState.Mastered));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(3));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }
    }
}
