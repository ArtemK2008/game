using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class WorldMapScreen : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private RectTransform nodeListContainer;
        private Font uiFont;
        private WorldMapScreenController screenController;

        public void Show(WorldGraph worldGraph, PersistentWorldState worldState)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            screenController = new WorldMapScreenController(worldGraph, worldState);
            gameObject.name = "WorldMapScreen";

            EnsureEventSystem();
            EnsureUi();
            Refresh();
        }

        private void Refresh()
        {
            IReadOnlyList<WorldMapNodeOption> nodeOptions = screenController.BuildNodeOptions();
            titleText.text = "World Map";
            summaryText.text = BuildSummaryText(nodeOptions);

            ClearNodeButtons();
            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                CreateNodeButton(nodeOption);
            }
        }

        private void HandleNodeSelection(NodeId nodeId)
        {
            if (!screenController.TrySelectNode(nodeId))
            {
                return;
            }

            Debug.Log($"World map node selected: {nodeId}.");
            Refresh();
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = LoadDefaultFont();

            GameObject canvasObject = new GameObject(
                "Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280f, 720f);

            RectTransform canvasRectTransform = canvasObject.GetComponent<RectTransform>();
            canvasRectTransform.anchorMin = Vector2.zero;
            canvasRectTransform.anchorMax = Vector2.one;
            canvasRectTransform.offsetMin = Vector2.zero;
            canvasRectTransform.offsetMax = Vector2.zero;

            GameObject panelObject = new GameObject(
                "Panel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup));
            panelObject.transform.SetParent(canvasObject.transform, false);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.10f, 0.14f, 0.92f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = Vector2.zero;
            panelRectTransform.anchorMax = Vector2.one;
            panelRectTransform.offsetMin = new Vector2(24f, 24f);
            panelRectTransform.offsetMax = new Vector2(-24f, -24f);

            VerticalLayoutGroup panelLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(20, 20, 20, 20);
            panelLayout.spacing = 12f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = false;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            titleText = CreateText(panelObject.transform, "Title", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            AddLayoutElement(titleText.gameObject, 44f);

            summaryText = CreateText(panelObject.transform, "Summary", 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.88f, 0.90f, 0.94f, 1f));
            AddLayoutElement(summaryText.gameObject, 78f);

            GameObject nodeListObject = new GameObject(
                "NodeList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            nodeListObject.transform.SetParent(panelObject.transform, false);

            VerticalLayoutGroup nodeListLayout = nodeListObject.GetComponent<VerticalLayoutGroup>();
            nodeListLayout.spacing = 8f;
            nodeListLayout.childAlignment = TextAnchor.UpperLeft;
            nodeListLayout.childControlWidth = true;
            nodeListLayout.childControlHeight = false;
            nodeListLayout.childForceExpandWidth = true;
            nodeListLayout.childForceExpandHeight = false;

            ContentSizeFitter contentSizeFitter = nodeListObject.GetComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            nodeListContainer = nodeListObject.GetComponent<RectTransform>();
        }

        private void ClearNodeButtons()
        {
            for (int i = nodeListContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(nodeListContainer.GetChild(i).gameObject);
            }
        }

        private void CreateNodeButton(WorldMapNodeOption nodeOption)
        {
            GameObject buttonObject = new GameObject(
                $"{nodeOption.NodeId.Value}_Button",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(nodeListContainer, false);

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 72f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = GetNodeColor(nodeOption);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.interactable = nodeOption.IsSelectable;
            button.onClick.AddListener(() => HandleNodeSelection(nodeOption.NodeId));

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.85f;
            button.colors = colors;

            Text buttonText = CreateText(buttonObject.transform, "Label", 18, FontStyle.Normal, TextAnchor.MiddleLeft, Color.white);
            buttonText.text = BuildNodeLabel(nodeOption);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
        }

        private Text CreateText(Transform parent, string name, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = uiFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            return text;
        }

        private static void AddLayoutElement(GameObject gameObject, float preferredHeight)
        {
            LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredHeight;
        }

        private static Font LoadDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (font != null)
            {
                return font;
            }

            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private static string BuildNodeLabel(WorldMapNodeOption nodeOption)
        {
            return $"{nodeOption.RegionId.Value} / {nodeOption.NodeId.Value}\nType: {nodeOption.NodeType} | State: {nodeOption.NodeState} | {BuildAvailabilityLabel(nodeOption)}";
        }

        private static string BuildAvailabilityLabel(WorldMapNodeOption nodeOption)
        {
            if (nodeOption.IsSelected)
            {
                return "Selected";
            }

            if (nodeOption.IsCurrentContext)
            {
                return "Current";
            }

            if (nodeOption.IsSelectable)
            {
                return "Selectable";
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return "Locked";
            }

            return "Known";
        }

        private string BuildSummaryText(IReadOnlyList<WorldMapNodeOption> nodeOptions)
        {
            string currentNodeLabel = "unknown";
            string selectedNodeLabel = screenController.HasSelection ? screenController.SelectedNodeId.Value : "none";
            int selectableCount = 0;

            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                if (nodeOption.IsCurrentContext)
                {
                    currentNodeLabel = nodeOption.NodeId.Value;
                }

                if (nodeOption.IsSelectable)
                {
                    selectableCount++;
                }
            }

            return $"Current node: {currentNodeLabel}\nSelectable destinations: {selectableCount}\nSelected node: {selectedNodeLabel}\nSelect a reachable node to mark the next destination.";
        }

        private static Color GetNodeColor(WorldMapNodeOption nodeOption)
        {
            if (nodeOption.IsSelected)
            {
                return new Color(0.77f, 0.62f, 0.20f, 1f);
            }

            if (nodeOption.IsCurrentContext)
            {
                return new Color(0.18f, 0.39f, 0.70f, 1f);
            }

            if (nodeOption.IsSelectable)
            {
                return new Color(0.18f, 0.50f, 0.24f, 1f);
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return new Color(0.24f, 0.24f, 0.27f, 1f);
            }

            return new Color(0.34f, 0.34f, 0.38f, 1f);
        }
    }
}
