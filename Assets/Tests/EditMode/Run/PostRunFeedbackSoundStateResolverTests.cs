using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    /// <summary>
    /// РџСЂРѕРІРµСЂСЏРµС‚, РєР°РєРёРµ system feedback-Р·РІСѓРєРё РЅСѓР¶РЅС‹ РїСЂРё РїРѕРєР°Р·Рµ resolved post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundStateResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnNoneForOrdinaryRunWithoutMilestoneOutcomes()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                CreateRunResult(RunResolutionState.Succeeded));

            Assert.That(feedbackSoundState.HasRequestedSounds, Is.False);
            Assert.That(feedbackSoundState.RequestedSounds, Is.Empty);
        }

        [Test]
        public void Resolve_ShouldRequestRouteUnlockSoundForRouteUnlockOutcome()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                CreateRunResult(RunResolutionState.Succeeded, didUnlockRoute: true));

            CollectionAssert.AreEqual(
                new[]
                {
                    UiSystemFeedbackSoundId.StateRouteUnlock,
                },
                feedbackSoundState.RequestedSounds);
        }

        [Test]
        public void Resolve_ShouldRequestNodeClearAndRouteUnlockSoundsForClearThresholdUnlockOutcome()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                CreateRunResult(
                    RunResolutionState.Succeeded,
                    didUnlockRoute: true,
                    rewardPayload: CreateMilestoneRewardPayload()));

            CollectionAssert.AreEqual(
                new[]
                {
                    UiSystemFeedbackSoundId.StateNodeClear,
                    UiSystemFeedbackSoundId.StateRouteUnlock,
                },
                feedbackSoundState.RequestedSounds);
        }

        [Test]
        public void Resolve_ShouldRequestBossRewardAndRouteUnlockSoundsForBossRewardUnlockOutcome()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                CreateRunResult(
                    RunResolutionState.Succeeded,
                    rewardPayload: CreateBossRewardPayload(),
                    bossProgressionGateUnlock: BossProgressionGateUnlockResult.CreateUnlocked(
                        BootstrapWorldScenario.CavernGateNodeId)));

            CollectionAssert.AreEqual(
                new[]
                {
                    UiSystemFeedbackSoundId.StateBossReward,
                    UiSystemFeedbackSoundId.StateRouteUnlock,
                },
                feedbackSoundState.RequestedSounds);
        }

        private static RunResult CreateRunResult(
            RunResolutionState resolutionState,
            bool didUnlockRoute = false,
            RunRewardPayload rewardPayload = null,
            BossProgressionGateUnlockResult bossProgressionGateUnlock = null)
        {
            return new RunResult(
                new NodeId("node_test"),
                resolutionState,
                rewardPayload ?? RunRewardPayload.Empty,
                nodeProgressDelta: 0,
                nodeProgressValue: 0,
                nodeProgressThreshold: 0,
                persistentProgressionDelta: 0,
                didUnlockRoute: didUnlockRoute,
                nextActionContext: new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true),
                bossProgressionGateUnlock: bossProgressionGateUnlock);
        }

        private static RunRewardPayload CreateMilestoneRewardPayload()
        {
            return new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                new[] { new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1) });
        }

        private static RunRewardPayload CreateBossRewardPayload()
        {
            return new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                new[] { new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2) });
        }
    }
}
