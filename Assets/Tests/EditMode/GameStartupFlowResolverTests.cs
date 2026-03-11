using System;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class GameStartupFlowResolverTests
    {
        [Test]
        public void ShouldRouteToMainMenuPlaceholderWhenNoResumeContextExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentWorldState worldState = new PersistentWorldState();

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(worldState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
        }

        [Test]
        public void ShouldRouteToWorldViewPlaceholderWhenLastSafeNodeExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetLastSafeNode(new NodeId("region_001_node_001"));

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(worldState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
        }

        [Test]
        public void ShouldRouteToWorldViewPlaceholderWhenCurrentNodeExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(new NodeId("region_001_node_001"));

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(worldState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
        }

        [Test]
        public void ShouldRejectMissingWorldState()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();

            TestDelegate action = () => resolver.ResolveInitialEntryTarget(null);

            Assert.That(action, Throws.ArgumentNullException);
        }
    }
}
