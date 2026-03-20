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

            Assert.That(passiveSkill.SkillId, Is.EqualTo("combat_passive_relentless_assault"));
            Assert.That(passiveSkill.DisplayName, Is.EqualTo("Relentless Assault"));
            Assert.That(passiveSkill.Category, Is.EqualTo(CombatSkillCategory.Passive));
            Assert.That(passiveSkill.ActivationType, Is.EqualTo(CombatSkillActivationType.AlwaysOn));
            Assert.That(passiveSkill.EffectType, Is.EqualTo(CombatSkillEffectType.DirectDamageModifier));
            Assert.That(
                typeof(CombatSkillDefinition).GetProperty("DirectDamageMultiplier", BindingFlags.Public | BindingFlags.Instance),
                Is.Null);
        }
    }
}
