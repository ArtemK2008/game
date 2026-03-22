using NUnit.Framework;
using Survivalon.Startup;
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
    }
}
