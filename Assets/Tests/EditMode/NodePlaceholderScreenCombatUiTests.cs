using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode
{
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

                Assert.That(ContainsText(hostObject, "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."), Is.True);
                Assert.That(ContainsText(hostObject, "Elapsed: 0s | Outcome: Ongoing"), Is.True);
                Assert.That(ContainsText(hostObject, "Targeting: Player Unit -> Enemy Unit; Enemy Unit -> Player Unit"), Is.True);
                Assert.That(ContainsText(hostObject, "Player Unit"), Is.True);
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

                Assert.That(ContainsText(hostObject, "Targeting: Player Unit -> Enemy Unit; Enemy Unit -> Player Unit"), Is.True);
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

                Assert.That(ContainsText(hostObject, "Post-run summary is active. Replay, return to the world map, or stop the session."), Is.True);
                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
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
            PersistentWorldState worldState = new PersistentWorldState();

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreateCombatPlaceholderState(),
                    runResult => { },
                    runResult => { },
                    worldState);

                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReplayNodeButton").onClick.Invoke();

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");
                Assert.That(advanceRunLifecycleButton.GetComponentInChildren<Text>(true).text, Is.EqualTo("Combat Auto-Running"));
                Assert.That(advanceRunLifecycleButton.interactable, Is.False);

                AdvanceToPostRun(hostObject);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Succeeded"), Is.True);
                Assert.That(ContainsText(hostObject, "Node progress total: 2 / 3"), Is.True);
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
                    worldState);

                AutoAdvanceCombat(hostObject, 64, 0.25f);

                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);
                Assert.That(ContainsText(hostObject, "Resolution: Failed"), Is.True);
                Assert.That(ContainsText(hostObject, "Node progress total: 0 / 3"), Is.True);
                Assert.That(ContainsText(hostObject, "Node progress delta: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "Route unlock changed: No"), Is.True);
                Assert.That(FindButton(hostObject, "ReturnToWorldMapButton").interactable, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}
