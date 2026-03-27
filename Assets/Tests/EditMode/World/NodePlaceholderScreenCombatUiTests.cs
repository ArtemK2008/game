using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Data.Characters;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.World;

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
                Assert.That(FindRectTransform(hostObject, "RunTimeSkillUpgradePanel").gameObject.activeSelf, Is.False);
                Assert.That(
                    TryFindButton(hostObject, $"{CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId}_RunTimeSkillUpgradeButton"),
                    Is.Null);
                Assert.That(ContainsText(hostObject, "Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Player | Alive: Yes | Act: Yes"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy Unit"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy | Alive: Yes | Act: Yes"), Is.True);
                Assert.That(advanceButtonText.text, Is.EqualTo("Combat Auto-Running"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
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

