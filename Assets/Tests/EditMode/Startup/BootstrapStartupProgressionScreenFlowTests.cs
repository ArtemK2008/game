using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapStartupProgressionScreenFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldAccumulatePersistentNodeProgressAcrossRepeatedSuccessfulCombatRuns()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                ReturnToWorldMap(hostObject);

                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(2));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldMarkCombatNodeClearedAtThresholdAndReflectItOnWorldMap()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(3));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(nodeState.State, Is.EqualTo(NodeState.Cleared));

                Button clearedNodeButton = FindButton(hostObject, "region_001_node_004_Button");
                Text clearedNodeLabel = clearedNodeButton.GetComponentInChildren<Text>(true);

                Assert.That(clearedNodeLabel, Is.Not.Null);
                Assert.That(clearedNodeLabel.text, Does.Contain("State: Cleared"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldUnlockNextConnectedNodeAfterPushNodeClearsAndReturnToWorld()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 2 / 3; persistent +0; route unlock No"), Is.True);

                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Milestone rewards: Persistent progression material x1"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 3 / 3; persistent +0; route unlock Yes"), Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
                Assert.That(pushNodeState.State, Is.EqualTo(NodeState.Cleared));
                Assert.That(pushNodeState.UnlockProgress, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
                Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Available));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(1));

                Button gateNodeButton = FindButton(hostObject, "region_001_node_003_Button");
                Text gateNodeLabel = gateNodeButton.GetComponentInChildren<Text>(true);

                Assert.That(gateNodeButton.interactable, Is.True);
                Assert.That(gateNodeLabel, Is.Not.Null);
                Assert.That(gateNodeLabel.text, Does.Contain("State: Available"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAllowReenteringClearedNodeWithoutRegressingStateOrUnlocks()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_001_node_001_Button");
                ReturnToWorldMap(hostObject);

                Button clearedPushNodeButton = FindButton(hostObject, "region_001_node_002_Button");
                Text clearedPushNodeLabel = clearedPushNodeButton.GetComponentInChildren<Text>(true);
                Assert.That(clearedPushNodeButton.interactable, Is.True);
                Assert.That(clearedPushNodeLabel, Is.Not.Null);
                Assert.That(clearedPushNodeLabel.text, Does.Contain("State: Cleared"));

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 3 / 3; persistent +0; route unlock No"), Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState clearedPushNodeState), Is.True);
                Assert.That(clearedPushNodeState.State, Is.EqualTo(NodeState.Cleared));
                Assert.That(clearedPushNodeState.UnlockProgress, Is.EqualTo(3));
                Assert.That(clearedPushNodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState unlockedGateNodeState), Is.True);
                Assert.That(unlockedGateNodeState.State, Is.EqualTo(NodeState.Available));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAllowEnteringClearedFarmNodeFromWorldMapEvenWhenItIsNotNormallyReachable()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            NodeId clearedFarmNodeId = new NodeId("region_001_node_002");
            NodeId unlockedNextNodeId = new NodeId("region_001_node_003");
            NodeId currentNodeId = new NodeId("region_002_node_001");
            NodeId lastSafeNodeId = new NodeId("region_001_node_001");

            try
            {
                Assert.That(gameState.WorldState.TryGetNodeState(clearedFarmNodeId, out PersistentNodeState clearedFarmNodeState), Is.True);
                clearedFarmNodeState.ApplyUnlockProgress(2);
                Assert.That(clearedFarmNodeState.State, Is.EqualTo(NodeState.Cleared));
                new NextNodeUnlockService().UnlockConnectedNodesWhenSourceClears(worldGraph, gameState.WorldState, clearedFarmNodeId);
                gameState.WorldState.SetCurrentNode(currentNodeId);
                gameState.WorldState.SetLastSafeNode(lastSafeNodeId);
                gameState.WorldState.ReplaceReachableNodes(new[] { lastSafeNodeId });
                storage.Seed(gameState);

                CreateAndInitializeBootstrap(hostObject, storage);

                Button clearedFarmNodeButton = FindButton(hostObject, "region_001_node_002_Button");
                Text clearedFarmNodeLabel = clearedFarmNodeButton.GetComponentInChildren<Text>(true);

                Assert.That(clearedFarmNodeButton.interactable, Is.True);
                Assert.That(clearedFarmNodeLabel, Is.Not.Null);
                Assert.That(clearedFarmNodeLabel.text, Does.Contain("State: Cleared"));
                Assert.That(ContainsText(hostObject, "Current: Cavern Service Hub"), Is.True);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 3 / 3; persistent +0; route unlock No"), Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(clearedFarmNodeId, out PersistentNodeState savedClearedFarmNodeState), Is.True);
                Assert.That(savedClearedFarmNodeState.State, Is.EqualTo(NodeState.Cleared));
                Assert.That(savedClearedFarmNodeState.UnlockProgress, Is.EqualTo(3));
                Assert.That(savedClearedFarmNodeState.UnlockThreshold, Is.EqualTo(3));
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(unlockedNextNodeId, out PersistentNodeState savedUnlockedNodeState), Is.True);
                Assert.That(savedUnlockedNodeState.State, Is.EqualTo(NodeState.Available));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldUnlockCavernGateAfterForestGateBossDefeatAndShowBossGateSummary()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_001_node_003_Button");
                FindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss rewards: Persistent progression material x2"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss gate unlock: Cavern gate opened"), Is.True);
                Assert.That(
                    ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 1 / 3; persistent +0; route unlock No"),
                    Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(
                    storage.SavedGameState.WorldState.TryGetNodeState(BootstrapWorldScenario.CavernGateNodeId, out PersistentNodeState cavernGateNodeState),
                    Is.True);
                Assert.That(cavernGateNodeState.State, Is.EqualTo(NodeState.Available));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(3));

                Button cavernGateButton = FindButton(hostObject, "region_002_node_002_Button");
                Text cavernGateLabel = cavernGateButton.GetComponentInChildren<Text>(true);

                Assert.That(cavernGateButton.interactable, Is.True);
                Assert.That(cavernGateLabel, Is.Not.Null);
                Assert.That(cavernGateLabel.text, Does.Contain("State: Available"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldKeepVerdantFrontierFarmUsefulAfterCavernGateOpens()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_001_node_003_Button");
                FindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                int regionMaterialBeforeFarm = storage.SavedGameState.ResourceBalances.GetAmount(
                    ResourceCategory.RegionMaterial);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");

                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Encounter: Combat"), Is.True);

                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Rewards gained: Soft currency x1, Region material x2"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward source: Frontier salvage"), Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Selected: none"), Is.True);
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial),
                    Is.EqualTo(regionMaterialBeforeFarm + 2));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldGrantHigherBossProgressionRewardAtCavernGateThanForestGate()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_001_node_003_Button");
                FindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss rewards: Persistent progression material x2"), Is.True);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                EnterNodeFromWorldMap(hostObject, "region_002_node_002_Button");
                FindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward source: Cavern relic caches"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss rewards: Persistent progression material x3"), Is.True);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(6));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}

