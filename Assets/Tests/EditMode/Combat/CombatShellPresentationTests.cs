using System;
using NUnit.Framework;
using UnityEngine;
using Survivalon.Combat;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatShellPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldRejectMissingEncounterState()
        {
            Assert.That(
                () => CombatShellTextBuilder.BuildSummaryText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("combatEncounterState"));
        }

        [Test]
        public void BuildSummaryText_ShouldMatchExistingOngoingSummaryFormatting()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            string summaryText = CombatShellTextBuilder.BuildSummaryText(encounterState);

            Assert.That(summaryText, Is.EqualTo(
                "Elapsed: 0s | Outcome: Ongoing\n" +
                "Targeting: Vanguard -> Enemy Unit; Enemy Unit -> Vanguard"));
        }

        [Test]
        public void BuildSummaryText_ShouldShowResolvedOutcomeWhenCombatEnds()
        {
            CombatEncounterState encounterState = CreateEncounterState();
            encounterState.Resolve(CombatEncounterOutcome.PlayerVictory);

            string summaryText = CombatShellTextBuilder.BuildSummaryText(encounterState);

            Assert.That(summaryText, Is.EqualTo(
                "Elapsed: 0s | Outcome: PlayerVictory\n" +
                "Targeting: Vanguard -> Enemy Unit; Enemy Unit -> Vanguard"));
        }

        [Test]
        public void BuildEntityCardText_ShouldMatchExistingStatFormatting()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            string playerCardText = CombatShellTextBuilder.BuildEntityCardText(encounterState.PlayerEntity);

            Assert.That(playerCardText, Is.EqualTo(
                "Vanguard\n" +
                "Player | Alive: Yes | Act: Yes\n" +
                "HP: 120 / 120 | ATK: 14\n" +
                $"Rate: {1.2f.ToString("0.##")}/s | DEF: 12"));
        }

        [Test]
        public void BuildEntityCardText_ShouldReflectCombatDamageAndDefeatState()
        {
            CombatEncounterState encounterState = CreateEncounterState();
            encounterState.EnemyEntity.ApplyDamage(encounterState.EnemyEntity.MaxHealth);

            string enemyCardText = CombatShellTextBuilder.BuildEntityCardText(encounterState.EnemyEntity);

            Assert.That(enemyCardText, Is.EqualTo(
                "Enemy Unit\n" +
                "Enemy | Alive: No | Act: No\n" +
                "HP: 0 / 75 | ATK: 7\n" +
                $"Rate: {1.25f.ToString("0.##")}/s | DEF: 2"));
        }

        [Test]
        public void BuildEntityCardText_ShouldRejectMissingCombatEntity()
        {
            Assert.That(
                () => CombatShellTextBuilder.BuildEntityCardText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("combatEntity"));
        }

        [Test]
        public void ResolveEntityCardColor_ShouldMatchExistingSideColors()
        {
            Assert.That(
                CombatShellStateResolver.ResolveEntityCardColor(CombatSide.Player),
                Is.EqualTo(new Color(0.18f, 0.38f, 0.68f, 1f)));
            Assert.That(
                CombatShellStateResolver.ResolveEntityCardColor(CombatSide.Enemy),
                Is.EqualTo(new Color(0.62f, 0.22f, 0.22f, 1f)));
        }

        [Test]
        public void ResolveEntityCardColor_ShouldRejectUnsupportedSide()
        {
            Assert.That(
                () => CombatShellStateResolver.ResolveEntityCardColor((CombatSide)999),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("side"));
        }

        private static CombatEncounterState CreateEncounterState()
        {
            CombatShellContext combatContext = new CombatShellContextFactory().Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                default);
            return new CombatEncounterState(combatContext);
        }
    }
}

