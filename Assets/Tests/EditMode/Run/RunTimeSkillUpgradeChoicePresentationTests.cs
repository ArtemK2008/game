using System;
using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Run;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunTimeSkillUpgradeChoicePresentationTests
    {
        [Test]
        public void Resolve_ShouldBuildReadableBurstStrikeChoiceState()
        {
            RunTimeSkillUpgradeChoiceStateResolver resolver = new RunTimeSkillUpgradeChoiceStateResolver();

            RunTimeSkillUpgradeChoiceState choiceState = resolver.Resolve(
                CombatRunTimeSkillUpgradeCatalog.GetTriggeredActiveSkillUpgradeOptions(CombatSkillCatalog.BurstStrike));

            Assert.That(choiceState.TitleDisplayName, Is.EqualTo("Run-only skill choice"));
            Assert.That(
                choiceState.SummaryDisplayName,
                Is.EqualTo("Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only."));
            Assert.That(choiceState.Options, Has.Count.EqualTo(2));
            Assert.That(choiceState.Options[0].UpgradeId, Is.EqualTo(CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId));
            Assert.That(choiceState.Options[0].EffectSummary, Is.EqualTo("Burst Strike triggers faster during this run."));
            Assert.That(choiceState.Options[0].PickHint, Is.EqualTo("Steadier burst pressure."));
            Assert.That(choiceState.Options[1].UpgradeId, Is.EqualTo(CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId));
            Assert.That(choiceState.Options[1].EffectSummary, Is.EqualTo("Burst Strike hits harder during this run."));
            Assert.That(choiceState.Options[1].PickHint, Is.EqualTo("Bigger damage spikes."));
        }

        [Test]
        public void Resolve_ShouldRejectMissingUpgradeOptions()
        {
            RunTimeSkillUpgradeChoiceStateResolver resolver = new RunTimeSkillUpgradeChoiceStateResolver();

            Assert.That(
                () => resolver.Resolve(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("upgradeOptions"));
        }

        [Test]
        public void BuildPanelText_ShouldFormatReadableChoiceHeader()
        {
            string panelText = RunTimeSkillUpgradeChoiceTextBuilder.BuildPanelText(
                new RunTimeSkillUpgradeChoiceState(
                    "Run-only skill choice",
                    "Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only.",
                    Array.Empty<RunTimeSkillUpgradeChoiceOptionState>()));

            Assert.That(
                panelText,
                Is.EqualTo(
                    "<b>Run-only skill choice</b>\n" +
                    "Choose 1 Burst Strike upgrade before auto-battle starts. This choice lasts for the current run only."));
        }

        [Test]
        public void BuildOptionButtonText_ShouldFormatEffectSummaryAndPickHint()
        {
            string optionText = RunTimeSkillUpgradeChoiceTextBuilder.BuildOptionButtonText(
                new RunTimeSkillUpgradeChoiceOptionState(
                    CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId,
                    "Burst Tempo",
                    "Burst Strike triggers faster during this run.",
                    "Steadier burst pressure."));

            Assert.That(
                optionText,
                Is.EqualTo(
                    "<b>Burst Tempo</b>\n" +
                    "Effect: Burst Strike triggers faster during this run.\n" +
                    "Best for: Steadier burst pressure."));
        }
    }
}
