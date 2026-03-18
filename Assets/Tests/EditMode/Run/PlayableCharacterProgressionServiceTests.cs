using NUnit.Framework;
using Survivalon.Run;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PlayableCharacterProgressionServiceTests
    {
        [Test]
        public void ShouldIncreaseCharacterRankForSuccessfulCombatRun()
        {
            PersistentCharacterState characterState = CreateCharacterState(progressionRank: 0);
            PlayableCharacterProgressionService service = new PlayableCharacterProgressionService();

            bool changed = service.TryApplyResolvedRunProgression(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                RunResolutionState.Succeeded,
                characterState);

            Assert.That(changed, Is.True);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(1));
        }

        [Test]
        public void ShouldNotIncreaseCharacterRankForFailedCombatRun()
        {
            PersistentCharacterState characterState = CreateCharacterState(progressionRank: 2);
            PlayableCharacterProgressionService service = new PlayableCharacterProgressionService();

            bool changed = service.TryApplyResolvedRunProgression(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                RunResolutionState.Failed,
                characterState);

            Assert.That(changed, Is.False);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(2));
        }

        [Test]
        public void ShouldNotIncreaseCharacterRankForSuccessfulNonCombatRun()
        {
            PersistentCharacterState characterState = CreateCharacterState(progressionRank: 2);
            PlayableCharacterProgressionService service = new PlayableCharacterProgressionService();

            bool changed = service.TryApplyResolvedRunProgression(
                RunLifecycleControllerTestData.CreateController().NodeContext,
                RunResolutionState.Succeeded,
                characterState);

            Assert.That(changed, Is.False);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(2));
        }

        private static PersistentCharacterState CreateCharacterState(int progressionRank)
        {
            return new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: progressionRank,
                skillPackageId: "skill_package_vanguard_default");
        }
    }
}
