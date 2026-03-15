using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunRewardResolutionServiceTests
    {
        [Test]
        public void ShouldGrantSoftCurrencyForSuccessfulCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded);

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldReturnEmptyPayloadForFailedCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Failed);

            Assert.That(rewardPayload, Is.SameAs(RunRewardPayload.Empty));
        }

        [Test]
        public void ShouldReturnEmptyPayloadForSuccessfulNonCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunResolutionState.Succeeded);

            Assert.That(rewardPayload, Is.SameAs(RunRewardPayload.Empty));
        }
    }
}
