using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode.Towns
{
    public sealed class TownServiceScreenUiTests
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
        public void Show_ShouldCreateDistinctNonCombatServiceScreenAndInvokeActions()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            bool returnedToWorld = false;
            bool stoppedSession = false;
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => returnedToWorld = true,
                    () => stoppedSession = true);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
                Assert.That(hostObject.GetComponent<GraphicRaycaster>(), Is.Not.Null);

                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                Assert.That(eventSystem, Is.Not.Null);
                Assert.That(eventSystem.gameObject.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);

                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Build preparation"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned package: Standard Guard"), Is.True);
                Assert.That(hostObject.GetComponentsInChildren<ScrollRect>(true).Length, Is.EqualTo(1));
                Assert.That(TryFindGameObject(hostObject, "ContentViewport"), Is.Not.Null);
                Assert.That(TryFindGameObject(hostObject, "Content"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "PushOffenseProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "AdvanceRunLifecycleButton"), Is.Null);
                Assert.That(TryFindButton(hostObject, "ReplayNodeButton"), Is.Null);
                Assert.That(TryFindButton(hostObject, "AssignGearButton"), Is.Null);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(returnedToWorld, Is.True);
                Assert.That(stoppedSession, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldPurchaseAffordableUpgradeAndRefreshVisibleProgressionState()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceProgressionInteractionService interactionService =
                new TownServiceProgressionInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => { },
                    () => { },
                    progressionInteractionService: interactionService);

                Button combatBaselineButton = FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton");
                Button pushOffenseButton = FindButton(hostObject, "PushOffenseProject_PurchaseUpgradeButton");
                Button farmYieldButton = FindButton(hostObject, "FarmYieldProject_PurchaseUpgradeButton");

                Assert.That(combatBaselineButton.interactable, Is.True);
                Assert.That(pushOffenseButton.interactable, Is.False);
                Assert.That(farmYieldButton.interactable, Is.True);

                combatBaselineButton.onClick.Invoke();

                Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Persistent progression material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "- Combat Baseline Project | Cost: Persistent progression material x1 | Purchased"), Is.True);
                Assert.That(ContainsText(hostObject, "- Farm Yield Project | Cost: Persistent progression material x1 | Need 1 more"), Is.True);
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(
                    storage.SavedGameState.ProgressionState.TryGetEntry(
                        "account_wide_combat_baseline_project",
                        out ProgressionEntryState progressionEntry),
                    Is.True);
                Assert.That(progressionEntry.IsUnlocked, Is.True);
                Assert.That(FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton").interactable, Is.False);
                Assert.That(FindButtonLabel(hostObject, "CombatBaselineProject_PurchaseUpgradeButton"), Is.EqualTo("Combat Baseline Project Purchased"));
                Assert.That(FindButton(hostObject, "FarmYieldProject_PurchaseUpgradeButton").interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
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

        private static GameObject TryFindGameObject(GameObject rootObject, string objectName)
        {
            Transform[] transforms = rootObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform currentTransform in transforms)
            {
                if (currentTransform.gameObject.name == objectName)
                {
                    return currentTransform.gameObject;
                }
            }

            return null;
        }

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button button = TryFindButton(rootObject, buttonObjectName);
            if (button != null)
            {
                return button;
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
        }

        private static Button TryFindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            return null;
        }

        private static string FindButtonLabel(GameObject rootObject, string buttonObjectName)
        {
            Button button = FindButton(rootObject, buttonObjectName);
            Text buttonLabel = button.GetComponentInChildren<Text>(true);
            Assert.That(buttonLabel, Is.Not.Null);
            return buttonLabel.text;
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

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
