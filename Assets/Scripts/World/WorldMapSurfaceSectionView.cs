using System;
using System.Collections.Generic;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Renders the authored world-map surface, node art, and route overlays without owning world logic.
    /// </summary>
    internal sealed class WorldMapSurfaceSectionView
    {
        private const float AuthoredNodeHitAreaSize = 108f;
        private const float AuthoredNodeAccentSize = 88f;
        private const float AuthoredSelectedGlowSize = 102f;
        private const float AuthoredCurrentGlowSize = 90f;
        private const float AuthoredNodeIconSize = 76f;
        private const float AuthoredConnectionThickness = 5f;

        private const float FallbackNodeButtonWidth = 184f;
        private const float FallbackNodeButtonHeight = 188f;
        private const float FallbackNodeBackingSize = 116f;
        private const float FallbackHighlightBackingSize = 136f;
        private const float FallbackNodeIconSize = 112f;
        private const float FallbackConnectionThickness = 8f;

        private readonly Font uiFont;
        private readonly RectTransform scrollViewRectTransform;
        private readonly RectTransform viewportRectTransform;
        private readonly RectTransform contentRectTransform;
        private readonly RectTransform connectionLayerRectTransform;
        private readonly RectTransform nodeLayerRectTransform;
        private readonly ScrollRect scrollRect;
        private readonly Image backgroundImage;
        private readonly AspectRatioFitter contentAspectFitter;
        private readonly Dictionary<NodeId, RectTransform> nodeButtonRectsById = new Dictionary<NodeId, RectTransform>();
        private readonly List<ConnectionVisual> connectionVisuals = new List<ConnectionVisual>();

        private WorldMapSurfaceLayout currentLayout;

        private WorldMapSurfaceSectionView(
            Font uiFont,
            RectTransform scrollViewRectTransform,
            RectTransform viewportRectTransform,
            RectTransform contentRectTransform,
            RectTransform connectionLayerRectTransform,
            RectTransform nodeLayerRectTransform,
            ScrollRect scrollRect,
            Image backgroundImage,
            AspectRatioFitter contentAspectFitter)
        {
            this.uiFont = uiFont ?? throw new ArgumentNullException(nameof(uiFont));
            this.scrollViewRectTransform = scrollViewRectTransform ??
                throw new ArgumentNullException(nameof(scrollViewRectTransform));
            this.viewportRectTransform = viewportRectTransform ??
                throw new ArgumentNullException(nameof(viewportRectTransform));
            this.contentRectTransform = contentRectTransform ??
                throw new ArgumentNullException(nameof(contentRectTransform));
            this.connectionLayerRectTransform = connectionLayerRectTransform ??
                throw new ArgumentNullException(nameof(connectionLayerRectTransform));
            this.nodeLayerRectTransform = nodeLayerRectTransform ??
                throw new ArgumentNullException(nameof(nodeLayerRectTransform));
            this.scrollRect = scrollRect ?? throw new ArgumentNullException(nameof(scrollRect));
            this.backgroundImage = backgroundImage ?? throw new ArgumentNullException(nameof(backgroundImage));
            this.contentAspectFitter = contentAspectFitter ?? throw new ArgumentNullException(nameof(contentAspectFitter));
        }

        public static WorldMapSurfaceSectionView Create(Transform parent, Font font)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            GameObject scrollViewObject = new GameObject(
                "NodeListScrollView",
                typeof(RectTransform),
                typeof(Image),
                typeof(ScrollRect),
                typeof(LayoutElement));
            scrollViewObject.transform.SetParent(parent, false);

            RectTransform scrollViewRectTransform = scrollViewObject.GetComponent<RectTransform>();
            scrollViewRectTransform.localScale = Vector3.one;

            Image scrollViewImage = scrollViewObject.GetComponent<Image>();
            scrollViewImage.color = new Color(0f, 0f, 0f, 0.05f);

            LayoutElement layoutElement = scrollViewObject.GetComponent<LayoutElement>();
            layoutElement.minWidth = 720f;
            layoutElement.preferredWidth = 920f;
            layoutElement.flexibleWidth = 1f;
            layoutElement.flexibleHeight = 1f;

            GameObject viewportObject = new GameObject(
                "NodeListViewport",
                typeof(RectTransform),
                typeof(Image),
                typeof(RectMask2D));
            viewportObject.transform.SetParent(scrollViewObject.transform, false);

            RectTransform viewportRectTransform = viewportObject.GetComponent<RectTransform>();
            viewportRectTransform.anchorMin = Vector2.zero;
            viewportRectTransform.anchorMax = Vector2.one;
            viewportRectTransform.offsetMin = Vector2.zero;
            viewportRectTransform.offsetMax = Vector2.zero;
            viewportRectTransform.localScale = Vector3.one;

            Image viewportImage = viewportObject.GetComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0f);

            GameObject contentObject = new GameObject(
                "NodeList",
                typeof(RectTransform),
                typeof(AspectRatioFitter));
            contentObject.transform.SetParent(viewportObject.transform, false);

            RectTransform contentRectTransform = contentObject.GetComponent<RectTransform>();
            contentRectTransform.localScale = Vector3.one;

            AspectRatioFitter contentAspectFitter = contentObject.GetComponent<AspectRatioFitter>();
            contentAspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            contentAspectFitter.aspectRatio = 16f / 9f;

            GameObject backgroundObject = new GameObject(
                "MapBackgroundArt",
                typeof(RectTransform),
                typeof(Image));
            backgroundObject.transform.SetParent(contentObject.transform, false);

            RectTransform backgroundRectTransform = backgroundObject.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = Vector2.zero;
            backgroundRectTransform.anchorMax = Vector2.one;
            backgroundRectTransform.offsetMin = Vector2.zero;
            backgroundRectTransform.offsetMax = Vector2.zero;
            backgroundRectTransform.localScale = Vector3.one;

            Image backgroundImage = backgroundObject.GetComponent<Image>();
            backgroundImage.color = new Color(0.22f, 0.18f, 0.14f, 1f);
            backgroundImage.preserveAspect = true;

            RectTransform connectionLayerRectTransform = CreateStretchLayer(
                contentObject.transform,
                "ConnectionLayer");
            RectTransform nodeLayerRectTransform = CreateStretchLayer(contentObject.transform, "NodeLayer");

            ScrollRect scrollRect = scrollViewObject.GetComponent<ScrollRect>();
            scrollRect.content = contentRectTransform;
            scrollRect.viewport = viewportRectTransform;
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 32f;
            scrollRect.verticalNormalizedPosition = 1f;

            return new WorldMapSurfaceSectionView(
                font,
                scrollViewRectTransform,
                viewportRectTransform,
                contentRectTransform,
                connectionLayerRectTransform,
                nodeLayerRectTransform,
                scrollRect,
                backgroundImage,
                contentAspectFitter);
        }

        public void Refresh(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            IReadOnlyList<WorldNodeConnection> connections,
            WorldMapArtResolver artResolver,
            WorldMapSurfaceLayout layout,
            Action<NodeId> onNodeSelected)
        {
            if (nodeOptions == null)
            {
                throw new ArgumentNullException(nameof(nodeOptions));
            }

            if (connections == null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            if (artResolver == null)
            {
                throw new ArgumentNullException(nameof(artResolver));
            }

            if (onNodeSelected == null)
            {
                throw new ArgumentNullException(nameof(onNodeSelected));
            }

            currentLayout = layout;
            nodeButtonRectsById.Clear();
            connectionVisuals.Clear();
            ClearChildren(connectionLayerRectTransform);
            ClearChildren(nodeLayerRectTransform);

            ConfigureContent(layout, artResolver.ResolveBackgroundSpriteOrNull());

            for (int index = 0; index < connections.Count; index++)
            {
                WorldNodeConnection connection = connections[index];
                if (!layout.NodePositions.ContainsKey(connection.SourceNodeId) ||
                    !layout.NodePositions.ContainsKey(connection.TargetNodeId))
                {
                    continue;
                }

                connectionVisuals.Add(CreateConnectionVisual(connection));
            }

            for (int index = 0; index < nodeOptions.Count; index++)
            {
                WorldMapNodeOption nodeOption = nodeOptions[index];
                if (!layout.NodePositions.TryGetValue(nodeOption.NodeId, out Vector2 position))
                {
                    continue;
                }

                RectTransform buttonRectTransform = CreateNodeButton(
                    nodeOption,
                    position,
                    artResolver,
                    onNodeSelected);
                nodeButtonRectsById[nodeOption.NodeId] = buttonRectTransform;
            }
        }

        public void RefreshLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(viewportRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeLayerRectTransform);
            PositionConnectionVisuals();
        }

        private void ConfigureContent(WorldMapSurfaceLayout layout, Sprite backgroundSprite)
        {
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.preserveAspect = layout.UsesNormalizedPositions;
            backgroundImage.color = backgroundSprite == null
                ? new Color(0.18f, 0.15f, 0.12f, 1f)
                : Color.white;

            if (layout.UsesNormalizedPositions)
            {
                contentAspectFitter.enabled = true;
                contentAspectFitter.aspectRatio = layout.ReferenceSize.y <= 0f
                    ? 16f / 9f
                    : layout.ReferenceSize.x / layout.ReferenceSize.y;

                contentRectTransform.anchorMin = Vector2.zero;
                contentRectTransform.anchorMax = Vector2.one;
                contentRectTransform.offsetMin = Vector2.zero;
                contentRectTransform.offsetMax = Vector2.zero;
                contentRectTransform.pivot = new Vector2(0.5f, 0.5f);
                contentRectTransform.anchoredPosition = Vector2.zero;

                scrollRect.horizontal = false;
                scrollRect.vertical = false;
                return;
            }

            contentAspectFitter.enabled = false;
            contentRectTransform.anchorMin = new Vector2(0f, 1f);
            contentRectTransform.anchorMax = new Vector2(1f, 1f);
            contentRectTransform.pivot = new Vector2(0.5f, 1f);
            contentRectTransform.sizeDelta = new Vector2(0f, layout.ReferenceSize.y);
            contentRectTransform.anchoredPosition = Vector2.zero;
            contentRectTransform.offsetMin = new Vector2(0f, contentRectTransform.offsetMin.y);
            contentRectTransform.offsetMax = new Vector2(0f, contentRectTransform.offsetMax.y);

            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.verticalNormalizedPosition = 1f;
        }

        private ConnectionVisual CreateConnectionVisual(WorldNodeConnection connection)
        {
            GameObject lineObject = new GameObject(
                $"{connection.SourceNodeId.Value}_to_{connection.TargetNodeId.Value}_Connection",
                typeof(RectTransform),
                typeof(Image));
            lineObject.transform.SetParent(connectionLayerRectTransform, false);

            RectTransform lineRectTransform = lineObject.GetComponent<RectTransform>();
            lineRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            lineRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            lineRectTransform.pivot = new Vector2(0.5f, 0.5f);
            lineRectTransform.localScale = Vector3.one;

            Image lineImage = lineObject.GetComponent<Image>();
            lineImage.color = currentLayout.UsesNormalizedPositions
                ? new Color(0.90f, 0.82f, 0.66f, 0.22f)
                : new Color(0.90f, 0.82f, 0.66f, 0.28f);
            lineImage.raycastTarget = false;

            return new ConnectionVisual(connection.SourceNodeId, connection.TargetNodeId, lineRectTransform);
        }

        private RectTransform CreateNodeButton(
            WorldMapNodeOption nodeOption,
            Vector2 position,
            WorldMapArtResolver artResolver,
            Action<NodeId> onNodeSelected)
        {
            GameObject buttonObject = new GameObject(
                $"{nodeOption.NodeId.Value}_Button",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(nodeLayerRectTransform, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;
            buttonRectTransform.sizeDelta = currentLayout.UsesNormalizedPositions
                ? Vector2.one * AuthoredNodeHitAreaSize
                : new Vector2(FallbackNodeButtonWidth, FallbackNodeButtonHeight);

            if (currentLayout.UsesNormalizedPositions)
            {
                buttonRectTransform.anchorMin = position;
                buttonRectTransform.anchorMax = position;
                buttonRectTransform.pivot = new Vector2(0.5f, 0.5f);
                buttonRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                buttonRectTransform.anchorMin = new Vector2(0.5f, 1f);
                buttonRectTransform.anchorMax = new Vector2(0.5f, 1f);
                buttonRectTransform.pivot = new Vector2(0.5f, 0.5f);
                buttonRectTransform.anchoredPosition = new Vector2(position.x, -position.y);
            }

            Image hitAreaImage = buttonObject.GetComponent<Image>();
            hitAreaImage.color = new Color(1f, 1f, 1f, 0.002f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = hitAreaImage;
            button.interactable = nodeOption.IsSelectable;
            button.onClick.AddListener(() => onNodeSelected(nodeOption.NodeId));

            Sprite nodeSprite = null;
            artResolver.TryResolveNodeSprite(nodeOption, out nodeSprite);

            if (currentLayout.UsesNormalizedPositions)
            {
                CreateNodeAccent(buttonObject.transform, nodeOption, nodeSprite);
                CreateNodeHighlight(buttonObject.transform, nodeOption, nodeSprite);
            }
            else
            {
                CreateNodeBacking(buttonObject.transform, nodeOption);
            }

            CreateNodeIcon(buttonObject.transform, nodeOption, nodeSprite);
            if (ShouldShowNodeLabel(nodeOption))
            {
                CreateNodeLabel(buttonObject.transform, nodeOption);
            }
            ConfigureButtonColors(button, nodeOption);
            return buttonRectTransform;
        }

        private void CreateNodeBacking(Transform parent, WorldMapNodeOption nodeOption)
        {
            GameObject backingObject = new GameObject(
                "StateBacking",
                typeof(RectTransform),
                typeof(Image));
            backingObject.transform.SetParent(parent, false);

            RectTransform backingRectTransform = backingObject.GetComponent<RectTransform>();
            backingRectTransform.anchorMin = new Vector2(0.5f, 1f);
            backingRectTransform.anchorMax = new Vector2(0.5f, 1f);
            backingRectTransform.pivot = new Vector2(0.5f, 0.5f);
            backingRectTransform.anchoredPosition = new Vector2(0f, -58f);
            backingRectTransform.sizeDelta = Vector2.one *
                (nodeOption.IsSelected || nodeOption.IsCurrentContext
                    ? FallbackHighlightBackingSize
                    : FallbackNodeBackingSize);
            backingRectTransform.localScale = Vector3.one;

            Image backingImage = backingObject.GetComponent<Image>();
            Color backingColor = WorldMapScreenStateResolver.ResolveNodeColor(nodeOption);
            backingImage.color = new Color(backingColor.r, backingColor.g, backingColor.b, 0.86f);
            backingImage.raycastTarget = false;
        }

        private void CreateNodeHighlight(Transform parent, WorldMapNodeOption nodeOption, Sprite nodeSprite)
        {
            if ((!nodeOption.IsSelected && !nodeOption.IsCurrentContext) || nodeSprite == null)
            {
                return;
            }

            GameObject highlightObject = new GameObject(
                "StateGlow",
                typeof(RectTransform),
                typeof(Image));
            highlightObject.transform.SetParent(parent, false);

            RectTransform highlightRectTransform = highlightObject.GetComponent<RectTransform>();
            highlightRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            highlightRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            highlightRectTransform.pivot = new Vector2(0.5f, 0.5f);
            highlightRectTransform.anchoredPosition = Vector2.zero;
            highlightRectTransform.sizeDelta = Vector2.one *
                (nodeOption.IsSelected ? AuthoredSelectedGlowSize : AuthoredCurrentGlowSize);
            highlightRectTransform.localScale = Vector3.one;

            Image highlightImage = highlightObject.GetComponent<Image>();
            highlightImage.sprite = nodeSprite;
            highlightImage.preserveAspect = true;
            highlightImage.raycastTarget = false;

            Color highlightColor = WorldMapScreenStateResolver.ResolveNodeColor(nodeOption);
            float alpha = nodeOption.IsSelected ? 0.26f : 0.16f;
            highlightImage.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b, alpha);
        }

        private void CreateNodeAccent(Transform parent, WorldMapNodeOption nodeOption, Sprite nodeSprite)
        {
            if (nodeSprite == null ||
                !WorldMapScreenStateResolver.TryResolveNodeAccent(nodeOption, out Color accentColor))
            {
                return;
            }

            GameObject accentObject = new GameObject(
                "StateAccent",
                typeof(RectTransform),
                typeof(Image));
            accentObject.transform.SetParent(parent, false);

            RectTransform accentRectTransform = accentObject.GetComponent<RectTransform>();
            accentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            accentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            accentRectTransform.pivot = new Vector2(0.5f, 0.5f);
            accentRectTransform.anchoredPosition = Vector2.zero;
            accentRectTransform.sizeDelta = Vector2.one * AuthoredNodeAccentSize;
            accentRectTransform.localScale = Vector3.one;

            Image accentImage = accentObject.GetComponent<Image>();
            accentImage.sprite = nodeSprite;
            accentImage.preserveAspect = true;
            accentImage.raycastTarget = false;
            accentImage.color = accentColor;
        }

        private void CreateNodeIcon(Transform parent, WorldMapNodeOption nodeOption, Sprite nodeSprite)
        {
            GameObject iconObject = new GameObject(
                "StateIcon",
                typeof(RectTransform),
                typeof(Image));
            iconObject.transform.SetParent(parent, false);

            RectTransform iconRectTransform = iconObject.GetComponent<RectTransform>();
            if (currentLayout.UsesNormalizedPositions)
            {
                iconRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                iconRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                iconRectTransform.pivot = new Vector2(0.5f, 0.5f);
                iconRectTransform.anchoredPosition = Vector2.zero;
                iconRectTransform.sizeDelta = Vector2.one * AuthoredNodeIconSize;
            }
            else
            {
                iconRectTransform.anchorMin = new Vector2(0.5f, 1f);
                iconRectTransform.anchorMax = new Vector2(0.5f, 1f);
                iconRectTransform.pivot = new Vector2(0.5f, 0.5f);
                iconRectTransform.anchoredPosition = new Vector2(0f, -58f);
                iconRectTransform.sizeDelta = Vector2.one * FallbackNodeIconSize;
            }

            iconRectTransform.localScale = Vector3.one;

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            if (nodeSprite != null)
            {
                iconImage.sprite = nodeSprite;
                iconImage.color = WorldMapScreenStateResolver.ResolveNodeIconTint(nodeOption);
                return;
            }

            iconImage.color = Color.white;
        }

        private void CreateNodeLabel(Transform parent, WorldMapNodeOption nodeOption)
        {
            GameObject labelPlateObject = new GameObject(
                "LabelPlate",
                typeof(RectTransform),
                typeof(Image));
            labelPlateObject.transform.SetParent(parent, false);

            RectTransform labelPlateRectTransform = labelPlateObject.GetComponent<RectTransform>();
            if (currentLayout.UsesNormalizedPositions)
            {
                labelPlateRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                labelPlateRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                labelPlateRectTransform.pivot = new Vector2(0.5f, 0.5f);
                labelPlateRectTransform.anchoredPosition = new Vector2(0f, -50f);
                labelPlateRectTransform.sizeDelta = new Vector2(152f, 28f);
            }
            else
            {
                labelPlateRectTransform.anchorMin = new Vector2(0.5f, 0f);
                labelPlateRectTransform.anchorMax = new Vector2(0.5f, 0f);
                labelPlateRectTransform.pivot = new Vector2(0.5f, 0.5f);
                labelPlateRectTransform.anchoredPosition = new Vector2(0f, 30f);
                labelPlateRectTransform.sizeDelta = new Vector2(164f, 54f);
            }

            labelPlateRectTransform.localScale = Vector3.one;

            Image labelPlateImage = labelPlateObject.GetComponent<Image>();
            labelPlateImage.color = currentLayout.UsesNormalizedPositions
                ? new Color(0.05f, 0.07f, 0.10f, 0.92f)
                : new Color(0.05f, 0.07f, 0.10f, 0.84f);
            labelPlateImage.raycastTarget = false;

            Text labelText = RuntimeUiSupport.CreateText(
                labelPlateObject.transform,
                uiFont,
                "Label",
                currentLayout.UsesNormalizedPositions ? 12 : 14,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            labelText.text = BuildNodeSurfaceLabel(nodeOption);
            labelText.resizeTextForBestFit = currentLayout.UsesNormalizedPositions;
            labelText.resizeTextMinSize = 10;
            labelText.resizeTextMaxSize = 12;
            labelText.raycastTarget = false;

            RectTransform labelRectTransform = labelText.rectTransform;
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(8f, 4f);
            labelRectTransform.offsetMax = new Vector2(-8f, -4f);
            labelRectTransform.localScale = Vector3.one;
        }

        private string BuildNodeSurfaceLabel(WorldMapNodeOption nodeOption)
        {
            if (currentLayout.UsesNormalizedPositions)
            {
                return nodeOption.NodeDisplayName;
            }

            string caption = WorldMapScreenTextBuilder.BuildNodeMapCaption(nodeOption);
            if (string.IsNullOrWhiteSpace(caption))
            {
                return nodeOption.NodeDisplayName;
            }

            return $"{nodeOption.NodeDisplayName}\n{caption}";
        }

        private bool ShouldShowNodeLabel(WorldMapNodeOption nodeOption)
        {
            if (!currentLayout.UsesNormalizedPositions)
            {
                return true;
            }

            return nodeOption.IsSelected;
        }

        private static void ConfigureButtonColors(Button button, WorldMapNodeOption nodeOption)
        {
            Color highlightColor = WorldMapScreenStateResolver.ResolveNodeColor(nodeOption);
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 0.002f);
            colors.selectedColor = colors.normalColor;
            colors.highlightedColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0.12f);
            colors.pressedColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0.18f);
            colors.disabledColor = new Color(1f, 1f, 1f, 0.002f);
            button.colors = colors;
        }

        private void PositionConnectionVisuals()
        {
            for (int index = 0; index < connectionVisuals.Count; index++)
            {
                ConnectionVisual connectionVisual = connectionVisuals[index];
                if (!nodeButtonRectsById.TryGetValue(connectionVisual.SourceNodeId, out RectTransform sourceRectTransform) ||
                    !nodeButtonRectsById.TryGetValue(connectionVisual.TargetNodeId, out RectTransform targetRectTransform))
                {
                    connectionVisual.RectTransform.gameObject.SetActive(false);
                    continue;
                }

                Vector2 sourcePosition = GetLocalCenter(sourceRectTransform);
                Vector2 targetPosition = GetLocalCenter(targetRectTransform);
                Vector2 delta = targetPosition - sourcePosition;
                float distance = delta.magnitude;
                if (distance <= 0.01f)
                {
                    connectionVisual.RectTransform.gameObject.SetActive(false);
                    continue;
                }

                connectionVisual.RectTransform.gameObject.SetActive(true);
                float connectionThickness = currentLayout.UsesNormalizedPositions
                    ? AuthoredConnectionThickness
                    : FallbackConnectionThickness;
                connectionVisual.RectTransform.sizeDelta = new Vector2(distance, connectionThickness);
                connectionVisual.RectTransform.anchoredPosition = (sourcePosition + targetPosition) * 0.5f;
                connectionVisual.RectTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            }
        }

        private Vector2 GetLocalCenter(RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            Vector3 worldCenter = (worldCorners[0] + worldCorners[2]) * 0.5f;
            return contentRectTransform.InverseTransformPoint(worldCenter);
        }

        private static RectTransform CreateStretchLayer(Transform parent, string objectName)
        {
            GameObject layerObject = new GameObject(objectName, typeof(RectTransform));
            layerObject.transform.SetParent(parent, false);

            RectTransform layerRectTransform = layerObject.GetComponent<RectTransform>();
            layerRectTransform.anchorMin = Vector2.zero;
            layerRectTransform.anchorMax = Vector2.one;
            layerRectTransform.offsetMin = Vector2.zero;
            layerRectTransform.offsetMax = Vector2.zero;
            layerRectTransform.localScale = Vector3.one;
            return layerRectTransform;
        }

        private static void ClearChildren(Transform container)
        {
            for (int index = container.childCount - 1; index >= 0; index--)
            {
                GameObject childObject = container.GetChild(index).gameObject;
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(childObject);
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(childObject);
            }
        }

        private readonly struct ConnectionVisual
        {
            public ConnectionVisual(NodeId sourceNodeId, NodeId targetNodeId, RectTransform rectTransform)
            {
                SourceNodeId = sourceNodeId;
                TargetNodeId = targetNodeId;
                RectTransform = rectTransform ?? throw new ArgumentNullException(nameof(rectTransform));
            }

            public NodeId SourceNodeId { get; }

            public NodeId TargetNodeId { get; }

            public RectTransform RectTransform { get; }
        }
    }
}
