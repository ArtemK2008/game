using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public abstract class NodePlaceholderScreenUiTestBase
    {
        [SetUp]
        public void SetUp()
        {
            DestroyCurrentEventSystem();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyCurrentEventSystem();
        }

        protected static NodePlaceholderState CreatePlaceholderState()
        {
            return NodePlaceholderTestData.CreateServicePlaceholderState();
        }

        protected static NodePlaceholderState CreateCombatPlaceholderState()
        {
            return NodePlaceholderTestData.CreateCombatPlaceholderState();
        }

        protected static NodePlaceholderState CreateBossCombatPlaceholderState()
        {
            return NodePlaceholderTestData.CreateBossCombatPlaceholderState();
        }

        protected static WorldGraph CreateWorldGraph()
        {
            return BootstrapWorldTestData.CreateWorldGraph();
        }

        protected static void AdvanceToPostRun(GameObject rootObject)
        {
            Button advanceRunLifecycleButton = FindButton(rootObject, "AdvanceRunLifecycleButton");
            Button returnToWorldButton = FindButton(rootObject, "ReturnToWorldMapButton");

            for (int index = 0; index < 40; index++)
            {
                if (returnToWorldButton.interactable)
                {
                    return;
                }

                if (TryChooseFirstRunTimeSkillUpgrade(rootObject))
                {
                    continue;
                }

                if (!advanceRunLifecycleButton.interactable)
                {
                    InvokeRuntimeAdvance(rootObject, 0.25f);
                    continue;
                }

                advanceRunLifecycleButton.onClick.Invoke();
            }

            Assert.Fail("AdvanceToPostRun did not reach the post-run state within the expected number of steps.");
        }

        protected static void AutoAdvanceCombat(GameObject rootObject, int stepCount, float elapsedSecondsPerStep)
        {
            for (int index = 0; index < stepCount; index++)
            {
                InvokeRuntimeAdvance(rootObject, elapsedSecondsPerStep);
            }
        }

        protected static void ForceUiLayout(GameObject rootObject)
        {
            Canvas.ForceUpdateCanvases();

            RectTransform[] rectTransforms = rootObject.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rectTransform in rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            Canvas.ForceUpdateCanvases();
        }

        protected static bool ContainsText(GameObject rootObject, string textFragment)
        {
            Text[] labels = rootObject.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                if (label.text.Contains(textFragment))
                {
                    return true;
                }
            }

            return false;
        }

        protected static string FindTextInObject(GameObject rootObject, string objectName)
        {
            Transform[] transforms = rootObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                if (transform.gameObject.name != objectName)
                {
                    continue;
                }

                Text label = transform.GetComponentInChildren<Text>(true);
                if (label == null)
                {
                    break;
                }

                return label.text;
            }

            Assert.Fail($"Text host '{objectName}' was not found.");
            return string.Empty;
        }

        protected static Button FindButton(GameObject rootObject, string buttonObjectName)
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

        protected static Button TryFindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            return null;
        }

        protected static RectTransform FindRectTransform(GameObject rootObject, string objectName)
        {
            RectTransform[] rectTransforms = rootObject.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rectTransform in rectTransforms)
            {
                if (rectTransform.gameObject.name == objectName)
                {
                    return rectTransform;
                }
            }

            Assert.Fail($"RectTransform '{objectName}' was not found.");
            return null;
        }

        protected static bool RectanglesOverlap(RectTransform first, RectTransform second)
        {
            Rect firstRect = GetWorldRect(first);
            Rect secondRect = GetWorldRect(second);
            return firstRect.Overlaps(secondRect, true);
        }

        protected static bool RectangleContains(RectTransform container, RectTransform child)
        {
            Rect containerRect = GetWorldRect(container);
            Rect childRect = GetWorldRect(child);
            return containerRect.xMin <= childRect.xMin &&
                containerRect.xMax >= childRect.xMax &&
                containerRect.yMin <= childRect.yMin &&
                containerRect.yMax >= childRect.yMax;
        }

        private static void DestroyCurrentEventSystem()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        private static bool TryChooseFirstRunTimeSkillUpgrade(GameObject rootObject)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (!button.gameObject.activeInHierarchy ||
                    !button.gameObject.name.EndsWith("_RunTimeSkillUpgradeButton"))
                {
                    continue;
                }

                button.onClick.Invoke();
                return true;
            }

            return false;
        }

        private static void InvokeRuntimeAdvance(GameObject rootObject, float elapsedSeconds)
        {
            NodePlaceholderScreen placeholderScreen = rootObject.GetComponent<NodePlaceholderScreen>();
            Assert.That(placeholderScreen, Is.Not.Null);

            MethodInfo autoAdvanceMethod = typeof(NodePlaceholderScreen).GetMethod(
                "TryAdvanceRuntimeTime",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(autoAdvanceMethod, Is.Not.Null);

            autoAdvanceMethod.Invoke(placeholderScreen, new object[] { elapsedSeconds });
        }

        private static Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            Vector2 min = worldCorners[0];
            Vector2 max = worldCorners[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}

