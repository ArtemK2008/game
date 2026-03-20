using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class PlayableCharacterSkillPackageAssignmentServiceTests
    {
        [Test]
        public void ShouldBuildOnlyValidSkillPackageOptionsForSelectedPlayableCharacter()
        {
            PersistentGameState gameState = CreateGameState(
                vanguardIsActive: true,
                vanguardSkillPackageId: "skill_package_vanguard_default",
                strikerSkillPackageId: "skill_package_striker_default");
            PlayableCharacterSkillPackageAssignmentService service =
                new PlayableCharacterSkillPackageAssignmentService();

            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions =
                service.BuildOptionsForSelectedCharacter(gameState);

            Assert.That(skillPackageOptions, Has.Count.EqualTo(2));
            Assert.That(skillPackageOptions[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(skillPackageOptions[0].SkillPackageId, Is.EqualTo("skill_package_vanguard_default"));
            Assert.That(skillPackageOptions[0].DisplayName, Is.EqualTo("Standard Guard"));
            Assert.That(skillPackageOptions[0].IsAssigned, Is.True);
            Assert.That(skillPackageOptions[1].SkillPackageId, Is.EqualTo("skill_package_vanguard_burst_drill"));
            Assert.That(skillPackageOptions[1].DisplayName, Is.EqualTo("Burst Drill"));
            Assert.That(skillPackageOptions[1].IsAssigned, Is.False);
        }

        [Test]
        public void ShouldResolveOnlyCurrentCharactersValidPackageOptionsWhenSelectedCharacterChanges()
        {
            PersistentGameState gameState = CreateGameState(
                vanguardIsActive: false,
                vanguardSkillPackageId: "skill_package_vanguard_default",
                strikerSkillPackageId: "skill_package_striker_default");
            PlayableCharacterSkillPackageAssignmentService service =
                new PlayableCharacterSkillPackageAssignmentService();

            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions =
                service.BuildOptionsForSelectedCharacter(gameState);

            Assert.That(skillPackageOptions, Has.Count.EqualTo(1));
            Assert.That(skillPackageOptions[0].CharacterId, Is.EqualTo("character_striker"));
            Assert.That(skillPackageOptions[0].SkillPackageId, Is.EqualTo("skill_package_striker_default"));
            Assert.That(skillPackageOptions[0].IsAssigned, Is.True);
        }

        [Test]
        public void ShouldRejectAssigningSkillPackageThatIsInvalidForSelectedPlayableCharacter()
        {
            PersistentGameState gameState = CreateGameState(
                vanguardIsActive: false,
                vanguardSkillPackageId: "skill_package_vanguard_default",
                strikerSkillPackageId: "skill_package_striker_default");
            PlayableCharacterSkillPackageAssignmentService service =
                new PlayableCharacterSkillPackageAssignmentService();

            bool didAssign = service.TryAssignSelectedCharacterSkillPackage(
                gameState,
                "skill_package_vanguard_burst_drill");

            Assert.That(didAssign, Is.False);
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo("skill_package_striker_default"));
        }

        [Test]
        public void ShouldAssignRequestedValidSkillPackageToSelectedPlayableCharacter()
        {
            PersistentGameState gameState = CreateGameState(
                vanguardIsActive: true,
                vanguardSkillPackageId: "skill_package_vanguard_default",
                strikerSkillPackageId: "skill_package_striker_default");
            PlayableCharacterSkillPackageAssignmentService service =
                new PlayableCharacterSkillPackageAssignmentService();

            bool didAssign = service.TryAssignSelectedCharacterSkillPackage(
                gameState,
                "skill_package_vanguard_burst_drill");

            Assert.That(didAssign, Is.True);
            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo("skill_package_vanguard_burst_drill"));
        }

        [Test]
        public void ShouldNormalizeInvalidPersistedSkillPackageAssignmentsToCharacterDefaults()
        {
            PersistentGameState gameState = CreateGameState(
                vanguardIsActive: true,
                vanguardSkillPackageId: "skill_package_striker_default",
                strikerSkillPackageId: "skill_package_vanguard_burst_drill");
            PlayableCharacterSkillPackageAssignmentService service =
                new PlayableCharacterSkillPackageAssignmentService();

            service.EnsureValidAssignments(gameState);

            Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo("skill_package_vanguard_default"));
            Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
            Assert.That(strikerState.SkillPackageId, Is.EqualTo("skill_package_striker_default"));
        }

        private static PersistentGameState CreateGameState(
            bool vanguardIsActive,
            string vanguardSkillPackageId,
            string strikerSkillPackageId)
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: vanguardIsActive,
                skillPackageId: vanguardSkillPackageId));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: !vanguardIsActive,
                skillPackageId: strikerSkillPackageId));
            return gameState;
        }
    }
}
