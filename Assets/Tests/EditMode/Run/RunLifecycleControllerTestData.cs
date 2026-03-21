using NUnit.Framework;
using Survivalon.Run;
using Survivalon.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public static class RunLifecycleControllerTestData
    {
        public static RunLifecycleController CreateController()
        {
            return new RunLifecycleController(NodePlaceholderTestData.CreateServicePlaceholderState());
        }

        public static NodePlaceholderState CreateCombatNodeState()
        {
            return NodePlaceholderTestData.CreateCombatPlaceholderState();
        }

        public static NodePlaceholderState CreateBossCombatNodeState()
        {
            return NodePlaceholderTestData.CreateBossCombatPlaceholderState();
        }

        public static NodePlaceholderState CreatePushCombatNodeState()
        {
            return NodePlaceholderTestData.CreatePushCombatPlaceholderState();
        }

        public static void RunToPostRun(RunLifecycleController controller, int maxStepCount = 128)
        {
            if (controller.RequiresRunTimeSkillUpgradeChoice)
            {
                Assert.That(controller.RunTimeSkillUpgradeOptions, Is.Not.Empty);
                Assert.That(
                    controller.TrySelectRunTimeSkillUpgrade(
                        controller.RunTimeSkillUpgradeOptions[0].UpgradeId),
                    Is.True);
            }

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < maxStepCount && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }
    }
}

