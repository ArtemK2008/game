using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.World;
using System.Collections.Generic;

namespace Survivalon.Tests.EditMode.World
{
    /// <summary>
    /// Проверяет combat UI placeholder-экрана узла с runtime character services из Characters.
    /// </summary>
    public sealed class NodePlaceholderScreenCombatUiTests : NodePlaceholderScreenUiTestBase
    {
        [Test]
        public void Show_ShouldDisplayCombatEntitiesForCombatNodeWithoutManualStart()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");
                Text advanceButtonText = advanceRunLifecycleButton.GetComponentInChildren<Text>(true);
                Assert.That(advanceButtonText.text, Is.EqualTo("Combat Auto-Running"));

                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Verdant Frontier | Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Encounter: Combat"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Vanguard 120 / 120 | Enemy Unit 75 / 75"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress: 0 / 3 toward node clear"), Is.True);
                Assert.That(
                    ContainsText(hostObject, "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."),
                    Is.False);
                Assert.That(
                    FindRectTransform(hostObject, "Summary").GetComponent<LayoutElement>().preferredHeight,
                    Is.EqualTo(54f));
                Assert.That(FindRectTransform(hostObject, "RunTimeSkillUpgradePanel").gameObject.activeSelf, Is.False);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);
                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Player | Alive: Yes | Act: Yes"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy Unit"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy | Alive: Yes | Act: Yes"), Is.True);
                Assert.That(FindImage(hostObject, "PlayerCombatEntitySprite").sprite, Is.Not.Null);
                Assert.That(FindImage(hostObject, "EnemyCombatEntitySprite").sprite, Is.Not.Null);
                Assert.That(FindImage(hostObject, "CombatShellBackgroundArt").sprite, Is.Not.Null);
                Assert.That(FindImage(hostObject, "PlayerCombatEntitySprite").preserveAspect, Is.True);
                Assert.That(FindImage(hostObject, "EnemyCombatEntitySprite").preserveAspect, Is.True);
                Assert.That(
                    FindImage(hostObject, "PlayerCombatEntitySprite").sprite,
                    Is.Not.SameAs(FindImage(hostObject, "EnemyCombatEntitySprite").sprite));
                Assert.That(advanceButtonText.text, Is.EqualTo("Combat Auto-Running"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldUseDistinctCombatBackgroundsForVerdantFrontierEchoCavernsAndSunscorchRuins()
        {
            GameObject frontierHostObject = new GameObject("NodePlaceholderHost_Frontier");
            GameObject cavernHostObject = new GameObject("NodePlaceholderHost_Cavern");
            GameObject sunscorchHostObject = new GameObject("NodePlaceholderHost_Sunscorch");

            try
            {
                NodePlaceholderScreen frontierPlaceholderScreen = frontierHostObject.AddComponent<NodePlaceholderScreen>();
                frontierPlaceholderScreen.Show(
                    CreateWorldGraph(),
                    NodePlaceholderTestData.CreateFrontierFarmPlaceholderState(),
                    runResult => { },
                    runResult => { });

                NodePlaceholderScreen cavernPlaceholderScreen = cavernHostObject.AddComponent<NodePlaceholderScreen>();
                cavernPlaceholderScreen.Show(
                    CreateWorldGraph(),
                    NodePlaceholderTestData.CreateCavernGateBossPlaceholderState(),
                    runResult => { },
                    runResult => { });

                NodePlaceholderScreen sunscorchPlaceholderScreen = sunscorchHostObject.AddComponent<NodePlaceholderScreen>();
                sunscorchPlaceholderScreen.Show(
                    CreateWorldGraph(),
                    NodePlaceholderTestData.CreateSunscorchCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                Sprite frontierBackground = FindImage(frontierHostObject, "CombatShellBackgroundArt").sprite;
                Sprite cavernBackground = FindImage(cavernHostObject, "CombatShellBackgroundArt").sprite;
                Sprite sunscorchBackground = FindImage(sunscorchHostObject, "CombatShellBackgroundArt").sprite;

                Assert.That(frontierBackground, Is.Not.Null);
                Assert.That(cavernBackground, Is.Not.Null);
                Assert.That(sunscorchBackground, Is.Not.Null);
                Assert.That(frontierBackground, Is.Not.SameAs(cavernBackground));
                Assert.That(frontierBackground, Is.Not.SameAs(sunscorchBackground));
                Assert.That(cavernBackground, Is.Not.SameAs(sunscorchBackground));
                Assert.That(
                    FindImage(frontierHostObject, "EnemyCombatEntitySprite").sprite,
                    Is.Not.SameAs(FindImage(sunscorchHostObject, "EnemyCombatEntitySprite").sprite));
                Assert.That(ContainsText(frontierHostObject, "Verdant Frontier | Forest Farm"), Is.True);
                Assert.That(ContainsText(cavernHostObject, "Echo Caverns | Cavern Gate"), Is.True);
                Assert.That(ContainsText(sunscorchHostObject, "Sunscorch Ruins | Scorched Approach"), Is.True);
                Assert.That(ContainsText(sunscorchHostObject, "Ruin Sentinel"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(frontierHostObject);
                Object.DestroyImmediate(cavernHostObject);
                Object.DestroyImmediate(sunscorchHostObject);
            }
        }

        [Test]
        public void Show_ShouldAutoPickBaselineRunTimeSkillUpgradeAndAutoStartCombatWhenTriggeredActiveSkillIsPresent()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    RunPersistentContext.FromGameState(gameState));

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");
                Assert.That(advanceRunLifecycleButton.GetComponentInChildren<Text>(true).text, Is.EqualTo("Combat Auto-Running"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);
                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Encounter: Combat"), Is.True);
                Assert.That(ContainsText(hostObject, "Run-only skill choice"), Is.False);
                Assert.That(
                    ContainsText(hostObject, "Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only."),
                    Is.False);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(
                    FindRectTransform(hostObject, "RunTimeSkillUpgradePanel").GetComponent<LayoutElement>(),
                    Is.Null);
                Assert.That(ContainsText(hostObject, "Striker"), Is.True);
                Assert.That(ContainsText(hostObject, "Verdant Frontier | Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Run state: Auto-battle active | Outcome: Ongoing | Elapsed: 0s"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Striker 110 / 110 | Enemy Unit 75 / 75"), Is.True);
                Assert.That(FindImage(hostObject, "PlayerCombatEntitySprite").sprite, Is.Not.Null);
                Assert.That(
                    FindImage(hostObject, "PlayerCombatEntitySprite").sprite.name,
                    Does.Contain("idle"));
                Assert.That(ContainsText(hostObject, "Progress: 0 / 3 toward node clear"), Is.True);
                Assert.That(
                    FindRectTransform(hostObject, "RunTimeSkillUpgradePanel").gameObject.activeSelf,
                    Is.False);
                ForceUiLayout(hostObject);
                Assert.That(
                    RectangleContains(
                        FindRectTransform(hostObject, "Panel"),
                        FindRectTransform(hostObject, "CombatShellView")),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldReflectEnemyHostilityDamageDuringAutomaticCombat()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                AutoAdvanceCombat(hostObject, 5, 0.25f);

                string runHudSummary = FindTextInObject(hostObject, "CombatShellSummary");

                Assert.That(runHudSummary, Does.Contain("Run state: Auto-battle active"));
                Assert.That(runHudSummary, Does.Not.Contain("Health: Vanguard 120 / 120 | Enemy Unit 75 / 75"));
                Assert.That(FindTextInObject(hostObject, "PlayerCombatEntity"), Does.Contain("Player | Alive: Yes | Act: Yes"));
                Assert.That(FindTextInObject(hostObject, "PlayerCombatEntity"), Does.Not.Contain("HP: 120 / 120"));
                Assert.That(FindTextInObject(hostObject, "EnemyCombatEntity"), Does.Contain("Enemy | Alive: Yes | Act: Yes"));
                Assert.That(FindImage(hostObject, "PlayerCombatEntitySprite").sprite, Is.Not.Null);
                Assert.That(FindImage(hostObject, "EnemyCombatEntitySprite").sprite, Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepCombatShellSeparatedFromAdvanceButtonForCombatNode()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                ForceUiLayout(hostObject);

                RectTransform combatShellRect = FindRectTransform(hostObject, "CombatShellView");
                RectTransform combatShellSummaryRect = FindRectTransform(hostObject, "CombatShellSummary");
                RectTransform combatEntityRowRect = FindRectTransform(hostObject, "CombatEntityRow");
                RectTransform playerCardRect = FindRectTransform(hostObject, "PlayerCombatEntity");
                RectTransform enemyCardRect = FindRectTransform(hostObject, "EnemyCombatEntity");
                RectTransform advanceButtonRect = FindRectTransform(hostObject, "AdvanceRunLifecycleButton");
                RectTransform mainPanelRect = FindRectTransform(hostObject, "Panel");

                Assert.That(RectangleContains(combatShellRect, combatShellSummaryRect), Is.True);
                Assert.That(RectangleContains(combatShellRect, combatEntityRowRect), Is.True);
                Assert.That(RectangleContains(combatEntityRowRect, playerCardRect), Is.True);
                Assert.That(RectangleContains(combatEntityRowRect, enemyCardRect), Is.True);
                Assert.That(RectangleContains(playerCardRect, FindRectTransform(hostObject, "PlayerCombatEntitySprite")), Is.True);
                Assert.That(RectangleContains(enemyCardRect, FindRectTransform(hostObject, "EnemyCombatEntitySprite")), Is.True);
                Assert.That(RectanglesOverlap(combatEntityRowRect, advanceButtonRect), Is.False);
                Assert.That(RectanglesOverlap(playerCardRect, advanceButtonRect), Is.False);
                Assert.That(RectanglesOverlap(enemyCardRect, advanceButtonRect), Is.False);
                Assert.That(RectangleContains(mainPanelRect, combatShellRect), Is.True);
                Assert.That(RectangleContains(mainPanelRect, advanceButtonRect), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldReachPostRunForCombatNodeWithoutManualCombatInput()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                AutoAdvanceCombat(hostObject, 24, 0.25f);

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");

                Assert.That(ContainsText(hostObject, "Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Recommended: Replay Forest Farm for more Region material."), Is.True);
                Assert.That(advanceRunLifecycleButton.GetComponentInChildren<Text>(true).text, Is.EqualTo("Run Lifecycle Complete"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);
                Assert.That(FindButton(hostObject, "ReplayNodeButton").interactable, Is.True);
                Assert.That(FindButton(hostObject, "ReturnToWorldMapButton").interactable, Is.True);
                Assert.That(FindButton(hostObject, "StopSessionButton").interactable, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldDisplayCombatEffectCueDuringAutomaticCombat()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                bool observedEffectCue = false;
                for (int index = 0; index < 8; index++)
                {
                    AutoAdvanceCombat(hostObject, 1, 0.25f);
                    if (FindImage(hostObject, "PlayerCombatEffectArt").enabled ||
                        FindImage(hostObject, "EnemyCombatEffectArt").enabled ||
                        FindImage(hostObject, "CombatShellCenterEffectArt").enabled)
                    {
                        observedEffectCue = true;
                        break;
                    }
                }

                Assert.That(observedEffectCue, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestBaselineCombatFeedbackDuringAutomaticCombat()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            List<CombatFeedbackSoundId> requestedSounds = new List<CombatFeedbackSoundId>();

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    combatFeedbackSoundRequested: requestedSounds.Add);

                AutoAdvanceCombat(hostObject, 5, 0.25f);

                Assert.That(requestedSounds, Does.Contain(CombatFeedbackSoundId.PlayerAttack));
                Assert.That(requestedSounds, Does.Contain(CombatFeedbackSoundId.EnemyAttack));
                Assert.That(requestedSounds, Has.No.Member(CombatFeedbackSoundId.PlayerHit));
                Assert.That(requestedSounds, Has.No.Member(CombatFeedbackSoundId.EnemyHit));
                Assert.That(requestedSounds, Has.No.Member(CombatFeedbackSoundId.DangerLowHealth));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestBurstStrikeInLiveCombatFlow()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            List<CombatFeedbackSoundId> requestedSounds = new List<CombatFeedbackSoundId>();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateBossCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    RunPersistentContext.FromGameState(gameState),
                    combatFeedbackSoundRequested: requestedSounds.Add);

                AutoAdvanceCombat(hostObject, 64, 0.25f);

                Assert.That(requestedSounds, Does.Contain(CombatFeedbackSoundId.BurstStrike));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldDisplayBurstStrikeCenterEffectInLiveCombatFlow()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateBossCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    RunPersistentContext.FromGameState(gameState));

                bool observedBurstEffect = false;
                for (int index = 0; index < 64; index++)
                {
                    AutoAdvanceCombat(hostObject, 1, 0.25f);
                    if (FindImage(hostObject, "CombatShellCenterEffectArt").enabled)
                    {
                        observedBurstEffect = true;
                        break;
                    }
                }

                Assert.That(observedBurstEffect, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldDisplaySunscorchCombatWithRuinSentinelFamily()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost_SunscorchCombat");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    NodePlaceholderTestData.CreateSunscorchCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                Assert.That(ContainsText(hostObject, "Sunscorch Ruins | Scorched Approach"), Is.True);
                Assert.That(ContainsText(hostObject, "Health: Vanguard 120 / 120 | Ruin Sentinel 95 / 95"), Is.True);
                Assert.That(FindImage(hostObject, "EnemyCombatEntitySprite").sprite, Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestConfirmFeedbackForReplayReturnAndStopActions()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    feedbackSoundRequested: requestedSounds.Add);

                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();

                CollectionAssert.AreEqual(
                    new[]
                    {
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
        public void Show_ShouldDisableSystemExitDuringActiveCombatAndEnableItAfterResolvedPostRun()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            bool stopRequested = false;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => stopRequested = true);

                FindButton(hostObject, "SystemMenuButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "System Menu"), Is.True);
                Assert.That(FindButton(hostObject, "SystemMenuExitButton").interactable, Is.False);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "Exit is unavailable until you reach a safe world, service, or resolved post-run context."),
                    Is.True);

                FindButton(hostObject, "SystemMenuResumeButton").onClick.Invoke();
                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "SystemMenuButton").onClick.Invoke();

                Assert.That(FindButton(hostObject, "SystemMenuExitButton").interactable, Is.True);

                FindButton(hostObject, "SystemMenuExitButton").onClick.Invoke();

                Assert.That(stopRequested, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldPauseAutomaticCombatProgressWhileSystemMenuIsVisible()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { });

                string initialSummary = FindTextInObject(hostObject, "CombatShellSummary");

                FindButton(hostObject, "SystemMenuButton").onClick.Invoke();
                InvokeRuntimeFrame(hostObject, 2.5f);

                Assert.That(FindTextInObject(hostObject, "CombatShellSummary"), Is.EqualTo(initialSummary));

                FindButton(hostObject, "SystemMenuResumeButton").onClick.Invoke();
                AutoAdvanceCombat(hostObject, 1, 2.5f);

                Assert.That(FindTextInObject(hostObject, "CombatShellSummary"), Is.Not.EqualTo(initialSummary));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldExposeRealSettingsSurfaceFromSystemMenuAndPropagateSettingsChange()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            UserSettingsState receivedSettingsState = null;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    settingsChanged: settingsState => receivedSettingsState = settingsState);

                FindButton(hostObject, "SystemMenuButton").onClick.Invoke();
                FindButton(hostObject, "SystemMenuSettingsButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Settings"), Is.True);
                Assert.That(ContainsText(hostObject, "Master volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Music volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "SFX volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Display mode: Windowed"), Is.True);
                Assert.That(FindButton(hostObject, "SystemMenuSettingsBackButton").gameObject.activeInHierarchy, Is.True);

                FindButton(hostObject, "SfxVolumeDecreaseButton").onClick.Invoke();

                Assert.That(receivedSettingsState, Is.Not.Null);
                Assert.That(receivedSettingsState.SfxVolume, Is.EqualTo(0.9f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestBossRewardAndUnlockFeedbackWhenBossPostRunIsPresented()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService(selectionService);
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade),
                Is.True);
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.GuardCharm),
                Is.True);

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateBossCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    RunPersistentContext.FromGameState(gameState),
                    feedbackSoundRequested: requestedSounds.Add);

                AdvanceToPostRun(hostObject);

                CollectionAssert.AreEqual(
                    new[]
                    {
                        UiSystemFeedbackSoundId.StateBossReward,
                        UiSystemFeedbackSoundId.StateRouteUnlock,
                    },
                    requestedSounds);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldAutoReplayCombatNodeWithoutManualCombatRestart()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    RunPersistentContext.FromGameState(gameState));

                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");
                Assert.That(advanceRunLifecycleButton.GetComponentInChildren<Text>(true).text, Is.EqualTo("Combat Auto-Running"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);
                Assert.That(ContainsText(hostObject, "Run-only skill choice"), Is.False);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);

                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress changes: node +1 this run; tracked total 2 / 3; persistent +0"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldReachFailedPostRunForHostileBossCombatWithoutManualCombatInput()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            PersistentWorldState worldState = new PersistentWorldState();

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateBossCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    new RunPersistentContext(persistentWorldState: worldState));

                Assert.That(ContainsText(hostObject, "Frontier Gate"), Is.True);
                Assert.That(ContainsText(hostObject, "Encounter: Gate boss"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss stakes: Gate clear, Boss rewards, Gear reward"), Is.True);
                Assert.That(ContainsText(hostObject, "Boss encounter | Verdant Frontier | Frontier Gate"), Is.True);
                Assert.That(FindImage(hostObject, "EnemyCombatEntitySprite").sprite, Is.Not.Null);
                Assert.That(
                    ContainsText(hostObject, "Boss role: Gate boss | Stakes: Gate clear, Boss rewards, Gear reward"),
                    Is.True);
                Assert.That(
                    FindRectTransform(hostObject, "Summary").GetComponent<LayoutElement>().preferredHeight,
                    Is.EqualTo(82f));

                AutoAdvanceCombat(hostObject, 64, 0.25f);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Failed"), Is.True);
                Assert.That(ContainsText(hostObject, "Progress changes: node +0 this run; tracked total 0 / 3; persistent +0"), Is.True);
                Assert.That(ContainsText(hostObject, "Recommended: Replay Frontier Gate for another attempt."), Is.True);
                Assert.That(FindButton(hostObject, "ReturnToWorldMapButton").interactable, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}

