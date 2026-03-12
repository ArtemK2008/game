using System.Reflection;
using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode
{
    public sealed class BootstrapStartupScreenFlowTests
    {
        [SetUp]
        public void SetUp()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [Test]
        public void ShouldReuseSingleWorldMapAndPlaceholderScreenAcrossEnterAndReturnFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");

            try
            {
                BootstrapStartup bootstrapStartup = hostObject.AddComponent<BootstrapStartup>();
                InvokeAwake(bootstrapStartup);

                AssertScreenCounts(hostObject, 1, 0, 1, 0);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static void EnterNodeFromWorldMap(GameObject rootObject, string nodeButtonName)
        {
            FindButton(rootObject, nodeButtonName).onClick.Invoke();
            FindButton(rootObject, "EnterSelectedNodeButton").onClick.Invoke();
        }

        private static void ReturnToWorldMap(GameObject rootObject)
        {
            FindButton(rootObject, "ReturnToWorldMapButton").onClick.Invoke();
        }

        private static void AssertScreenCounts(
            GameObject rootObject,
            int activeWorldMapCount,
            int activePlaceholderCount,
            int totalWorldMapCount,
            int totalPlaceholderCount)
        {
            Assert.That(CountActiveComponents<WorldMapScreen>(rootObject), Is.EqualTo(activeWorldMapCount));
            Assert.That(CountActiveComponents<NodePlaceholderScreen>(rootObject), Is.EqualTo(activePlaceholderCount));
            Assert.That(rootObject.GetComponentsInChildren<WorldMapScreen>(true).Length, Is.EqualTo(totalWorldMapCount));
            Assert.That(rootObject.GetComponentsInChildren<NodePlaceholderScreen>(true).Length, Is.EqualTo(totalPlaceholderCount));
        }

        private static int CountActiveComponents<T>(GameObject rootObject) where T : Component
        {
            int activeCount = 0;
            foreach (T component in rootObject.GetComponentsInChildren<T>(true))
            {
                if (component.gameObject.activeInHierarchy)
                {
                    activeCount++;
                }
            }

            return activeCount;
        }

        private static void InvokeAwake(BootstrapStartup bootstrapStartup)
        {
            MethodInfo awakeMethod = typeof(BootstrapStartup).GetMethod(
                "Awake",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(awakeMethod, Is.Not.Null);

            awakeMethod.Invoke(bootstrapStartup, null);
        }

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
        }
    }
}
