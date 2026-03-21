using NUnit.Framework;
using UnityEngine;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Startup
{
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

                Assert.That(ContainsText(hostObject, "Combat Shell: region_001_node_004"), Is.True);
                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy Unit"), Is.True);
                Assert.That(ContainsText(hostObject, "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."), Is.True);
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

                Assert.That(ContainsText(hostObject, "Combat Shell: region_001_node_002"), Is.True);
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
                Assert.That(ContainsText(hostObject, "Run-only skill choice"), Is.True);
                FindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId}_RunTimeSkillUpgradeButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Striker"), Is.True);
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

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Rewards gained: Soft currency x1, Region material x1"), Is.True);
                Assert.That(ContainsText(hostObject, "Milestone rewards:"), Is.False);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 1 / 3; persistent +0; route unlock No"), Is.True);
                Assert.That(FindButton(hostObject, "ReplayNodeButton").interactable, Is.True);
                Assert.That(FindButton(hostObject, "ReturnToWorldMapButton").interactable, Is.True);
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
                Assert.That(ContainsText(hostObject, "Recent node: region_001_node_004"), Is.True);
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(1));
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

                Assert.That(ContainsText(hostObject, "Rewards gained: Soft currency x1, Region material x2"), Is.True);
                Assert.That(ContainsText(hostObject, "Milestone rewards:"), Is.False);

                ReturnToWorldMap(hostObject);

                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(0));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}

