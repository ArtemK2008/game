using NUnit.Framework;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    /// <summary>
    /// Проверяет компактную boss-presentaion сводку для текущего placeholder boss flow.
    /// </summary>
    public sealed class BossEncounterPresentationStateResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnNoneForOrdinaryCombatNode()
        {
            BossEncounterPresentationState presentationState =
                BossEncounterPresentationStateResolver.Resolve(
                    NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(presentationState.IsBossEncounter, Is.False);
            Assert.That(presentationState.EncounterDisplayName, Is.Empty);
            Assert.That(presentationState.StakesSummary, Is.Empty);
        }

        [Test]
        public void Resolve_ShouldDescribeForestGateBossRoleAndStakes()
        {
            BossEncounterPresentationState presentationState =
                BossEncounterPresentationStateResolver.Resolve(
                    NodePlaceholderTestData.CreateForestGateBossPlaceholderState());

            Assert.That(presentationState.IsBossEncounter, Is.True);
            Assert.That(presentationState.EncounterDisplayName, Is.EqualTo("Gate boss"));
            Assert.That(
                presentationState.StakesSummary,
                Is.EqualTo("Gate clear, Boss rewards, Gear reward"));
        }

        [Test]
        public void Resolve_ShouldDescribeCavernGateBossAsRewardFocusedWithoutGateUnlock()
        {
            BossEncounterPresentationState presentationState =
                BossEncounterPresentationStateResolver.Resolve(
                    NodePlaceholderTestData.CreateCavernGateBossPlaceholderState());

            Assert.That(presentationState.IsBossEncounter, Is.True);
            Assert.That(presentationState.EncounterDisplayName, Is.EqualTo("Gate boss"));
            Assert.That(presentationState.StakesSummary, Is.EqualTo("Boss rewards"));
        }
    }
}
