using System.Reflection;
using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode
{
    public sealed class BootstrapStartupScreenFlowTests
    {
        [SetUp]
        public void SetUp()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [Test]
        public void ShouldReuseSingleWorldMapAndPlaceholderScreenAcrossEnterAndReturnFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                AssertScreenCounts(hostObject, 1, 0, 1, 0);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPersistSafeResumeContextWhenReturningToWorldFromPostRun()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
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
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                ReturnToWorldMap(hostObject);

                Assert.That(ContainsText(hostObject, "Recent node: region_002_node_001"), Is.True);
                Assert.That(ContainsText(hostObject, "Recent push target: region_002_node_001"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldEnterCombatShellFromWorldMapCombatNodeFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");

                Assert.That(ContainsText(hostObject, "Combat Shell: region_001_node_004"), Is.True);
                Assert.That(ContainsText(hostObject, "Player Unit"), Is.True);
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
        public void ShouldResolveCombatNodeRunWithoutManualCombatInteractionAfterNodeEntry()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Node progress total: 1 / 3"), Is.True);
                Assert.That(ContainsText(hostObject, "Node progress delta: 1"), Is.True);
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
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_001_node_004_Button");
                ReturnToWorldMap(hostObject);

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Recent node: region_001_node_004"), Is.True);
                Assert.That(storage.SavedGameState.WorldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
                Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
                Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAccumulatePersistentNodeProgressAcrossRepeatedSuccessfulCombatRuns()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

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
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

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
        public void ShouldShowStartupPlaceholderWhenPostRunStopIsRequested()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                bootstrapStartup.ConfigurePersistenceStorage(storage);
                InvokeAwake(bootstrapStartup);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));

                StartupPlaceholderView placeholderView = hostObject.GetComponentInChildren<StartupPlaceholderView>(true);
                Assert.That(placeholderView, Is.Not.Null);
                Assert.That(placeholderView.ActiveTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static void EnterNodeFromWorldMap(GameObject rootObject, string nodeButtonName)
        {
            FindButton(rootObject, nodeButtonName).onClick.Invoke();
            FindButton(rootObject, "EnterSelectedNodeButton").onClick.Invoke();
        }

        private static void ReturnToWorldMap(GameObject rootObject)
        {
            AdvanceToPostRun(rootObject);
            FindButton(rootObject, "ReturnToWorldMapButton").onClick.Invoke();
        }

        private static void AdvanceToPostRun(GameObject rootObject)
        {
            Button advanceRunLifecycleButton = FindButton(rootObject, "AdvanceRunLifecycleButton");
            Button returnToWorldButton = FindButton(rootObject, "ReturnToWorldMapButton");

            for (int index = 0; index < 40; index++)
            {
                if (returnToWorldButton.interactable)
                {
                    return;
                }

                if (!advanceRunLifecycleButton.interactable)
                {
                    InvokeRuntimeAdvance(rootObject, 0.25f);
                    continue;
                }

                advanceRunLifecycleButton.onClick.Invoke();
            }

            Assert.Fail("AdvanceToPostRun did not reach the post-run state within the expected number of steps.");
        }

        private static void InvokeRuntimeAdvance(GameObject rootObject, float elapsedSeconds)
        {
            NodePlaceholderScreen placeholderScreen = rootObject.GetComponentInChildren<NodePlaceholderScreen>(true);
            Assert.That(placeholderScreen, Is.Not.Null);

            MethodInfo autoAdvanceMethod = typeof(NodePlaceholderScreen).GetMethod(
                "TryAdvanceRuntimeTime",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(autoAdvanceMethod, Is.Not.Null);

            autoAdvanceMethod.Invoke(placeholderScreen, new object[] { elapsedSeconds });
        }

        private static void AssertScreenCounts(
            GameObject rootObject,
            int activeWorldMapCount,
            int activePlaceholderCount,
            int totalWorldMapCount,
            int totalPlaceholderCount)
        {
            Assert.That(CountActiveComponents<WorldMapScreen>(rootObject), Is.EqualTo(activeWorldMapCount));
            Assert.That(CountActiveComponents<NodePlaceholderScreen>(rootObject), Is.EqualTo(activePlaceholderCount));
            Assert.That(rootObject.GetComponentsInChildren<WorldMapScreen>(true).Length, Is.EqualTo(totalWorldMapCount));
            Assert.That(rootObject.GetComponentsInChildren<NodePlaceholderScreen>(true).Length, Is.EqualTo(totalPlaceholderCount));
        }

        private static int CountActiveComponents<T>(GameObject rootObject) where T : Component
        {
            int activeCount = 0;
            foreach (T component in rootObject.GetComponentsInChildren<T>(true))
            {
                if (component.gameObject.activeInHierarchy)
                {
                    activeCount++;
                }
            }

            return activeCount;
        }

        private static void InvokeAwake(BootstrapStartup bootstrapStartup)
        {
            MethodInfo awakeMethod = typeof(BootstrapStartup).GetMethod(
                "Awake",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(awakeMethod, Is.Not.Null);

            awakeMethod.Invoke(bootstrapStartup, null);
        }

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
        }

        private static bool ContainsText(GameObject rootObject, string textFragment)
        {
            Text[] labels = rootObject.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                if (label.text.Contains(textFragment))
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public bool HasSavedState => SavedGameState != null;

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
