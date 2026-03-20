using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatEntityStateTests
    {
        [Test]
        public void ShouldCreatePlayerSideCombatEntityWithAliveAndActiveDefaults()
        {
            CombatStatBlock baseStats = new CombatStatBlock(120f, 14f, 1.2f, 12f);
            CombatEntityState combatEntity = new CombatEntityState(
                new CombatEntityId("player_main"),
                "Player Unit",
                CombatSide.Player,
                baseStats);

            Assert.That(combatEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(combatEntity.DisplayName, Is.EqualTo("Player Unit"));
            Assert.That(combatEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatEntity.BaseStats, Is.EqualTo(baseStats));
            Assert.That(combatEntity.BaselineAttackSkill, Is.SameAs(CombatSkillCatalog.BasicAttack));
            Assert.That(combatEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(combatEntity.PassiveSkills, Is.Empty);
            Assert.That(combatEntity.IsAlive, Is.True);
            Assert.That(combatEntity.IsActive, Is.True);
        }

        [Test]
        public void ShouldCreateEnemySideCombatEntityWithExplicitIdentity()
        {
            CombatStatBlock baseStats = new CombatStatBlock(75f, 8f, 0.9f, 4f);
            CombatEntityState combatEntity = new CombatEntityState(
                new CombatEntityId("region_001_node_004_enemy_001"),
                "Enemy Unit",
                CombatSide.Enemy,
                baseStats);

            Assert.That(combatEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(combatEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(combatEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(combatEntity.BaseStats, Is.EqualTo(baseStats));
            Assert.That(combatEntity.IsAlive, Is.True);
            Assert.That(combatEntity.IsActive, Is.True);
        }

        [Test]
        public void ShouldCreateCombatShellContextUsingCombatEntityState()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                default);

            Assert.That(combatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(combatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(combatContext.PlayerEntity.BaselineAttackSkill.SkillId, Is.EqualTo("combat_skill_basic_attack"));
            Assert.That(combatContext.PlayerEntity.BaselineAttackSkill.Category, Is.EqualTo(CombatSkillCategory.BasicAttack));
            Assert.That(
                combatContext.PlayerEntity.BaselineAttackSkill.ActivationType,
                Is.EqualTo(CombatSkillActivationType.AutomatedInterval));
            Assert.That(
                combatContext.PlayerEntity.BaselineAttackSkill.EffectType,
                Is.EqualTo(CombatSkillEffectType.DirectDamage));
            Assert.That(combatContext.PlayerEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(combatContext.PlayerEntity.PassiveSkills, Is.Empty);
            Assert.That(combatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(combatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(combatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(combatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
            Assert.That(combatContext.EnemyEntity.BaselineAttackSkill, Is.SameAs(CombatSkillCatalog.BasicAttack));
            Assert.That(combatContext.EnemyEntity.TriggeredActiveSkill, Is.Null);
            Assert.That(combatContext.EnemyEntity.PassiveSkills, Is.Empty);
            Assert.That(combatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(combatContext.EnemyEntity.IsActive, Is.True);
        }

        [Test]
        public void ShouldApplyPurchasedAccountWideCombatBaselineEffectToPlayerCombatStats()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 10,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 0));

            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(130f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(combatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(combatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
        }

        [Test]
        public void ShouldApplyPurchasedPushOffenseProjectToPlayerCombatStats()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 4,
                    ordinaryRegionMaterialRewardBonus: 0));

            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(combatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(combatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
        }

        [Test]
        public void ShouldKeepCombatBaselineUnchangedWhenOnlyFarmYieldProjectIsPurchased()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 1));

            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(combatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(combatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(combatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
        }

        [Test]
        public void ShouldApplyResolvedPlayableCharacterIdentityToCombatContext()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                PlayableCharacterCatalog.Default,
                null,
                default);

            Assert.That(combatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
        }

        [Test]
        public void ShouldApplySecondPlayableCharacterIdentityToCombatContext()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                PlayableCharacterCatalog.Get("character_striker"),
                null,
                default);

            Assert.That(combatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_striker")));
            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Striker"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(110f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(18f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.35f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(8f));
            Assert.That(combatContext.PlayerEntity.TriggeredActiveSkill, Is.SameAs(CombatSkillCatalog.BurstStrike));
            Assert.That(combatContext.PlayerEntity.TriggeredActiveSkill.SkillId, Is.EqualTo("combat_active_burst_strike"));
            Assert.That(combatContext.PlayerEntity.TriggeredActiveSkill.Category, Is.EqualTo(CombatSkillCategory.TriggeredActive));
            Assert.That(
                combatContext.PlayerEntity.TriggeredActiveSkill.ActivationType,
                Is.EqualTo(CombatSkillActivationType.PeriodicAutoTrigger));
            Assert.That(
                combatContext.PlayerEntity.TriggeredActiveSkill.EffectType,
                Is.EqualTo(CombatSkillEffectType.DirectDamage));
            Assert.That(combatContext.PlayerEntity.PassiveSkills.Count, Is.EqualTo(1));
            Assert.That(combatContext.PlayerEntity.PassiveSkills[0].SkillId, Is.EqualTo("combat_passive_relentless_assault"));
            Assert.That(combatContext.PlayerEntity.PassiveSkills[0].Category, Is.EqualTo(CombatSkillCategory.Passive));
            Assert.That(
                combatContext.PlayerEntity.PassiveSkills[0].ActivationType,
                Is.EqualTo(CombatSkillActivationType.AlwaysOn));
            Assert.That(
                combatContext.PlayerEntity.PassiveSkills[0].EffectType,
                Is.EqualTo(CombatSkillEffectType.DirectDamageModifier));
        }

        [Test]
        public void ShouldSkipNullPassiveSkillEntriesWhenCreatingCombatEntityState()
        {
            CombatEntityState combatEntity = new CombatEntityState(
                new CombatEntityId("player_main"),
                "Player Unit",
                CombatSide.Player,
                new CombatStatBlock(100f, 10f, 1f, 0f),
                passiveSkills: new CombatSkillDefinition[]
                {
                    null,
                    CombatSkillCatalog.RelentlessAssault,
                    null,
                });

            Assert.That(combatEntity.PassiveSkills.Count, Is.EqualTo(1));
            Assert.That(combatEntity.PassiveSkills[0], Is.SameAs(CombatSkillCatalog.RelentlessAssault));
        }

        [Test]
        public void ShouldApplyPersistentCharacterProgressionRankBonusToPlayerCombatStats()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            PersistentCharacterState characterState = new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: 2,
                skillPackageId: "skill_package_vanguard_default");
            CombatShellContext combatContext = factory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                PlayableCharacterCatalog.Default,
                characterState,
                default);

            Assert.That(combatContext.PlayerEntity.DisplayName, Is.EqualTo("Vanguard"));
            Assert.That(combatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(130f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(combatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(combatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(combatContext.PlayerEntity.TriggeredActiveSkill, Is.Null);
        }
    }
}

