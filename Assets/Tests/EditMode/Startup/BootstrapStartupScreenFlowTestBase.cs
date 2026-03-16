using System.Reflection;
using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Startup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public abstract class BootstrapStartupScreenFlowTestBase
    {
        [SetUp]
        public void SetUp()
        {
            DestroyCurrentEventSystem();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyCurrentEventSystem();
        }

        protected static BootstrapStartup CreateAndInitializeBootstrap(
            GameObject hostObject,
            MemoryPersistentGameStateStorage storage)
        {
            BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
            bootstrapStartup.ConfigurePersistenceStorage(storage);
            InvokeAwake(bootstrapStartup);
            return bootstrapStartup;
        }

        protected static void EnterNodeFromWorldMap(GameObject rootObject, string nodeButtonName)
        {
            FindButton(rootObject, nodeButtonName).onClick.Invoke();
            FindButton(rootObject, "EnterSelectedNodeButton").onClick.Invoke();
        }

        protected static void ReturnToWorldMap(GameObject rootObject)
        {
            AdvanceToPostRun(rootObject);
            FindButton(rootObject, "ReturnToWorldMapButton").onClick.Invoke();
        }

        protected static void AdvanceToPostRun(GameObject rootObject)
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

        protected static void AssertScreenCounts(
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

        protected static int CountActiveComponents<T>(GameObject rootObject) where T : Component
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

        protected static Button FindButton(GameObject rootObject, string buttonObjectName)
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

        protected static bool ContainsText(GameObject rootObject, string textFragment)
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

        protected sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public bool HasSavedState => SavedGameState != null;

            public void Seed(PersistentGameState gameState)
            {
                SavedGameState = gameState;
            }

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

        private static void DestroyCurrentEventSystem()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
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

        private static void InvokeAwake(BootstrapStartup bootstrapStartup)
        {
            MethodInfo awakeMethod = typeof(BootstrapStartup).GetMethod(
                "Awake",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(awakeMethod, Is.Not.Null);

            awakeMethod.Invoke(bootstrapStartup, null);
        }
    }
}
