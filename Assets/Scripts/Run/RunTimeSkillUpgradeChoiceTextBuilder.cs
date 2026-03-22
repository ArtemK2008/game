using System;

namespace Survivalon.Run
{
    public static class RunTimeSkillUpgradeChoiceTextBuilder
    {
        public static string BuildPanelText(RunTimeSkillUpgradeChoiceState choiceState)
        {
            if (choiceState == null)
            {
                throw new ArgumentNullException(nameof(choiceState));
            }

            return $"<b>{choiceState.TitleDisplayName}</b>\n{choiceState.SummaryDisplayName}";
        }

        public static string BuildOptionButtonText(RunTimeSkillUpgradeChoiceOptionState optionState)
        {
            if (optionState == null)
            {
                throw new ArgumentNullException(nameof(optionState));
            }

            return
                $"<b>{optionState.DisplayName}</b>\n" +
                $"Effect: {optionState.EffectSummary}\n" +
                $"Best for: {optionState.PickHint}";
        }
    }
}
