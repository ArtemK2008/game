using NUnit.Framework;
using Survivalon.Startup;
using UnityEngine;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.Towns;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapStartupScreenFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldReuseSingleWorldMapPlaceholderAndTownServiceScreensAcrossEnterAndReturnFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                AssertScreenCounts(hostObject, 1, 0, 1, 0, 0, 0);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                AssertScreenCounts(hostObject, 0, 0, 1, 0, 1, 1);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                AssertScreenCounts(hostObject, 1, 0, 1, 0, 0, 1);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1, 0, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1, 0, 1);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPersistSafeResumeContextWhenReturningToWorldFromTownServiceScreen()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton").onClick.Invoke();
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
                Assert.That(
                    storage.SavedGameState.ProgressionState.TryGetEntry(
                        "account_wide_combat_baseline_project",
                        out ProgressionEntryState progressionEntry),
                    Is.True);
                Assert.That(progressionEntry.IsUnlocked, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowRecentNodeContextAfterReturningToWorldMap()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Recent: region_002_node_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Push target: region_002_node_001"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowStartupPlaceholderWhenTownServiceStopIsRequested()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));

                StartupPlaceholderView placeholderView = hostObject.GetComponentInChildren<StartupPlaceholderView>(true);
                Assert.That(placeholderView, Is.Not.Null);
                Assert.That(ContainsText(hostObject, "Main Menu Placeholder"), Is.True);
                Assert.That(ContainsText(hostObject, "Session stopped at a safe world or service point."), Is.True);
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowReadableWorldStateSummaryOnInitialWorldMap()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier | Region: region_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Current node: region_001_node_002 (InProgress) | Selected: none"), Is.True);
                Assert.That(ContainsText(hostObject, "Forward routes: region_001_node_004, region_002_node_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Backtrack / farm: region_001_node_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Blocked links: region_001_node_003"), Is.True);
                Assert.That(ContainsText(hostObject, "State legend: Available = enterable"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}

