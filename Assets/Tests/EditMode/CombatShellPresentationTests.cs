using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatShellPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldMatchExistingOngoingSummaryFormatting()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            string summaryText = CombatShellTextBuilder.BuildSummaryText(encounterState);

            Assert.That(summaryText, Is.EqualTo(
                "Elapsed: 0s | Outcome: Ongoing\n" +
                "Targeting: Player Unit -> Enemy Unit; Enemy Unit -> Player Unit"));
        }

        [Test]
        public void BuildSummaryText_ShouldShowResolvedOutcomeWhenCombatEnds()
        {
            CombatEncounterState encounterState = CreateEncounterState();
            encounterState.Resolve(CombatEncounterOutcome.PlayerVictory);

            string summaryText = CombatShellTextBuilder.BuildSummaryText(encounterState);

            Assert.That(summaryText, Is.EqualTo(
                "Elapsed: 0s | Outcome: PlayerVictory\n" +
                "Targeting: Player Unit -> Enemy Unit; Enemy Unit -> Player Unit"));
        }

        [Test]
        public void BuildEntityCardText_ShouldMatchExistingStatFormatting()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            string playerCardText = CombatShellTextBuilder.BuildEntityCardText(encounterState.PlayerEntity);

            Assert.That(playerCardText, Is.EqualTo(
                "Player Unit\n" +
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
                "HP: 0 / 75 | ATK: 8\n" +
                $"Rate: {0.9f.ToString("0.##")}/s | DEF: 4"));
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

        private static CombatEncounterState CreateEncounterState()
        {
            CombatShellContext combatContext = new CombatShellContextFactory().Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState());
            return new CombatEncounterState(combatContext);
        }
    }
}
