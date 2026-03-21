using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatTriggeredActiveSkillTimingResolverTests
    {
        [Test]
        public void ShouldResolveBurstStrikePeriodicTriggerInterval()
        {
            float intervalSeconds = CombatTriggeredActiveSkillTimingResolver.ResolveIntervalSeconds(
                CombatSkillCatalog.BurstStrike);

            Assert.That(intervalSeconds, Is.EqualTo(2.5f).Within(0.001f));
        }

        [Test]
        public void ShouldResolveBurstTempoPeriodicTriggerInterval()
        {
            float intervalSeconds = CombatTriggeredActiveSkillTimingResolver.ResolveIntervalSeconds(
                CombatSkillCatalog.BurstTempo);

            Assert.That(intervalSeconds, Is.EqualTo(1.75f).Within(0.001f));
        }

        [Test]
        public void ShouldResolveBurstPayloadPeriodicTriggerInterval()
        {
            float intervalSeconds = CombatTriggeredActiveSkillTimingResolver.ResolveIntervalSeconds(
                CombatSkillCatalog.BurstPayload);

            Assert.That(intervalSeconds, Is.EqualTo(2.5f).Within(0.001f));
        }

        [Test]
        public void ShouldResolveNoTimerWhenTriggeredActiveSkillIsMissing()
        {
            float intervalSeconds = CombatTriggeredActiveSkillTimingResolver.ResolveIntervalSeconds(null);

            Assert.That(intervalSeconds, Is.EqualTo(float.PositiveInfinity));
        }
    }
}
