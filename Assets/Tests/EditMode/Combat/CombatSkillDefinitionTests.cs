using System.Reflection;
using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatSkillDefinitionTests
    {
        [Test]
        public void ShouldKeepCombatSkillDefinitionLimitedToGenericSkillMetadata()
        {
            CombatSkillDefinition passiveSkill = CombatSkillCatalog.RelentlessAssault;
            CombatSkillDefinition activeSkill = CombatSkillCatalog.BurstStrike;

            Assert.That(passiveSkill.SkillId, Is.EqualTo("combat_passive_relentless_assault"));
            Assert.That(passiveSkill.DisplayName, Is.EqualTo("Relentless Assault"));
            Assert.That(passiveSkill.Category, Is.EqualTo(CombatSkillCategory.Passive));
            Assert.That(passiveSkill.ActivationType, Is.EqualTo(CombatSkillActivationType.AlwaysOn));
            Assert.That(passiveSkill.EffectType, Is.EqualTo(CombatSkillEffectType.DirectDamageModifier));
            Assert.That(activeSkill.SkillId, Is.EqualTo("combat_active_burst_strike"));
            Assert.That(activeSkill.DisplayName, Is.EqualTo("Burst Strike"));
            Assert.That(activeSkill.Category, Is.EqualTo(CombatSkillCategory.TriggeredActive));
            Assert.That(activeSkill.ActivationType, Is.EqualTo(CombatSkillActivationType.PeriodicAutoTrigger));
            Assert.That(activeSkill.EffectType, Is.EqualTo(CombatSkillEffectType.DirectDamage));
            Assert.That(
                typeof(CombatSkillDefinition).GetProperty("DirectDamageMultiplier", BindingFlags.Public | BindingFlags.Instance),
                Is.Null);
        }
    }
}
