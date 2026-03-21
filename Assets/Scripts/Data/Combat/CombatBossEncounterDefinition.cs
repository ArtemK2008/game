using System;

namespace Survivalon.Data.Combat
{
    public sealed class CombatBossEncounterDefinition : CombatEncounterDefinition
    {
        public CombatBossEncounterDefinition(
            string encounterId,
            string bossId,
            CombatBossRoleType bossRoleType,
            CombatEnemyProfile bossProfile)
            : base(encounterId, CombatEncounterType.Boss)
        {
            if (string.IsNullOrWhiteSpace(bossId))
            {
                throw new ArgumentException("Boss id cannot be null or whitespace.", nameof(bossId));
            }

            if (bossProfile == null)
            {
                throw new ArgumentNullException(nameof(bossProfile));
            }

            if (bossProfile.HostileEntityType != CombatHostileEntityType.Boss)
            {
                throw new ArgumentException(
                    "Boss encounter requires a boss hostile profile.",
                    nameof(bossProfile));
            }

            BossId = bossId;
            BossRoleType = bossRoleType;
            BossProfile = bossProfile;
        }

        public string BossId { get; }

        public CombatBossRoleType BossRoleType { get; }

        public CombatEnemyProfile BossProfile { get; }

        public override CombatEnemyProfile PrimaryEnemyProfile => BossProfile;
    }
}
