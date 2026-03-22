using System;
using NUnit.Framework;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Проверяет общий formatter player-facing labels для shared core enum/category значений.
    /// </summary>
    public sealed class PlayerFacingCoreLabelFormatterTests
    {
        [Test]
        public void ShouldFormatSharedCoreLabelsWithCurrentShippedWording()
        {
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatResourceCategory(ResourceCategory.SoftCurrency),
                Is.EqualTo("Soft currency"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatResourceCategory(ResourceCategory.RegionMaterial),
                Is.EqualTo("Region material"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatResourceCategory(ResourceCategory.PersistentProgressionMaterial),
                Is.EqualTo("Persistent progression material"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeType(NodeType.Combat),
                Is.EqualTo("Combat"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeType(NodeType.BossOrGate),
                Is.EqualTo("Boss gate"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeType(NodeType.ServiceOrProgression),
                Is.EqualTo("Service hub"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeState(NodeState.Available),
                Is.EqualTo("Available"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeState(NodeState.InProgress),
                Is.EqualTo("In progress"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeState(NodeState.Cleared),
                Is.EqualTo("Cleared"));
            Assert.That(
                PlayerFacingCoreLabelFormatter.FormatNodeState(NodeState.Locked),
                Is.EqualTo("Locked"));
        }

        [Test]
        public void ShouldThrowForUnknownCoreEnumValues()
        {
            Assert.That(
                () => PlayerFacingCoreLabelFormatter.FormatResourceCategory((ResourceCategory)999),
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => PlayerFacingCoreLabelFormatter.FormatNodeType((NodeType)999),
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => PlayerFacingCoreLabelFormatter.FormatNodeState((NodeState)999),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
