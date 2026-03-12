using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class NodePlaceholderScreen : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Text statusText;
        private Button returnButton;
        private Font uiFont;
        private Action onReturnRequested;

        public void Show(NodePlaceholderState placeholderState, Action returnRequested)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            onReturnRequested = returnRequested ?? throw new ArgumentNullException(nameof(returnRequested));
            gameObject.name = "NodePlaceholderScreen";

            EnsureEventSystem();
            EnsureUi();
            Refresh(placeholderState);
        }

        private void Refresh(NodePlaceholderState placeholderState)
        {
            titleText.text = $"Node Placeholder: {placeholderState.NodeId.Value}";
            summaryText.text =
                $"Region: {placeholderState.RegionId.Value}\n" +
                $"Type: {placeholderState.NodeType}\n" +
                $"State: {placeholderState.NodeState}\n" +
                $"Entered from: {placeholderState.OriginNodeId.Value}";
            statusText.text = "Placeholder node content is active. Resolve this placeholder to return to the world map.";
        }

        private void HandleReturnRequested()
        {
            onReturnRequested?.Invoke();
        }

        private void EnsureEventSystem()
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
                    Destroy(inputModule);
                    continue;
                }

                DestroyImmediate(inputModule);
            }
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = LoadDefaultFont();

            RectTransform rootRectTransform = GetOrAddComponent<RectTransform>(gameObject);
            canvas = GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = GetOrAddComponent<CanvasScaler>(gameObject);
            GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 110;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280f, 720f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.offsetMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;
            rootRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rootRectTransform.localScale = Vector3.one;

            GameObject panelObject = new GameObject(
                "Panel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup));
            panelObject.transform.SetParent(transform, false);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.09f, 0.09f, 0.12f, 0.96f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.18f, 0.18f);
            panelRectTransform.anchorMax = new Vector2(0.82f, 0.82f);
            panelRectTransform.offsetMin = Vector2.zero;
            panelRectTransform.offsetMax = Vector2.zero;
            panelRectTransform.localScale = Vector3.one;

            VerticalLayoutGroup panelLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(24, 24, 24, 24);
            panelLayout.spacing = 12f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            titleText = CreateText(panelObject.transform, "Title", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            AddLayoutElement(titleText.gameObject, 44f);

            summaryText = CreateText(panelObject.transform, "Summary", 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.90f, 0.90f, 0.94f, 1f));
            AddLayoutElement(summaryText.gameObject, 120f);

            statusText = CreateText(panelObject.transform, "Status", 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.78f, 0.82f, 0.90f, 1f));
            AddLayoutElement(statusText.gameObject, 78f);

            returnButton = CreateActionButton(
                panelObject.transform,
                "ReturnToWorldMapButton",
                "Resolve Placeholder Node and Return");
            AddLayoutElement(returnButton.gameObject, 56f);
            returnButton.onClick.AddListener(HandleReturnRequested);
        }

        private Button CreateActionButton(Transform parent, string objectName, string label)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.20f, 0.46f, 0.26f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            Text buttonText = CreateText(buttonObject.transform, "Label", 18, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            buttonText.text = label;

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;

            return button;
        }

        private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = uiFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return text;
        }

        private static void AddLayoutElement(GameObject gameObject, float preferredHeight)
        {
            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        private static T GetOrAddComponent<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return target.AddComponent<T>();
        }

        private static Font LoadDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
            {
                return font;
            }

            throw new InvalidOperationException("NodePlaceholderScreen could not load a Unity-compatible runtime font.");
        }
    }
}
