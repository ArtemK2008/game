using System;
using System.Collections.Generic;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Отвечает за scrollable node-list карты мира и за перерисовку кнопок узлов.
    /// </summary>
    internal sealed class WorldMapNodeListSectionView
    {
        private const float NodeButtonPreferredHeight = 84f;

        private readonly Font uiFont;
        private readonly RectTransform nodeListScrollViewRectTransform;
        private readonly RectTransform nodeListViewport;
        private readonly RectTransform nodeListContainer;
        private readonly ScrollRect nodeListScrollRect;

        private WorldMapNodeListSectionView(
            Font uiFont,
            RectTransform nodeListScrollViewRectTransform,
            RectTransform nodeListViewport,
            RectTransform nodeListContainer,
            ScrollRect nodeListScrollRect)
        {
            this.uiFont = uiFont ?? throw new ArgumentNullException(nameof(uiFont));
            this.nodeListScrollViewRectTransform = nodeListScrollViewRectTransform ??
                throw new ArgumentNullException(nameof(nodeListScrollViewRectTransform));
            this.nodeListViewport = nodeListViewport ?? throw new ArgumentNullException(nameof(nodeListViewport));
            this.nodeListContainer = nodeListContainer ?? throw new ArgumentNullException(nameof(nodeListContainer));
            this.nodeListScrollRect = nodeListScrollRect ?? throw new ArgumentNullException(nameof(nodeListScrollRect));
        }

        public static WorldMapNodeListSectionView Create(Transform parent, Font font)
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

            RectTransform nodeListScrollViewRectTransform = scrollViewObject.GetComponent<RectTransform>();
            nodeListScrollViewRectTransform.localScale = Vector3.one;

            Image scrollViewImage = scrollViewObject.GetComponent<Image>();
            scrollViewImage.color = new Color(0.04f, 0.05f, 0.08f, 0.30f);

            LayoutElement scrollViewLayoutElement = scrollViewObject.GetComponent<LayoutElement>();
            scrollViewLayoutElement.flexibleHeight = 1f;

            GameObject viewportObject = new GameObject(
                "NodeListViewport",
                typeof(RectTransform),
                typeof(RectMask2D));
            viewportObject.transform.SetParent(scrollViewObject.transform, false);

            RectTransform nodeListViewport = viewportObject.GetComponent<RectTransform>();
            nodeListViewport.anchorMin = Vector2.zero;
            nodeListViewport.anchorMax = Vector2.one;
            nodeListViewport.offsetMin = Vector2.zero;
            nodeListViewport.offsetMax = Vector2.zero;
            nodeListViewport.pivot = new Vector2(0.5f, 0.5f);
            nodeListViewport.localScale = Vector3.one;

            GameObject nodeListObject = new GameObject(
                "NodeList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            nodeListObject.transform.SetParent(viewportObject.transform, false);

            VerticalLayoutGroup nodeListLayout = nodeListObject.GetComponent<VerticalLayoutGroup>();
            nodeListLayout.spacing = 8f;
            nodeListLayout.childAlignment = TextAnchor.UpperLeft;
            nodeListLayout.childControlWidth = true;
            nodeListLayout.childControlHeight = true;
            nodeListLayout.childForceExpandWidth = true;
            nodeListLayout.childForceExpandHeight = false;

            ContentSizeFitter nodeListFitter = nodeListObject.GetComponent<ContentSizeFitter>();
            nodeListFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            nodeListFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform nodeListContainer = nodeListObject.GetComponent<RectTransform>();

            ScrollRect nodeListScrollRect = scrollViewObject.GetComponent<ScrollRect>();
            nodeListScrollRect.content = nodeListContainer;
            nodeListScrollRect.viewport = nodeListViewport;
            nodeListScrollRect.horizontal = false;
            nodeListScrollRect.vertical = true;
            nodeListScrollRect.movementType = ScrollRect.MovementType.Clamped;
            nodeListScrollRect.scrollSensitivity = 32f;
            nodeListScrollRect.verticalNormalizedPosition = 1f;

            WorldMapNodeListSectionView sectionView = new WorldMapNodeListSectionView(
                font,
                nodeListScrollViewRectTransform,
                nodeListViewport,
                nodeListContainer,
                nodeListScrollRect);
            sectionView.ConfigureNodeListContentRect();
            return sectionView;
        }

        public void Refresh(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            Action<NodeId> onNodeSelected)
        {
            if (nodeOptions == null)
            {
                throw new ArgumentNullException(nameof(nodeOptions));
            }

            if (onNodeSelected == null)
            {
                throw new ArgumentNullException(nameof(onNodeSelected));
            }

            ClearChildren(nodeListContainer);
            for (int index = 0; index < nodeOptions.Count; index++)
            {
                CreateNodeButton(nodeOptions[index], onNodeSelected);
            }
        }

        public void RefreshLayout()
        {
            ConfigureNodeListContentRect();
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeListScrollViewRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeListContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeListViewport);
            ConfigureNodeListContentRect();
        }

        private void CreateNodeButton(WorldMapNodeOption nodeOption, Action<NodeId> onNodeSelected)
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
            buttonRectTransform.anchorMin = new Vector2(0f, 1f);
            buttonRectTransform.anchorMax = new Vector2(1f, 1f);
            buttonRectTransform.pivot = new Vector2(0.5f, 1f);

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = NodeButtonPreferredHeight;
            layoutElement.preferredHeight = NodeButtonPreferredHeight;
            layoutElement.flexibleWidth = 1f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = WorldMapScreenStateResolver.ResolveNodeColor(nodeOption);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.interactable = nodeOption.IsSelectable;
            button.onClick.AddListener(() => onNodeSelected(nodeOption.NodeId));

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
            buttonText.text = WorldMapScreenTextBuilder.BuildNodeLabel(nodeOption);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;
        }

        private void ConfigureNodeListContentRect()
        {
            nodeListContainer.anchorMin = new Vector2(0f, 1f);
            nodeListContainer.anchorMax = new Vector2(1f, 1f);
            nodeListContainer.pivot = new Vector2(0.5f, 1f);
            nodeListContainer.localScale = Vector3.one;

            Vector2 sizeDelta = nodeListContainer.sizeDelta;
            sizeDelta.x = 0f;
            nodeListContainer.sizeDelta = sizeDelta;

            Vector2 offsetMin = nodeListContainer.offsetMin;
            offsetMin.x = 0f;
            nodeListContainer.offsetMin = offsetMin;

            Vector2 offsetMax = nodeListContainer.offsetMax;
            offsetMax.x = 0f;
            nodeListContainer.offsetMax = offsetMax;

            Vector2 anchoredPosition = nodeListContainer.anchoredPosition;
            anchoredPosition.x = 0f;
            nodeListContainer.anchoredPosition = anchoredPosition;

            nodeListScrollRect.horizontalNormalizedPosition = 0f;
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
    }
}
