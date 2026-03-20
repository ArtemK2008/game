using System;

namespace Survivalon.Combat
{
    public sealed class CombatSkillExecutionRequest
    {
        public CombatSkillExecutionRequest(
            CombatSkillDefinition skillDefinition,
            CombatEntityRuntimeState sourceEntity,
            CombatEntityRuntimeState targetEntity)
        {
            SkillDefinition = skillDefinition ?? throw new ArgumentNullException(nameof(skillDefinition));
            SourceEntity = sourceEntity ?? throw new ArgumentNullException(nameof(sourceEntity));
            TargetEntity = targetEntity ?? throw new ArgumentNullException(nameof(targetEntity));
        }

        public CombatSkillDefinition SkillDefinition { get; }

        public CombatEntityRuntimeState SourceEntity { get; }

        public CombatEntityRuntimeState TargetEntity { get; }
    }
}
