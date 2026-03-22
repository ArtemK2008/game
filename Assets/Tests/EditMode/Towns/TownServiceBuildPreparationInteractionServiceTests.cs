using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.Run;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
    /// <summary>
    /// Проверяет town/service build preparation orchestration после выноса runtime character services в Characters.
    /// </summary>
    public sealed class TownServiceBuildPreparationInteractionServiceTests
    {
        [Test]
        public void PublicInteractionMethods_ShouldNotDependOnGearPresentationOptionModel()
        {
            ParameterInfo[] publicParameters = typeof(TownServiceBuildPreparationInteractionService)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(methodInfo => methodInfo.GetParameters())
                .ToArray();

            Assert.That(
                publicParameters.Any(parameterInfo => parameterInfo.ParameterType == typeof(PlayableCharacterGearAssignmentOption)),
                Is.False);
        }

        [Test]
        public void ShouldAssignSelectedCharacterSkillPackageAndPersistUpdatedState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceBuildPreparationInteractionService interactionService =
                new TownServiceBuildPreparationInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            bool changed = interactionService.TryAssignSkillPackage(
                gameState,
                PlayableCharacterSkillPackageIds.VanguardBurstDrill);

            Assert.That(changed, Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(
                storage.SavedGameState.TryGetCharacterState(
                    "character_vanguard",
                    out PersistentCharacterState savedVanguardState),
                Is.True);
            Assert.That(savedVanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
        }

        [Test]
        public void ShouldAssignGearByGearIdThroughTownInteractionAndPersistUpdatedLoadoutState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceBuildPreparationInteractionService interactionService =
                new TownServiceBuildPreparationInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            bool primaryChanged = interactionService.TryAssignGear(gameState, GearIds.TrainingBlade);
            bool supportChanged = interactionService.TryAssignGear(gameState, GearIds.GuardCharm);

            Assert.That(primaryChanged, Is.True);
            Assert.That(supportChanged, Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState primaryGearState),
                Is.True);
            Assert.That(primaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.SecondarySupport,
                    out EquippedGearState supportGearState),
                Is.True);
            Assert.That(supportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(
                storage.SavedGameState.TryGetCharacterState(
                    "character_vanguard",
                    out PersistentCharacterState savedVanguardState),
                Is.True);
            Assert.That(
                savedVanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out EquippedGearState savedPrimaryGearState),
                Is.True);
            Assert.That(savedPrimaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(
                savedVanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.SecondarySupport,
                    out EquippedGearState savedSupportGearState),
                Is.True);
            Assert.That(savedSupportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
        }

        [Test]
        public void ShouldClearGearByCategoryThroughTownInteractionAndPersistUpdatedLoadoutState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceBuildPreparationInteractionService interactionService =
                new TownServiceBuildPreparationInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService();

            Assert.That(gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade), Is.True);

            bool changed = interactionService.TryClearGear(gameState, GearCategory.PrimaryCombat);

            Assert.That(changed, Is.True);
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(
                vanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out _),
                Is.False);
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(
                storage.SavedGameState.TryGetCharacterState(
                    "character_vanguard",
                    out PersistentCharacterState savedVanguardState),
                Is.True);
            Assert.That(
                savedVanguardState.LoadoutState.TryGetEquippedGearState(
                    GearCategory.PrimaryCombat,
                    out _),
                Is.False);
        }

        [Test]
        public void ShouldUseUpdatedTownBuildStateForFutureRunEntry()
        {
            TownServiceBuildPreparationInteractionService interactionService =
                new TownServiceBuildPreparationInteractionService();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            Assert.That(
                interactionService.TryAssignSkillPackage(
                    gameState,
                    PlayableCharacterSkillPackageIds.VanguardBurstDrill),
                Is.True);
            Assert.That(interactionService.TryAssignGear(gameState, GearIds.TrainingBlade), Is.True);
            Assert.That(interactionService.TryAssignGear(gameState, GearIds.GuardCharm), Is.True);

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.RequiresRunTimeSkillUpgradeChoice, Is.True);
            Assert.That(
                controller.TrySelectRunTimeSkillUpgrade(CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId),
                Is.True);
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(160f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(16f));
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public void Save(PersistentGameState gameState)
            {
                SavedGameState = gameState;
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = SavedGameState;
                return gameState != null;
            }
        }
    }
}
