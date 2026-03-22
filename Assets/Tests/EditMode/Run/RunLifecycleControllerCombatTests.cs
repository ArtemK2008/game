using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    /// <summary>
    /// Проверяет боевой run lifecycle после переноса runtime character services в Characters.
    /// </summary>
    public sealed class RunLifecycleControllerCombatTests
    {
        [Test]
        public void ShouldCreateCombatShellContextWhenCombatRunEntersActiveState()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            bool enteredActive = controller.TryEnterActiveState();

            Assert.That(enteredActive, Is.True);
            Assert.That(controller.HasCombatEncounterState, Is.True);
            Assert.That(controller.CombatContext, Is.Not.Null);
            Assert.That(controller.CombatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(controller.CombatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(controller.CombatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.PlayerEntity.IsActive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(controller.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(controller.CombatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(7f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(1.25f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(2f));
            Assert.That(controller.CombatContext.EnemyEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.IsActive, Is.True);
            Assert.That(controller.CombatEncounterState.PlayerEntity.CurrentHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(75f));
        }

        [Test]
        public void ShouldAdvanceCombatUntilCombatRunResolves()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 5 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActiveEnemy, Is.False);
            Assert.That(controller.CombatEncounterState.ActiveEnemyCount, Is.EqualTo(0));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(0f));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void ShouldResolveForestPushCombatAgainstDurableEnemyMoreSlowlyThanFarmCombat()
        {
            RunLifecycleController farmController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState());
            RunLifecycleController pushController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreatePushCombatNodeState());

            RunLifecycleControllerTestData.RunToPostRun(farmController);
            RunLifecycleControllerTestData.RunToPostRun(pushController);

            Assert.That(farmController.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(farmController.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(farmController.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(7f));
            Assert.That(farmController.CombatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(1.25f));
            Assert.That(pushController.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Bulwark Raider"));
            Assert.That(pushController.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(105f));
            Assert.That(pushController.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(9f));
            Assert.That(pushController.CombatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.85f));
            Assert.That(farmController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(pushController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(
                pushController.CombatEncounterState.ElapsedCombatSeconds,
                Is.GreaterThan(farmController.CombatEncounterState.ElapsedCombatSeconds));
            Assert.That(
                pushController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.LessThan(farmController.CombatEncounterState.PlayerEntity.CurrentHealth));
        }

        [Test]
        public void ShouldCreateSharperEarlyPressureForEnemyUnitAndStrongerAttritionForBulwarkRaider()
        {
            RunLifecycleController farmController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState());
            RunLifecycleController pushController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreatePushCombatNodeState());

            Assert.That(farmController.TryStartAutomaticFlow(), Is.True);
            Assert.That(pushController.TryStartAutomaticFlow(), Is.True);
            Assert.That(farmController.TryAdvanceAutomaticTime(2.5f), Is.True);
            Assert.That(pushController.TryAdvanceAutomaticTime(2.5f), Is.True);

            Assert.That(farmController.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(pushController.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(
                farmController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.LessThan(pushController.CombatEncounterState.PlayerEntity.CurrentHealth));
            Assert.That(
                farmController.CombatEncounterState.EnemyEntity.CurrentHealth,
                Is.LessThan(pushController.CombatEncounterState.EnemyEntity.CurrentHealth));
            Assert.That(
                pushController.CombatEncounterState.ElapsedCombatSeconds,
                Is.EqualTo(farmController.CombatEncounterState.ElapsedCombatSeconds).Within(0.001f));
        }

        [Test]
        public void ShouldStartAutomaticCombatFlowWithoutAdvancingTime()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldIgnoreZeroElapsedAutomaticTimeProgression()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            controller.TryStartAutomaticFlow();

            bool changed = controller.TryAdvanceAutomaticTime(0f);

            Assert.That(changed, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldAdvanceAutomaticCombatFlowFromNodeEntryToPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                worldGraph: BootstrapWorldTestData.CreateWorldGraph(),
                persistentContext: new RunPersistentContext(
                    persistentWorldState: worldState,
                    resourceBalancesState: resourceBalances));

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(controller.RunResult.RewardPayload.MaterialRewards[0].Amount, Is.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.BossMaterialRewards, Is.Empty);
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(1));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
        }

        [Test]
        public void ShouldResolveCombatRunAsFailedWhenPlayerIsDefeated()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateBossCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Gate Boss"));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(180f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(16f));

            for (int index = 0; index < 12 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActivePlayer, Is.False);
            Assert.That(controller.CombatEncounterState.ActivePlayerCount, Is.EqualTo(0));
        }

        [Test]
        public void ShouldAdvanceAutomaticHostileCombatFlowToFailedPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: new RunPersistentContext(
                    persistentWorldState: worldState,
                    resourceBalancesState: resourceBalances));

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 64 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.RunResult.RewardPayload, Is.SameAs(RunRewardPayload.Empty));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(0));
        }

        [Test]
        public void ShouldGrantBossRewardBundleWhenBossCombatIsWon()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(gameState));

            RunLifecycleControllerTestData.RunToPostRun(controller);

            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.MaterialRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(controller.RunResult.RewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(controller.RunResult.RewardPayload.BossMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(controller.RunResult.RewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(2));
        }

        [Test]
        public void ShouldIncreaseFutureBossRewardBundleWhenBossSalvageProjectIsPurchased()
        {
            PersistentGameState baselineGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState upgradedGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();

            Assert.That(selectionService.TrySelectCharacter(baselineGameState, "character_striker"), Is.True);
            Assert.That(selectionService.TrySelectCharacter(upgradedGameState, "character_striker"), Is.True);
            upgradedGameState.ResourceBalances.Add(
                ResourceCategory.PersistentProgressionMaterial,
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.BossSalvageProject).CostAmount);
            Assert.That(
                boardService.TryPurchase(upgradedGameState, AccountWideUpgradeId.BossSalvageProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));

            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(baselineGameState));
            RunLifecycleController upgradedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(upgradedGameState));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(upgradedController);

            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(upgradedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(baselineController.RunResult.RewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(upgradedController.RunResult.RewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(3));
            Assert.That(baselineGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(2));
            Assert.That(upgradedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(3));
        }

        [Test]
        public void ShouldStopAutomaticCombatFlowAfterPostRunIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            controller.TryStartAutomaticFlow();
            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            float resolvedElapsedSeconds = controller.CombatEncounterState.ElapsedCombatSeconds;
            bool advancedAfterPostRun = controller.TryAdvanceAutomaticTime(1f);

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(advancedAfterPostRun, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(resolvedElapsedSeconds).Within(0.001f));
        }

        [Test]
        public void ShouldRejectManualCombatResolutionBeforeCombatOutcomeIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(RunLifecycleControllerTestData.CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);

            Assert.That(resolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldIncreasePlayerCombatBaselineAndRemainingHealthWhenAccountWideUpgradeIsPurchased()
        {
            PersistentProgressionState purchasedProgressionState =
                CreatePurchasedProgressionState(AccountWideUpgradeId.CombatBaselineProject);
            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState());
            RunLifecycleController upgradedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: new RunPersistentContext(
                    persistentProgressionState: purchasedProgressionState));

            Assert.That(baselineController.TryStartAutomaticFlow(), Is.True);
            Assert.That(upgradedController.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < 24 && baselineController.CurrentState != RunLifecycleState.PostRun; index++)
            {
                baselineController.TryAdvanceAutomaticTime(0.25f);
            }

            for (int index = 0; index < 24 && upgradedController.CurrentState != RunLifecycleState.PostRun; index++)
            {
                upgradedController.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(baselineController.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(upgradedController.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(130f));
            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(upgradedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(
                upgradedController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.GreaterThan(baselineController.CombatEncounterState.PlayerEntity.CurrentHealth));
        }

        [Test]
        public void ShouldResolveStandardCombatFasterWhenTrainingBladeIsEquipped()
        {
            PersistentGameState baselineGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState equippedGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService();
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(
                    equippedGameState,
                    GearIds.TrainingBlade),
                Is.True);

            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(baselineGameState));
            RunLifecycleController equippedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(equippedGameState));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(equippedController);

            Assert.That(baselineController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(equippedController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(16f));
            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(equippedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(
                equippedController.CombatEncounterState.ElapsedCombatSeconds,
                Is.LessThan(baselineController.CombatEncounterState.ElapsedCombatSeconds));
            Assert.That(
                equippedController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.GreaterThanOrEqualTo(baselineController.CombatEncounterState.PlayerEntity.CurrentHealth));
        }

        [Test]
        public void ShouldIncreasePlayerCombatBaselineAndRemainingHealthWhenGuardCharmIsEquipped()
        {
            PersistentGameState baselineGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState equippedGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService();
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(
                    equippedGameState,
                    GearIds.GuardCharm),
                Is.True);

            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(baselineGameState));
            RunLifecycleController equippedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(equippedGameState));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(equippedController);

            Assert.That(baselineController.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(equippedController.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(160f));
            Assert.That(baselineController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(equippedController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(equippedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(
                equippedController.CombatEncounterState.PlayerEntity.CurrentHealth,
                Is.GreaterThan(baselineController.CombatEncounterState.PlayerEntity.CurrentHealth));
        }

        [Test]
        public void ShouldUsePersistentPlayableCharacterForCombatEntryWhenRunPersistentContextIsBuiltFromGameState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            Assert.That(persistentContext.PlayableCharacter, Is.Not.Null);
            Assert.That(persistentContext.PlayableCharacterState, Is.Not.Null);
            Assert.That(persistentContext.PlayableCharacter.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(persistentContext.PlayableCharacterState.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(controller.CombatContext.PlayerEntity.PassiveSkills, Is.Empty);
        }

        [Test]
        public void ShouldUseSelectedSecondPlayableCharacterForCombatEntryWhenRunPersistentContextIsBuiltFromGameState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.RequiresRunTimeSkillUpgradeChoice, Is.True);
            Assert.That(controller.RunTimeSkillUpgradeOptions.Count, Is.EqualTo(2));
            Assert.That(controller.TryStartAutomaticFlow(), Is.False);
            Assert.That(
                controller.TrySelectRunTimeSkillUpgrade(CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId),
                Is.True);
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            Assert.That(persistentContext.PlayableCharacter, Is.Not.Null);
            Assert.That(persistentContext.PlayableCharacterState, Is.Not.Null);
            Assert.That(persistentContext.PlayableCharacter.CharacterId, Is.EqualTo("character_striker"));
            Assert.That(persistentContext.PlayableCharacterState.CharacterId, Is.EqualTo("character_striker"));
            Assert.That(controller.CombatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_striker")));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Striker"));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(110f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.35f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(8f));
            Assert.That(controller.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                controller.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(controller.CombatContext.PlayerEntity.PassiveSkills.Count, Is.EqualTo(1));
            Assert.That(controller.CombatContext.PlayerEntity.PassiveSkills[0].SkillId, Is.EqualTo("combat_passive_relentless_assault"));
        }

        [Test]
        public void ShouldUseAssignedSkillPackageForCombatEntryWhenSelectedCharacterOverridesProfileDefault()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            vanguardState.SetSkillPackageId(PlayableCharacterSkillPackageIds.VanguardBurstDrill);

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.RequiresRunTimeSkillUpgradeChoice, Is.True);
            Assert.That(
                controller.TrySelectRunTimeSkillUpgrade(CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId),
                Is.True);
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            Assert.That(persistentContext.PlayableCharacter.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                controller.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstPayload));
            Assert.That(controller.CombatContext.PlayerEntity.PassiveSkills, Is.Empty);
        }

        [Test]
        public void ShouldNotPersistRunTimeSkillUpgradeChoiceAcrossNewRunControllers()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);
            RunPersistentContext firstPersistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController firstController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: firstPersistentContext);

            Assert.That(
                firstController.TrySelectRunTimeSkillUpgrade(CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId),
                Is.True);
            Assert.That(firstController.TryStartAutomaticFlow(), Is.True);
            Assert.That(firstController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                firstController.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstPayload));

            RunPersistentContext secondPersistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController secondController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: secondPersistentContext);

            Assert.That(secondController.RequiresRunTimeSkillUpgradeChoice, Is.True);
            Assert.That(secondController.TryStartAutomaticFlow(), Is.False);
            Assert.That(secondPersistentContext.PlayableCharacterState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }

        [Test]
        public void ShouldApplyAccountWideProgressionEffectsOnTopOfPersistentPlayableCharacterBaseline()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(
                ResourceCategory.PersistentProgressionMaterial,
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.CombatBaselineProject).CostAmount);
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            Assert.That(persistentContext.PlayableCharacter.CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(130f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
        }

        [Test]
        public void ShouldIncreaseSelectedPlayableCharacterProgressionRankAfterSuccessfulCombatRun()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            RunLifecycleControllerTestData.RunToPostRun(controller);

            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(persistentContext.PlayableCharacterState.ProgressionRank, Is.EqualTo(1));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(1));
        }

        [Test]
        public void ShouldIncreaseSelectedSecondPlayableCharacterProgressionRankWithoutChangingInactiveCharacter()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);
            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            RunLifecycleControllerTestData.RunToPostRun(controller);

            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(persistentContext.PlayableCharacterState.CharacterId, Is.EqualTo("character_striker"));
            Assert.That(persistentContext.PlayableCharacterState.ProgressionRank, Is.EqualTo(1));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                Is.True);
            Assert.That(vanguardState.ProgressionRank, Is.EqualTo(0));
            Assert.That(
                gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.ProgressionRank, Is.EqualTo(1));
        }

        [Test]
        public void ShouldApplyCharacterProgressionOnTopOfAccountWideEffectsForFutureRunEntry()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 3);
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.PushOffenseProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(
                gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            characterState.IncreaseProgressionRank(2);

            RunPersistentContext persistentContext = RunPersistentContext.FromGameState(gameState);
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: persistentContext);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(140f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
        }

        [Test]
        public void ShouldTurnCurrentBossCombatFromVanguardFailureIntoStrikerSuccess()
        {
            PersistentGameState vanguardGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState strikerGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            Assert.That(selectionService.TrySelectCharacter(strikerGameState, "character_striker"), Is.True);

            RunLifecycleController vanguardController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(vanguardGameState));
            RunLifecycleController strikerController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(strikerGameState));

            RunLifecycleControllerTestData.RunToPostRun(vanguardController);
            RunLifecycleControllerTestData.RunToPostRun(strikerController);

            Assert.That(vanguardController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(vanguardController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(vanguardController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(strikerController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Striker"));
            Assert.That(strikerController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(strikerController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                strikerController.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(strikerController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(strikerController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(
                strikerController.CombatEncounterState.ElapsedCombatSeconds,
                Is.LessThan(vanguardController.CombatEncounterState.ElapsedCombatSeconds));
        }

        [Test]
        public void ShouldResolveSelectedStrikerCombatRunFasterThanVanguardBecauseOfCurrentActiveSkillPackage()
        {
            PersistentGameState vanguardGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState strikerGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            Assert.That(selectionService.TrySelectCharacter(strikerGameState, "character_striker"), Is.True);

            RunLifecycleController vanguardController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(vanguardGameState));
            RunLifecycleController strikerController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(strikerGameState));

            RunLifecycleControllerTestData.RunToPostRun(vanguardController);
            RunLifecycleControllerTestData.RunToPostRun(strikerController);

            Assert.That(vanguardController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(strikerController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(vanguardController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(vanguardController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(strikerController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Striker"));
            Assert.That(strikerController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                strikerController.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(
                strikerController.CombatEncounterState.ElapsedCombatSeconds,
                Is.LessThan(vanguardController.CombatEncounterState.ElapsedCombatSeconds));
        }

        [Test]
        public void ShouldTurnCurrentBossCombatFromVanguardFailureIntoSuccessWhenBurstDrillPackageIsAssigned()
        {
            PersistentGameState baselineGameState = BootstrapWorldTestData.CreateGameState();
            PersistentGameState assignedGameState = BootstrapWorldTestData.CreateGameState();
            Assert.That(
                assignedGameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState assignedVanguardState),
                Is.True);
            assignedVanguardState.SetSkillPackageId(PlayableCharacterSkillPackageIds.VanguardBurstDrill);

            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(baselineGameState));
            RunLifecycleController assignedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(assignedGameState));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(assignedController);

            Assert.That(baselineController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(baselineController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(baselineController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(assignedController.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(assignedController.CombatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(
                assignedController.CombatContext.PlayerEntity.TriggeredActiveSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(assignedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(assignedController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(
                assignedController.CombatEncounterState.ElapsedCombatSeconds,
                Is.LessThan(baselineController.CombatEncounterState.ElapsedCombatSeconds));
        }

        [Test]
        public void ShouldTurnCurrentBossCombatFromFailureIntoSuccessWhenPushOffenseProjectIsPurchased()
        {
            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState());
            RunLifecycleController upgradedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentContext: new RunPersistentContext(
                    persistentProgressionState: CreatePurchasedProgressionState(AccountWideUpgradeId.PushOffenseProject)));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(upgradedController);

            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(baselineController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(upgradedController.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(upgradedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(upgradedController.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(
                upgradedController.CombatEncounterState.ElapsedCombatSeconds,
                Is.LessThan(baselineController.CombatEncounterState.ElapsedCombatSeconds));
        }

        [Test]
        public void ShouldIncreaseOrdinaryRegionMaterialRewardWhenFarmYieldProjectIsPurchased()
        {
            ResourceBalancesState baselineBalances = new ResourceBalancesState();
            ResourceBalancesState upgradedBalances = new ResourceBalancesState();
            RunLifecycleController baselineController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                worldGraph: BootstrapWorldTestData.CreateWorldGraph(),
                persistentContext: new RunPersistentContext(resourceBalancesState: baselineBalances));
            RunLifecycleController upgradedController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                worldGraph: BootstrapWorldTestData.CreateWorldGraph(),
                persistentContext: new RunPersistentContext(
                    resourceBalancesState: upgradedBalances,
                    persistentProgressionState: CreatePurchasedProgressionState(AccountWideUpgradeId.FarmYieldProject)));

            RunLifecycleControllerTestData.RunToPostRun(baselineController);
            RunLifecycleControllerTestData.RunToPostRun(upgradedController);

            Assert.That(baselineController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(upgradedController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(baselineController.RunResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(upgradedController.RunResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(baselineController.RunResult.RewardPayload.MaterialRewards[0].Amount, Is.EqualTo(1));
            Assert.That(upgradedController.RunResult.RewardPayload.MaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(baselineController.RunResult.RewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(upgradedController.RunResult.RewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(baselineBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(1));
            Assert.That(upgradedBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
            Assert.That(upgradedBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
        }

        private static PersistentProgressionState CreatePurchasedProgressionState(params AccountWideUpgradeId[] upgradeIds)
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();
            int requiredMaterial = 0;

            foreach (AccountWideUpgradeId upgradeId in upgradeIds)
            {
                requiredMaterial += AccountWideProgressionUpgradeCatalog.Get(upgradeId).CostAmount;
            }

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, requiredMaterial);

            foreach (AccountWideUpgradeId upgradeId in upgradeIds)
            {
                AccountWideUpgradePurchaseStatus purchaseStatus =
                    boardService.TryPurchase(gameState, upgradeId);
                Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            }

            return gameState.ProgressionState;
        }
    }
}

