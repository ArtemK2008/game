using NUnit.Framework;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode.Towns
{
    public sealed class TownServiceScreenUiTests
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
        public void Show_ShouldCreateDistinctNonCombatServiceScreenAndInvokeActions()
        {
            GameObject hostObject = new GameObject("TownServiceScreenHost");
            bool returnedToWorld = false;
            bool stoppedSession = false;

            try
            {
                TownServiceScreen townServiceScreen = hostObject.AddComponent<TownServiceScreen>();
                townServiceScreen.Show(
                    NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                    BootstrapWorldTestData.CreateGameState(),
                    () => returnedToWorld = true,
                    () => stoppedSession = true);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
                Assert.That(hostObject.GetComponent<GraphicRaycaster>(), Is.Not.Null);

                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                Assert.That(eventSystem, Is.Not.Null);
                Assert.That(eventSystem.gameObject.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);

                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Build preparation"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned package: Standard Guard"), Is.True);
                Assert.That(TryFindButton(hostObject, "AdvanceRunLifecycleButton"), Is.Null);
                Assert.That(TryFindButton(hostObject, "ReplayNodeButton"), Is.Null);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(returnedToWorld, Is.True);
                Assert.That(stoppedSession, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static bool ContainsText(GameObject rootObject, string textFragment)
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

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button button = TryFindButton(rootObject, buttonObjectName);
            if (button != null)
            {
                return button;
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
        }

        private static Button TryFindButton(GameObject rootObject, string buttonObjectName)
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
    }
}
