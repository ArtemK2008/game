using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Run
{
    public sealed class RunTimeSkillUpgradeAutoPickResolver
    {
        public CombatRunTimeSkillUpgradeOption ResolveAutomaticFlowSelection(
            IReadOnlyList<CombatRunTimeSkillUpgradeOption> availableOptions)
        {
            if (availableOptions == null)
            {
                throw new ArgumentNullException(nameof(availableOptions));
            }

            for (int index = 0; index < availableOptions.Count; index++)
            {
                CombatRunTimeSkillUpgradeOption availableOption = availableOptions[index];
                if (availableOption == null)
                {
                    continue;
                }

                if (availableOption.UpgradeId == CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId)
                {
                    return availableOption;
                }
            }

            return null;
        }
    }
}
