using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    /// <summary>
    /// РџСЂРѕРІРµСЂСЏРµС‚, РєР°РєРёРµ system feedback-Р·РІСѓРєРё РЅСѓР¶РЅС‹ РїСЂРё РїРѕРєР°Р·Рµ resolved post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundStateResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnNoneForOrdinaryRunWithoutUnlocks()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                CreateRunResult(RunResolutionState.Succeeded));

            Assert.That(feedbackSoundState.ShouldPlayBossClearSound, Is.False);
            Assert.That(feedbackSoundState.ShouldPlayUnlockSound, Is.False);
        }

        [Test]
        public void Resolve_ShouldRequestUnlockSoundForRouteUnlockOutcome()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                CreateRunResult(RunResolutionState.Succeeded, didUnlockRoute: true));

            Assert.That(feedbackSoundState.ShouldPlayBossClearSound, Is.False);
            Assert.That(feedbackSoundState.ShouldPlayUnlockSound, Is.True);
        }

        [Test]
        public void Resolve_ShouldRequestBossClearAndUnlockSoundsForSuccessfulBossGateClear()
        {
            PostRunFeedbackSoundState feedbackSoundState = new PostRunFeedbackSoundStateResolver().Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                CreateRunResult(
                    RunResolutionState.Succeeded,
                    bossProgressionGateUnlock: BossProgressionGateUnlockResult.CreateUnlocked(
                        BootstrapWorldScenario.CavernGateNodeId)));

            Assert.That(feedbackSoundState.ShouldPlayBossClearSound, Is.True);
            Assert.That(feedbackSoundState.ShouldPlayUnlockSound, Is.True);
        }

        private static RunResult CreateRunResult(
            RunResolutionState resolutionState,
            bool didUnlockRoute = false,
            BossProgressionGateUnlockResult bossProgressionGateUnlock = null)
        {
            return new RunResult(
                new NodeId("node_test"),
                resolutionState,
                RunRewardPayload.Empty,
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
    }
}
