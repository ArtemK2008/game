using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatPassiveSkillEffectResolverTests
    {
        [Test]
        public void ShouldResolveBaseOutgoingDirectDamageMultiplierWhenCombatEntityHasNoPassiveSkills()
        {
            CombatPassiveSkillEffectResolver resolver = new CombatPassiveSkillEffectResolver();
            CombatEntityRuntimeState sourceEntity = CreateRuntimeState();

            float directDamageMultiplier = resolver.ResolveOutgoingDirectDamageMultiplier(
                sourceEntity,
                CombatSkillCatalog.BasicAttack);

            Assert.That(directDamageMultiplier, Is.EqualTo(1f));
        }

        [Test]
        public void ShouldResolveOutgoingDirectDamageMultiplierFromAlwaysOnPassiveSkills()
        {
            CombatPassiveSkillEffectResolver resolver = new CombatPassiveSkillEffectResolver();
            CombatEntityRuntimeState sourceEntity = CreateRuntimeState(CombatSkillCatalog.RelentlessAssault);

            float directDamageMultiplier = resolver.ResolveOutgoingDirectDamageMultiplier(
                sourceEntity,
                CombatSkillCatalog.BasicAttack);

            Assert.That(directDamageMultiplier, Is.EqualTo(1.2f));
        }

        [Test]
        public void ShouldIgnoreNullPassiveSkillEntriesWhenResolvingOutgoingDirectDamageMultiplier()
        {
            CombatPassiveSkillEffectResolver resolver = new CombatPassiveSkillEffectResolver();
            CombatEntityRuntimeState sourceEntity = CreateRuntimeState(null, CombatSkillCatalog.RelentlessAssault, null);

            float directDamageMultiplier = resolver.ResolveOutgoingDirectDamageMultiplier(
                sourceEntity,
                CombatSkillCatalog.BasicAttack);

            Assert.That(directDamageMultiplier, Is.EqualTo(1.2f));
        }

        private static CombatEntityRuntimeState CreateRuntimeState(params CombatSkillDefinition[] passiveSkills)
        {
            return new CombatEntityRuntimeState(
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    new CombatStatBlock(100f, 10f, 1f, 0f),
                    passiveSkills: passiveSkills));
        }
    }
}
