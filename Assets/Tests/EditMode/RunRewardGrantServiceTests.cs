using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunRewardGrantServiceTests
    {
        [Test]
        public void ShouldApplySoftCurrencyRewardToPersistentBalances()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                new[]
                {
                    new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                },
                System.Array.Empty<RunMaterialReward>());

            service.Grant(resourceBalances, rewardPayload);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
        }

        [Test]
        public void ShouldLeaveBalancesUnchangedWhenRewardPayloadIsEmpty()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();

            service.Grant(resourceBalances, RunRewardPayload.Empty);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(0));
        }
    }
}
