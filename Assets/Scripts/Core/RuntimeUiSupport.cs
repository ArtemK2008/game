using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Core
{
    /// <summary>
    /// Общий helper для сборки и базовой настройки runtime UI в placeholder-экранах.
    /// </summary>
    public static class RuntimeUiSupport
    {
        public static void EnsureInputSystemEventSystem()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
            }

            InputSystemUIInputModule inputSystemModule;
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.SetActive(false);
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
                inputSystemModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
                eventSystemObject.SetActive(true);
            }
            else
            {
                inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (inputSystemModule == null)
                {
                    inputSystemModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }

            inputSystemModule.enabled = true;

            BaseInputModule[] inputModules = eventSystem.GetComponents<BaseInputModule>();
            foreach (BaseInputModule inputModule in inputModules)
            {
                if (inputModule == null || inputModule == inputSystemModule)
                {
                    continue;
                }

                inputModule.enabled = false;
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(inputModule);
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(inputModule);
            }
        }

        public static Font LoadFallbackFont(string consumerName)
        {
            if (string.IsNullOrWhiteSpace(consumerName))
            {
                throw new ArgumentException("Consumer name cannot be null or whitespace.", nameof(consumerName));
            }

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
            {
                return font;
            }

            throw new InvalidOperationException($"{consumerName} could not load a Unity-compatible runtime font.");
        }

        public static Text CreateText(
            Transform parent,
            Font font,
            string objectName,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static LayoutElement AddLayoutElement(
            GameObject gameObject,
            float preferredHeight,
            float flexibleWidth = 0f,
            float flexibleHeight = 0f,
            float preferredWidth = -1f)
        {
            LayoutElement layoutElement = GetOrAddComponent<LayoutElement>(gameObject);
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = flexibleHeight;

            if (preferredWidth >= 0f)
            {
                layoutElement.minWidth = preferredWidth;
                layoutElement.preferredWidth = preferredWidth;
            }

            return layoutElement;
        }

        public static T GetOrAddComponent<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return target.AddComponent<T>();
        }
    }
}

