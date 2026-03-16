using NUnit.Framework;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class TrackedNodeProgressRulesTests
    {
        [Test]
        public void ShouldReturnExpectedDefaultThresholdForTrackedAndUntrackedNodes()
        {
            Assert.That(TrackedNodeProgressRules.ShouldTrack(NodeType.Combat), Is.True);
            Assert.That(TrackedNodeProgressRules.ShouldTrack(NodeType.BossOrGate), Is.True);
            Assert.That(TrackedNodeProgressRules.ShouldTrack(NodeType.ServiceOrProgression), Is.False);

            Assert.That(TrackedNodeProgressRules.GetDefaultThreshold(NodeType.Combat), Is.EqualTo(3));
            Assert.That(TrackedNodeProgressRules.GetDefaultThreshold(NodeType.BossOrGate), Is.EqualTo(3));
            Assert.That(TrackedNodeProgressRules.GetDefaultThreshold(NodeType.ServiceOrProgression), Is.EqualTo(0));
        }
    }
}
