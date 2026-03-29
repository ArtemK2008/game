using System;
using System.Reflection;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Startup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Survivalon.State.Persistence;
using Survivalon.Towns;
using Survivalon.World;
using Object = UnityEngine.Object;

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
            MemoryPersistentGameStateStorage storage,
            bool autoEnterPlayableFlow = true,
            IApplicationQuitService quitService = null,
            MemoryUserSettingsStorage settingsStorage = null,
            FakeDisplaySettingsApplier displaySettingsApplier = null,
            DateTimeOffset? sessionStartUtc = null)
        {
            BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
            bootstrapStartup.ConfigurePersistenceStorage(storage);
            bootstrapStartup.ConfigureUserSettingsStorage(settingsStorage ?? new MemoryUserSettingsStorage());
            if (quitService != null)
            {
                bootstrapStartup.ConfigureQuitService(quitService);
            }
            if (displaySettingsApplier != null)
            {
                bootstrapStartup.ConfigureDisplaySettingsApplier(displaySettingsApplier);
            }
            if (sessionStartUtc.HasValue)
            {
                bootstrapStartup.ConfigureSessionStartUtc(sessionStartUtc.Value);
            }

            InvokeAwake(bootstrapStartup);
            if (autoEnterPlayableFlow)
            {
                TryEnterPlayableFlowFromMainMenu(hostObject);
            }

            return bootstrapStartup;
        }

        protected static void StartFromMainMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "StartButton").onClick.Invoke();
        }

        protected static void ContinueFromMainMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "ContinueButton").onClick.Invoke();
        }

        protected static void OpenSettingsFromMainMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SettingsButton").onClick.Invoke();
        }

        protected static void CloseSettingsFromMainMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SettingsBackButton").onClick.Invoke();
        }

        protected static void OpenSystemMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SystemMenuButton").onClick.Invoke();
        }

        protected static void ResumeFromSystemMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SystemMenuResumeButton").onClick.Invoke();
        }

        protected static void OpenSettingsFromSystemMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SystemMenuSettingsButton").onClick.Invoke();
        }

        protected static void CloseSettingsFromSystemMenu(GameObject rootObject)
        {
            FindActiveButton(rootObject, "SystemMenuSettingsBackButton").onClick.Invoke();
        }

        protected static void EnterNodeFromWorldMap(GameObject rootObject, string nodeButtonName)
        {
            FindButton(rootObject, nodeButtonName).onClick.Invoke();
            FindButton(rootObject, "EnterSelectedNodeButton").onClick.Invoke();
        }

        protected static void ShowEnteredPlaceholderState(
            BootstrapStartup bootstrapStartup,
            NodePlaceholderState placeholderState)
        {
            MethodInfo showEnteredNodeContextMethod = typeof(BootstrapStartup).GetMethod(
                "ShowEnteredNodeContext",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(showEnteredNodeContextMethod, Is.Not.Null);

            showEnteredNodeContextMethod.Invoke(bootstrapStartup, new object[] { placeholderState });
        }

        protected static void ReturnToWorldMap(GameObject rootObject)
        {
            if (CountActiveComponents<TownServiceScreen>(rootObject) > 0)
            {
                FindActiveButton(rootObject, "ReturnToWorldMapButton").onClick.Invoke();
                return;
            }

            AdvanceToPostRun(rootObject);
            FindActiveButton(rootObject, "ReturnToWorldMapButton").onClick.Invoke();
        }

        protected static void AdvanceToPostRun(GameObject rootObject)
        {
            if (CountActiveComponents<TownServiceScreen>(rootObject) > 0)
            {
                Assert.Fail("AdvanceToPostRun should not be used while the town service screen is active.");
            }

            for (int index = 0; index < 40; index++)
            {
                Button returnToWorldButton = TryFindActiveButton(rootObject, "ReturnToWorldMapButton");
                if (returnToWorldButton != null && returnToWorldButton.interactable)
                {
                    return;
                }

                if (TryChooseFirstRunTimeSkillUpgrade(rootObject))
                {
                    continue;
                }

                Button advanceRunLifecycleButton = TryFindActiveButton(rootObject, "AdvanceRunLifecycleButton");
                Assert.That(advanceRunLifecycleButton, Is.Not.Null);

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
            int totalPlaceholderCount,
            int activeTownServiceCount = 0,
            int totalTownServiceCount = 0)
        {
            Assert.That(CountActiveComponents<WorldMapScreen>(rootObject), Is.EqualTo(activeWorldMapCount));
            Assert.That(CountActiveComponents<NodePlaceholderScreen>(rootObject), Is.EqualTo(activePlaceholderCount));
            Assert.That(CountActiveComponents<TownServiceScreen>(rootObject), Is.EqualTo(activeTownServiceCount));
            Assert.That(rootObject.GetComponentsInChildren<WorldMapScreen>(true).Length, Is.EqualTo(totalWorldMapCount));
            Assert.That(rootObject.GetComponentsInChildren<NodePlaceholderScreen>(true).Length, Is.EqualTo(totalPlaceholderCount));
            Assert.That(rootObject.GetComponentsInChildren<TownServiceScreen>(true).Length, Is.EqualTo(totalTownServiceCount));
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
            Button activeButton = TryFindActiveButton(rootObject, buttonObjectName);
            if (activeButton != null)
            {
                return activeButton;
            }

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
                if (!label.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (label.text.Contains(textFragment))
                {
                    return true;
                }
            }

            return false;
        }

        protected static Image FindImage(GameObject rootObject, string objectName)
        {
            Image[] images = rootObject.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.gameObject.name == objectName)
                {
                    return image;
                }
            }

            Assert.Fail($"Image '{objectName}' was not found.");
            return null;
        }

        protected sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            private PersistentGameState savedGameState;

            public PersistentGameState SavedGameState => savedGameState;

            public bool HasSavedState => SavedGameState != null;

            public int SaveCallCount { get; private set; }

            public void Seed(PersistentGameState gameState)
            {
                savedGameState = CloneGameState(gameState);
                SaveCallCount = 0;
            }

            public void Save(PersistentGameState gameState)
            {
                savedGameState = CloneGameState(gameState);
                SaveCallCount++;
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = savedGameState == null ? null : CloneGameState(savedGameState);
                return gameState != null;
            }

            private static PersistentGameState CloneGameState(PersistentGameState gameState)
            {
                string json = JsonUtility.ToJson(gameState);
                PersistentGameState clonedGameState = JsonUtility.FromJson<PersistentGameState>(json);
                Assert.That(clonedGameState, Is.Not.Null);
                return clonedGameState;
            }
        }

        private static void DestroyCurrentEventSystem()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        private static bool TryChooseFirstRunTimeSkillUpgrade(GameObject rootObject)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (!button.gameObject.activeInHierarchy ||
                    !button.gameObject.name.EndsWith("_RunTimeSkillUpgradeButton"))
                {
                    continue;
                }

                button.onClick.Invoke();
                return true;
            }

            return false;
        }

        private static Button FindActiveButton(GameObject rootObject, string buttonObjectName)
        {
            Button button = TryFindActiveButton(rootObject, buttonObjectName);
            if (button != null)
            {
                return button;
            }

            Assert.Fail($"Active button '{buttonObjectName}' was not found.");
            return null;
        }

        private static Button TryFindActiveButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName && button.gameObject.activeInHierarchy)
                {
                    return button;
                }
            }

            return null;
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

        private static void TryEnterPlayableFlowFromMainMenu(GameObject rootObject)
        {
            if (CountActiveComponents<StartupPlaceholderView>(rootObject) == 0)
            {
                return;
            }

            Button continueButton = TryFindActiveButton(rootObject, "ContinueButton");
            if (continueButton != null && continueButton.interactable)
            {
                continueButton.onClick.Invoke();
                return;
            }

            Button startButton = TryFindActiveButton(rootObject, "StartButton");
            if (startButton != null && startButton.interactable)
            {
                startButton.onClick.Invoke();
            }
        }

        protected sealed class MemoryUserSettingsStorage : IUserSettingsStorage
        {
            private UserSettingsState savedSettingsState;

            public UserSettingsState SavedSettingsState => savedSettingsState;

            public int SaveCallCount { get; private set; }

            public void Seed(UserSettingsState settingsState)
            {
                savedSettingsState = CloneSettingsState(settingsState);
                SaveCallCount = 0;
            }

            public void Save(UserSettingsState settingsState)
            {
                savedSettingsState = CloneSettingsState(settingsState);
                SaveCallCount++;
            }

            public bool TryLoad(out UserSettingsState settingsState)
            {
                settingsState = savedSettingsState == null ? null : CloneSettingsState(savedSettingsState);
                return settingsState != null;
            }

            private static UserSettingsState CloneSettingsState(UserSettingsState settingsState)
            {
                if (settingsState == null)
                {
                    return null;
                }

                string json = JsonUtility.ToJson(settingsState);
                UserSettingsState clonedSettingsState = JsonUtility.FromJson<UserSettingsState>(json);
                Assert.That(clonedSettingsState, Is.Not.Null);
                return clonedSettingsState;
            }
        }

        protected sealed class FakeDisplaySettingsApplier : IDisplaySettingsApplier
        {
            public bool LastAppliedFullscreen { get; private set; }

            public int ApplyCallCount { get; private set; }

            public void Apply(bool useFullscreen)
            {
                LastAppliedFullscreen = useFullscreen;
                ApplyCallCount++;
            }
        }
    }
}

