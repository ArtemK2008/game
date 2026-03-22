using NUnit.Framework;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode.Core
{
    /// <summary>
    /// Проверяет базовые smoke-сценарии общего runtime UI helper после переноса в Core.
    /// </summary>
    public sealed class RuntimeUiSupportTests
    {
        [SetUp]
        public void SetUp()
        {
            DestroyAllEventSystems();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyAllEventSystems();
        }

        [Test]
        public void ShouldEnsureInputSystemEventSystemWithEnabledInputModule()
        {
            RuntimeUiSupport.EnsureInputSystemEventSystem();

            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);

            Assert.That(eventSystem, Is.Not.Null);
            Assert.That(eventSystem.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);
            Assert.That(eventSystem.GetComponent<InputSystemUIInputModule>().enabled, Is.True);
        }

        [Test]
        public void ShouldCreateConfiguredTextObjectUnderParent()
        {
            GameObject parentObject = new GameObject("Parent", typeof(RectTransform));
            Font font = RuntimeUiSupport.LoadFallbackFont(nameof(RuntimeUiSupportTests));

            try
            {
                Text text = RuntimeUiSupport.CreateText(
                    parentObject.transform,
                    font,
                    "ChildText",
                    18,
                    FontStyle.Bold,
                    TextAnchor.MiddleLeft,
                    Color.white);

                Assert.That(text.font, Is.SameAs(font));
                Assert.That(text.fontSize, Is.EqualTo(18));
                Assert.That(text.fontStyle, Is.EqualTo(FontStyle.Bold));
                Assert.That(text.alignment, Is.EqualTo(TextAnchor.MiddleLeft));
                Assert.That(text.color, Is.EqualTo(Color.white));
                Assert.That(text.horizontalOverflow, Is.EqualTo(HorizontalWrapMode.Wrap));
                Assert.That(text.verticalOverflow, Is.EqualTo(VerticalWrapMode.Overflow));
                Assert.That(text.raycastTarget, Is.False);
                Assert.That(text.transform.parent, Is.EqualTo(parentObject.transform));
            }
            finally
            {
                Object.DestroyImmediate(parentObject);
            }
        }

        [Test]
        public void ShouldReuseExistingComponentAndConfigureLayoutElement()
        {
            GameObject targetObject = new GameObject("Target");

            try
            {
                Image firstImage = RuntimeUiSupport.GetOrAddComponent<Image>(targetObject);
                Image secondImage = RuntimeUiSupport.GetOrAddComponent<Image>(targetObject);
                LayoutElement layoutElement = RuntimeUiSupport.AddLayoutElement(
                    targetObject,
                    preferredHeight: 42f,
                    flexibleWidth: 1f,
                    flexibleHeight: 2f,
                    preferredWidth: 120f);

                Assert.That(secondImage, Is.SameAs(firstImage));
                Assert.That(layoutElement.minHeight, Is.EqualTo(42f));
                Assert.That(layoutElement.preferredHeight, Is.EqualTo(42f));
                Assert.That(layoutElement.flexibleWidth, Is.EqualTo(1f));
                Assert.That(layoutElement.flexibleHeight, Is.EqualTo(2f));
                Assert.That(layoutElement.minWidth, Is.EqualTo(120f));
                Assert.That(layoutElement.preferredWidth, Is.EqualTo(120f));
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        private static void DestroyAllEventSystems()
        {
            EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            for (int index = 0; index < eventSystems.Length; index++)
            {
                Object.DestroyImmediate(eventSystems[index].gameObject);
            }
        }
    }
}
