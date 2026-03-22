using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Startup;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;
using Survivalon.World;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapStartupTownServiceFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldOpenTownServiceScreenForServiceNodeAndShowProgressionAndBuildSections()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");

                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Build preparation"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned package: Standard Guard"), Is.True);
                Assert.That(ContainsText(hostObject, "Current build changes still happen on the world map in this MVP."), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldFallbackToGenericPlaceholderWhenServiceNodeHasNoTownServiceContext()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = CreateAndInitializeBootstrap(hostObject, storage);

                ShowEnteredPlaceholderState(
                    bootstrapStartup,
                    NodePlaceholderTestData.CreateServicePlaceholderState());

                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Run Shell: region_002_node_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPurchaseUpgradeFromTownServiceScreenAndPersistImmediately()
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

                Assert.That(ContainsText(hostObject, "Persistent progression material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "- Combat Baseline Project | Cost: Persistent progression material x1 | Purchased"), Is.True);
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
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
    }
}
