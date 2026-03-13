using System;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatStatCalculatorTests
    {
        [Test]
        public void ShouldPreserveBaseStatValuesWhenCombatStatBlockIsCreated()
        {
            CombatStatBlock stats = new CombatStatBlock(150f, 18f, 1.5f, 20f);

            Assert.That(stats.MaxHealth, Is.EqualTo(150f));
            Assert.That(stats.AttackPower, Is.EqualTo(18f));
            Assert.That(stats.AttackRate, Is.EqualTo(1.5f));
            Assert.That(stats.Defense, Is.EqualTo(20f));
        }

        [Test]
        public void ShouldCalculateAttackIntervalFromAttackRate()
        {
            CombatStatBlock stats = new CombatStatBlock(100f, 10f, 2f, 0f);

            float attackIntervalSeconds = CombatStatCalculator.CalculateAttackIntervalSeconds(stats);

            Assert.That(attackIntervalSeconds, Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void ShouldReduceIncomingDamagePredictablyAsDefenseIncreases()
        {
            CombatStatBlock lowDefenseStats = new CombatStatBlock(100f, 12f, 1f, 0f);
            CombatStatBlock highDefenseStats = new CombatStatBlock(100f, 12f, 1f, 50f);

            float lowDefenseDamage = CombatStatCalculator.CalculateMitigatedDamage(50f, lowDefenseStats);
            float highDefenseDamage = CombatStatCalculator.CalculateMitigatedDamage(50f, highDefenseStats);

            Assert.That(lowDefenseDamage, Is.EqualTo(50f).Within(0.0001f));
            Assert.That(highDefenseDamage, Is.EqualTo(33.3333f).Within(0.001f));
            Assert.That(highDefenseDamage, Is.LessThan(lowDefenseDamage));
        }

        [Test]
        public void ShouldCalculateBoundedMitigationRatioFromDefense()
        {
            CombatStatBlock stats = new CombatStatBlock(100f, 10f, 1f, 75f);

            float mitigationRatio = CombatStatCalculator.CalculateMitigationRatio(stats);

            Assert.That(mitigationRatio, Is.EqualTo(75f / 175f).Within(0.0001f));
            Assert.That(mitigationRatio, Is.GreaterThanOrEqualTo(0f));
            Assert.That(mitigationRatio, Is.LessThan(1f));
        }

        [Test]
        public void ShouldRejectInvalidCombatStatValues()
        {
            Assert.That(() => new CombatStatBlock(0f, 10f, 1f, 0f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new CombatStatBlock(10f, -1f, 1f, 0f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new CombatStatBlock(10f, 1f, 0f, 0f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new CombatStatBlock(10f, 1f, 1f, -1f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => CombatStatCalculator.CalculateMitigatedDamage(-1f, new CombatStatBlock(10f, 1f, 1f, 0f)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
