using NUnit.Framework;
using Survivalon.Characters;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Core;
using Survivalon.Data.Progression;
using Survivalon.Startup;
using Survivalon.State.Persistence;
using Survivalon.Towns;
using Survivalon.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Startup
{
    /// <summary>
    /// Проверяет startup -> combat flow при сохранении progression behavior после data-split рефактора.
    /// </summary>
    public sealed class BootstrapStartupCombatScreenFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldEnterCombatShellFromWorldMapCombatNodeFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");

                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Verdant Frontier | Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Encounter: Combat"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Vanguard 120 / 120 | Enemy Unit 75 / 75"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress: 0 / 3 toward node clear"), Is.True);
                Assert.That(ContainsText(hostObject, "Run-only skill choice"), Is.False);
                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy Unit"), Is.True);
                Assert.That(
                    ContainsText(hostObject, "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."),
                    Is.False);
                Assert.That(FindButton(hostObject, "AdvanceRunLifecycleButton").interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldEnterBulwarkRaiderCombatShellFromWorldMapPushNodeFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);
                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");

                Assert.That(ContainsText(hostObject, "Raider Trail"), Is.True);
                Assert.That(ContainsText(hostObject, "Verdant Frontier | Raider Trail"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Vanguard 120 / 120 | Bulwark Raider 105 / 105"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress: 1 / 3 toward node clear"), Is.True);
                Assert.That(ContainsText(hostObject, "Bulwark Raider"), Is.True);
                Assert.That(ContainsText(hostObject, "HP: 105 / 105 | ATK: 9"), Is.True);
                Assert.That(FindButton(hostObject, "AdvanceRunLifecycleButton").interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowSelectedCharacterOnWorldMapAndUseSelectedCharacterForRunEntry()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(ContainsText(hostObject, "Selected character: Vanguard"), Is.True);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();
                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Run-only skill choice"), Is.False);
                Assert.That(
                    ContainsText(hostObject, "Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only."),
                    Is.False);
                Assert.That(ContainsText(hostObject, "Striker"), Is.True);
                Assert.That(ContainsText(hostObject, "Verdant Frontier | Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Striker 110 / 110 | Enemy Unit 75 / 75"), Is.True);
                Assert.That(ContainsText(hostObject, "HP: 110 / 110 | ATK: 18"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldResolveCombatNodeRunWithoutManualCombatInteractionAfterNodeEntry()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Ordinary rewards: Soft currency x1, Region material x2"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward source: Frontier salvage"), Is.True);
                Assert.That(ContainsText(hostObject, "Clear spike rewards:"), Is.False);
                Assert.That(ContainsText(hostObject, "Unlock outcomes:"), Is.False);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 1 / 3; persistent +0"), Is.True);
                Assert.That(ContainsText(hostObject, "Recommended: Replay Forest Farm to keep pushing node progress."), Is.True);
                Assert.That(FindButton(hostObject, "ReplayNodeButton").interactable, Is.True);
                Assert.That(FindButton(hostObject, "ReturnToWorldMapButton").interactable, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAutosaveResolvedRunStateWhenEnteringPostRunBeforeReturnOrStop()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                Assert.That(storage.SaveCallCount, Is.EqualTo(0));

                AdvanceToPostRun(hostObject);

                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(storage.SaveCallCount, Is.EqualTo(1));
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
                Assert.That(storage.SavedGameState.OfflineProgressStableSaveAnchorState.HasStableSaveAnchor, Is.True);
                Assert.That(
                    storage.SavedGameState.OfflineProgressStableSaveAnchorState.LastStableSaveUnixTimeSeconds,
                    Is.GreaterThan(0));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                    Is.True);
                Assert.That(characterState.ProgressionRank, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldReturnToWorldMapAfterAutoResolvedCombatRun()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                ReturnToWorldMap(hostObject);

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Selected: none"), Is.True);
                Assert.That(
                    FindButton(hostObject, "EnterSelectedNodeButton").GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Replay Forest Farm"));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                    Is.True);
                Assert.That(characterState.ProgressionRank, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldQuickReplayRecentCombatNodeFromWorldMapWithoutReselectingIt()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                ReturnToWorldMap(hostObject);

                Button quickReplayButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Assert.That(quickReplayButton.interactable, Is.True);
                Assert.That(
                    quickReplayButton.GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Replay Forest Farm"));

                quickReplayButton.onClick.Invoke();

                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldKeepReplayFlowWorkingAfterResolvedPostRunAutosave()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(storage.SaveCallCount, Is.EqualTo(1));
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                Assert.That(storage.SaveCallCount, Is.EqualTo(1));

                AdvanceToPostRun(hostObject);

                Assert.That(storage.SaveCallCount, Is.EqualTo(2));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(2));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(2));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(4));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldResumeToWorldMapFromResolvedPostRunAutosaveInsteadOfReopeningRunState()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(firstHostObject, storage);

                EnterNodeFromWorldMap(firstHostObject, "region_001_node_004_Button");
                AdvanceToPostRun(firstHostObject);

                Assert.That(storage.SaveCallCount, Is.EqualTo(1));

                CreateAndInitializeBootstrap(secondHostObject, storage);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(secondHostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(ContainsText(secondHostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Current: Forest Farm (In progress) | Selected: none"), Is.True);
                Assert.That(FindButton(secondHostObject, "EnterSelectedNodeButton").interactable, Is.False);
                Assert.That(
                    FindButton(secondHostObject, "EnterSelectedNodeButton").GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Select a reachable node to enter"));
                Assert.That(ContainsText(secondHostObject, "Run finished."), Is.False);
                Assert.That(ContainsText(secondHostObject, "Run-only skill choice"), Is.False);
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency),
                    Is.EqualTo(1));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial),
                    Is.EqualTo(2));
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }

        [Test]
        public void ShouldNotResumeTemporaryRunOnlyUpgradeChoiceAfterRestartLoad()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            Assert.That(selectionService.TrySelectCharacter(seededGameState, "character_striker"), Is.True);
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(firstHostObject, storage);

                EnterNodeFromWorldMap(firstHostObject, "region_001_node_004_Button");
                Assert.That(ContainsText(firstHostObject, "Run-only skill choice"), Is.False);
                Assert.That(ContainsText(firstHostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(storage.SaveCallCount, Is.EqualTo(0));

                CreateAndInitializeBootstrap(secondHostObject, storage);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(secondHostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(ContainsText(secondHostObject, "Selected character: Striker"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Run-only skill choice"), Is.False);
                Assert.That(ContainsText(secondHostObject, "Run finished."), Is.False);

                EnterNodeFromWorldMap(secondHostObject, "region_001_node_004_Button");

                Assert.That(ContainsText(secondHostObject, "Run-only skill choice"), Is.False);
                Assert.That(
                    ContainsText(
                        secondHostObject,
                        "Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only."),
                    Is.False);
                Assert.That(ContainsText(secondHostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }

        [Test]
        public void ShouldApplyPurchasedAccountWideCombatBaselineUpgradeToCombatEntryAndReplay()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            storage.Seed(gameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");

                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "HP: 130 / 130 | ATK: 14"), Is.True);

                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "HP: 135 / 135 | ATK: 14"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowIncreasedOrdinaryRegionMaterialRewardWhenFarmYieldProjectIsPurchased()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.FarmYieldProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            storage.Seed(gameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Ordinary rewards: Soft currency x1, Region material x3"), Is.True);
                Assert.That(ContainsText(hostObject, "Clear spike rewards:"), Is.False);

                ReturnToWorldMap(hostObject);

                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(3));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(0));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldRestoreToWorldMapAndShowFarmReplayShortcutWhenFarmReplayProjectIsPurchasedForFarmReadyCurrentNode()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState gameState = CreateFarmReadyReplaySavedGameState();

            storage.Seed(gameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Current: Forest Farm (Cleared) | Selected: none"), Is.True);

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Assert.That(entryButton.interactable, Is.True);
                Assert.That(
                    entryButton.GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Replay Forest Farm"));

                entryButton.onClick.Invoke();

                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static PersistentGameState CreateFarmReadyReplaySavedGameState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();
            NodeId farmNodeId = new NodeId("region_001_node_004");
            PersistentNodeState farmNodeState = gameState.WorldState.GetOrAddNodeState(
                farmNodeId,
                unlockThreshold: 3,
                initialState: NodeState.Available);

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 3);
            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.FarmReplayProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            farmNodeState.ApplyUnlockProgress(3);
            gameState.WorldState.SetCurrentNode(farmNodeId);
            gameState.WorldState.SetLastSafeNode(farmNodeId);
            gameState.SafeResumeState.MarkWorldMap(farmNodeId);
            return gameState;
        }
    }
}

