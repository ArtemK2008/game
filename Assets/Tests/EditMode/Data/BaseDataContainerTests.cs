using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.Data.Rewards;
using Survivalon.Runtime.Data.World;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class BaseDataContainerTests
    {
        [Test]
        public void ShouldExposeTypedRegionIdFromSerializedValue()
        {
            RegionDefinition regionDefinition = ScriptableObject.CreateInstance<RegionDefinition>();
            SetPrivateField(regionDefinition, "regionIdValue", "region_001");

            Assert.That(regionDefinition.RegionId, Is.EqualTo(new RegionId("region_001")));

            Object.DestroyImmediate(regionDefinition);
        }

        [Test]
        public void ShouldCreateNodeDefinitionWithEmptyConnectionAndPrerequisiteLists()
        {
            NodeDefinition nodeDefinition = ScriptableObject.CreateInstance<NodeDefinition>();
            SetPrivateField(nodeDefinition, "nodeIdValue", "region_001_node_001");

            Assert.That(nodeDefinition.NodeId, Is.EqualTo(new NodeId("region_001_node_001")));
            Assert.That(nodeDefinition.OutboundConnections, Is.Empty);
            Assert.That(nodeDefinition.UnlockPrerequisiteIds, Is.Empty);

            Object.DestroyImmediate(nodeDefinition);
        }

        [Test]
        public void ShouldCreateRewardDefinitionWithEmptyPayloadCollections()
        {
            RewardDefinition rewardDefinition = ScriptableObject.CreateInstance<RewardDefinition>();

            Assert.That(rewardDefinition.ResourceAmounts, Is.Empty);
            Assert.That(rewardDefinition.GearRewards, Is.Empty);

            Object.DestroyImmediate(rewardDefinition);
        }

        [Test]
        public void ShouldCreateCharacterAndGearDefinitionsWithStatContainers()
        {
            CharacterDefinition characterDefinition = ScriptableObject.CreateInstance<CharacterDefinition>();
            GearDefinition gearDefinition = ScriptableObject.CreateInstance<GearDefinition>();

            Assert.That(characterDefinition.BaseStats, Is.Not.Null);
            Assert.That(characterDefinition.SupportedGearCategories, Is.Empty);
            Assert.That(gearDefinition.StatModifiers, Is.Not.Null);

            Object.DestroyImmediate(characterDefinition);
            Object.DestroyImmediate(gearDefinition);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo fieldInfo = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(fieldInfo, Is.Not.Null, $"Missing field '{fieldName}'.");
            fieldInfo.SetValue(target, value);
        }
    }
}
