using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class WorldMapScreen : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Button enterSelectedNodeButton;
        private Text enterSelectedNodeButtonText;
        private RectTransform nodeListContainer;
        private Font uiFont;
        private WorldMapScreenController screenController;
        private Action<NodeId> onNodeEntryRequested;

        public void Show(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            Action<NodeId> nodeEntryRequested = null,
            SessionContextState sessionContext = null)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            screenController = new WorldMapScreenController(worldGraph, worldState, sessionContext: sessionContext);
            onNodeEntryRequested = nodeEntryRequested;
            gameObject.name = "WorldMapScreen";

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();
        }

        private void Refresh()
        {
            IReadOnlyList<WorldMapNodeOption> nodeOptions = screenController.BuildNodeOptions();
            titleText.text = "World Map";
            summaryText.text = BuildSummaryText(nodeOptions);
            RefreshEntryButton();

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

        private void HandleNodeEntryRequest()
        {
            if (onNodeEntryRequested == null)
            {
                return;
            }

            if (!screenController.TryGetSelectedNodeId(out NodeId selectedNodeId))
            {
                return;
            }

            onNodeEntryRequested(selectedNodeId);
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(WorldMapScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

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
            panelImage.color = new Color(0.08f, 0.10f, 0.14f, 0.94f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = Vector2.zero;
            panelRectTransform.anchorMax = Vector2.one;
            panelRectTransform.offsetMin = new Vector2(24f, 24f);
            panelRectTransform.offsetMax = new Vector2(-24f, -24f);
            panelRectTransform.localScale = Vector3.one;

            VerticalLayoutGroup panelLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(20, 20, 20, 20);
            panelLayout.spacing = 12f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            titleText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Title",
                30,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, 44f);

            summaryText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Summary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 132f);

            enterSelectedNodeButton = CreateActionButton(
                panelObject.transform,
                "EnterSelectedNodeButton",
                "Select a reachable node to enter",
                out enterSelectedNodeButtonText);
            RuntimeUiSupport.AddLayoutElement(enterSelectedNodeButton.gameObject, 56f);
            enterSelectedNodeButton.onClick.AddListener(HandleNodeEntryRequest);

            GameObject nodeListObject = new GameObject(
                "NodeList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement));
            nodeListObject.transform.SetParent(panelObject.transform, false);

            VerticalLayoutGroup nodeListLayout = nodeListObject.GetComponent<VerticalLayoutGroup>();
            nodeListLayout.spacing = 8f;
            nodeListLayout.childAlignment = TextAnchor.UpperLeft;
            nodeListLayout.childControlWidth = true;
            nodeListLayout.childControlHeight = true;
            nodeListLayout.childForceExpandWidth = true;
            nodeListLayout.childForceExpandHeight = false;

            LayoutElement nodeListLayoutElement = nodeListObject.GetComponent<LayoutElement>();
            nodeListLayoutElement.flexibleHeight = 1f;

            nodeListContainer = nodeListObject.GetComponent<RectTransform>();
            nodeListContainer.anchorMin = new Vector2(0f, 1f);
            nodeListContainer.anchorMax = new Vector2(1f, 1f);
            nodeListContainer.pivot = new Vector2(0.5f, 1f);
            nodeListContainer.localScale = Vector3.one;
        }

        private void ClearNodeButtons()
        {
            for (int i = nodeListContainer.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(nodeListContainer.GetChild(i).gameObject);
                    continue;
                }

                DestroyImmediate(nodeListContainer.GetChild(i).gameObject);
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

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 72f;
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

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                18,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                Color.white);
            buttonText.text = BuildNodeLabel(nodeOption);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;
        }

        private Button CreateActionButton(Transform parent, string objectName, string label, out Text buttonText)
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
            buttonImage.color = new Color(0.25f, 0.33f, 0.58f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = label;

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;

            return button;
        }

        private void RefreshEntryButton()
        {
            NodeId selectedNodeId = default;
            bool canEnterSelection = onNodeEntryRequested != null &&
                screenController.TryGetSelectedNodeId(out selectedNodeId);

            enterSelectedNodeButton.interactable = canEnterSelection;
            enterSelectedNodeButtonText.text = canEnterSelection
                ? $"Enter {selectedNodeId.Value}"
                : "Select a reachable node to enter";
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
            string recentNodeLabel = GetSessionNodeLabel(
                screenController.SessionContext,
                context => context.HasRecentNode,
                context => context.RecentNodeId);
            string recentPushTargetLabel = GetSessionNodeLabel(
                screenController.SessionContext,
                context => context.HasRecentPushTarget,
                context => context.RecentPushTargetNodeId);
            string lastSelectedNodeLabel = GetSessionNodeLabel(
                screenController.SessionContext,
                context => context.HasLastSelectedNode,
                context => context.LastSelectedNodeId);
            int selectableCount = 0;
            int forwardSelectableCount = screenController.ForwardSelectableNodeCount;

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

            string routeChoiceLabel = screenController.HasForwardRouteChoice
                ? "Branch choice available"
                : "Single forward route";

            return
                $"Current node: {currentNodeLabel}\n" +
                $"Recent node: {recentNodeLabel}\n" +
                $"Recent push target: {recentPushTargetLabel}\n" +
                $"Last selected node: {lastSelectedNodeLabel}\n" +
                $"Selectable destinations: {selectableCount}\n" +
                $"Forward route options: {forwardSelectableCount} ({routeChoiceLabel})\n" +
                $"Selected node: {selectedNodeLabel}\n" +
                "Select a reachable node, then confirm entry to start the placeholder node flow.";
        }

        private static string GetSessionNodeLabel(
            SessionContextState sessionContext,
            Func<SessionContextState, bool> hasValue,
            Func<SessionContextState, NodeId> selector)
        {
            if (sessionContext == null || !hasValue(sessionContext))
            {
                return "none";
            }

            return selector(sessionContext).Value;
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

