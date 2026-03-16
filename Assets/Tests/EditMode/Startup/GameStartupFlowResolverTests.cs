using System;
using NUnit.Framework;
using Survivalon.Runtime.Startup;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class GameStartupFlowResolverTests
    {
        [Test]
        public void ShouldRouteToMainMenuPlaceholderWhenNoResumeContextExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentGameState gameState = new PersistentGameState();

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(gameState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
        }

        [Test]
        public void ShouldRouteToWorldViewPlaceholderWhenLastSafeNodeExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetLastSafeNode(new NodeId("region_001_node_001"));

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(gameState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
        }

        [Test]
        public void ShouldRouteToWorldViewPlaceholderWhenCurrentNodeExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_001_node_001"));

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(gameState);

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
        }

        [Test]
        public void ShouldRouteToWorldViewPlaceholderWhenSafeResumeTargetExists()
        {
            GameStartupFlowResolver resolver = new GameStartupFlowResolver();
            PersistentGameState gameState = new PersistentGameState();
            gameState.SafeResumeState.MarkWorldMap(new NodeId("region_001_node_002"));

            StartupEntryTarget entryTarget = resolver.ResolveInitialEntryTarget(gameState);

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
