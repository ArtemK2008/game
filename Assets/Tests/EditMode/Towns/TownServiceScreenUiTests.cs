using NUnit.Framework;
using System.Collections.Generic;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode.Towns
{
    /// <summary>
    /// Проверяет town/service screen UI после переноса runtime character services и options в Characters.
    /// </summary>
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
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 2);

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
                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward focus: Persistent progression gains"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward source: Cavern relic caches"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy emphasis: Gate guardians"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Material power path:"), Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "Already affordable projects: Combat Baseline Project, Farm Yield Project"),
                    Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "New project targets after refinement: Push Offense Project, Boss Salvage Project"),
                    Is.True);
                Assert.That(ContainsText(hostObject, "Build preparation"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned package: Standard Guard"), Is.True);
                Assert.That(hostObject.GetComponentsInChildren<ScrollRect>(true).Length, Is.EqualTo(1));
                Assert.That(TryFindGameObject(hostObject, "ContentViewport"), Is.Not.Null);
                Assert.That(TryFindGameObject(hostObject, "Content"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "PushOffenseProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "BossSalvageProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "FarmReplayProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "RegionMaterialRefinement_ConversionButton"), Is.Not.Null);
                Assert.That(
                    TryFindButton(
                        hostObject,
                        $"TownService_{PlayableCharacterSkillPackageIds.VanguardDefault}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(
                    TryFindButton(
                        hostObject,
                        $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(
                    TryFindButton(
                        hostObject,
                        $"TownService_{GearIds.TrainingBlade}_GearButton"),
                    Is.Not.Null);
                Assert.That(
                    TryFindButton(
                        hostObject,
                        $"TownService_{GearIds.GuardCharm}_GearButton"),
                    Is.Not.Null);
                Assert.That(TryFindButton(hostObject, "AdvanceRunLifecycleButton"), Is.Null);
                Assert.That(TryFindButton(hostObject, "ReplayNodeButton"), Is.Null);

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
        public void Show_ShouldConvertAffordableRegionMaterialAndRefreshVisibleProgressionState()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceConversionInteractionService interactionService =
                new TownServiceConversionInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => { },
                    () => { },
                    conversionInteractionService: interactionService);

                Button conversionButton = FindButton(hostObject, "RegionMaterialRefinement_ConversionButton");

                Assert.That(conversionButton.interactable, Is.True);
                Assert.That(
                    FindButtonLabel(hostObject, "RegionMaterialRefinement_ConversionButton"),
                    Is.EqualTo("Run Region Material Refinement"));

                conversionButton.onClick.Invoke();

                Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
                Assert.That(
                    gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(1));
                Assert.That(storage.SaveCallCount, Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Region material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "Persistent progression material: 1"), Is.True);
                Assert.That(ContainsText(hostObject, "Material power path:"), Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "Already affordable projects: Combat Baseline Project, Farm Yield Project"),
                    Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "New project targets after refinement: Push Offense Project, Boss Salvage Project"),
                    Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "- Region Material Refinement | Region material x3 -> Persistent progression material x1 | Need 3 more"),
                    Is.True);
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState, Is.Not.SameAs(gameState));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(1));
                Assert.That(FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").interactable, Is.False);
                Assert.That(
                    FindButtonLabel(hostObject, "RegionMaterialRefinement_ConversionButton"),
                    Is.EqualTo("Region Material Refinement Unavailable"));
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
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 2);

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
                Button bossSalvageButton = FindButton(hostObject, "BossSalvageProject_PurchaseUpgradeButton");

                Assert.That(combatBaselineButton.interactable, Is.True);
                Assert.That(pushOffenseButton.interactable, Is.False);
                Assert.That(farmYieldButton.interactable, Is.True);
                Assert.That(bossSalvageButton.interactable, Is.False);
                Assert.That(FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").interactable, Is.False);

                combatBaselineButton.onClick.Invoke();

                Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Persistent progression material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "Region material: 2"), Is.True);
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
                Assert.That(FindButton(hostObject, "BossSalvageProject_PurchaseUpgradeButton").interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldAssignBuildPreparationOptionsAndRefreshVisibleBuildSummary()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceBuildPreparationInteractionService interactionService =
                new TownServiceBuildPreparationInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 2);

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => { },
                    () => { },
                    buildPreparationInteractionService: interactionService);

                FindButton(
                    hostObject,
                    $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton")
                    .onClick.Invoke();
                FindButton(hostObject, $"TownService_{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"TownService_{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                Assert.That(
                    gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState),
                    Is.True);
                Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                        out EquippedGearState equippedPrimaryGearState),
                    Is.True);
                Assert.That(equippedPrimaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState equippedSupportGearState),
                    Is.True);
                Assert.That(equippedSupportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
                Assert.That(ContainsText(hostObject, "Assigned package: Burst Drill"), Is.True);
                Assert.That(ContainsText(hostObject, "Primary gear: Training Blade"), Is.True);
                Assert.That(ContainsText(hostObject, "Support gear: Guard Charm"), Is.True);
                Assert.That(
                    FindButtonLabel(
                        hostObject,
                        $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton"),
                    Is.EqualTo("Assigned package: Burst Drill"));
                Assert.That(
                    FindButtonLabel(hostObject, $"TownService_{GearIds.TrainingBlade}_GearButton"),
                    Is.EqualTo("Unequip primary: Training Blade"));
                Assert.That(
                    FindButtonLabel(hostObject, $"TownService_{GearIds.GuardCharm}_GearButton"),
                    Is.EqualTo("Unequip support: Guard Charm"));
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState(
                        "character_vanguard",
                        out PersistentCharacterState savedVanguardState),
                    Is.True);
                Assert.That(savedVanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                        out EquippedGearState savedPrimaryGearState),
                    Is.True);
                Assert.That(savedPrimaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState savedSupportGearState),
                    Is.True);
                Assert.That(savedSupportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestConfirmFeedbackForAcceptedTownActions()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => { },
                    () => { },
                    progressionInteractionService: new TownServiceProgressionInteractionService(persistenceService),
                    conversionInteractionService: new TownServiceConversionInteractionService(persistenceService),
                    buildPreparationInteractionService: new TownServiceBuildPreparationInteractionService(persistenceService),
                    feedbackSoundRequested: requestedSounds.Add);

                FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton").onClick.Invoke();
                FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").onClick.Invoke();
                FindButton(
                    hostObject,
                    $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton")
                    .onClick.Invoke();
                FindButton(hostObject, $"TownService_{GearIds.TrainingBlade}_GearButton").onClick.Invoke();

                CollectionAssert.AreEqual(
                    new[]
                    {
                        UiSystemFeedbackSoundId.UiConfirm,
                        UiSystemFeedbackSoundId.UiConfirm,
                        UiSystemFeedbackSoundId.UiConfirm,
                        UiSystemFeedbackSoundId.UiConfirm,
                    },
                    requestedSounds);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestErrorFeedbackForRejectedTownActions()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    gameState,
                    () => { },
                    () => { },
                    feedbackSoundRequested: requestedSounds.Add);

                FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton").onClick.Invoke();
                FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").onClick.Invoke();

                CollectionAssert.AreEqual(
                    new[]
                    {
                        UiSystemFeedbackSoundId.UiError,
                        UiSystemFeedbackSoundId.UiError,
                    },
                    requestedSounds);
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

            public int SaveCallCount { get; private set; }

            public void Save(PersistentGameState gameState)
            {
                SavedGameState = CloneGameState(gameState);
                SaveCallCount++;
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = SavedGameState == null ? null : CloneGameState(SavedGameState);
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
    }
}
