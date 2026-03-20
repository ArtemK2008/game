using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.State;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldMapScreen : MonoBehaviour
    {
        private const float SummaryPreferredHeight = 176f;
        private const float CharacterSelectionSummaryPreferredHeight = 54f;
        private const float CharacterSelectionButtonPreferredHeight = 44f;
        private const float SkillPackageAssignmentSummaryPreferredHeight = 88f;
        private const float SkillPackageAssignmentButtonPreferredHeight = 40f;

        private Canvas canvas;
        private RectTransform panelRectTransform;
        private Text titleText;
        private Text summaryText;
        private Text characterSelectionText;
        private Text skillPackageAssignmentText;
        private Button enterSelectedNodeButton;
        private Text enterSelectedNodeButtonText;
        private RectTransform characterSelectionContainer;
        private RectTransform skillPackageAssignmentContainer;
        private RectTransform nodeListViewport;
        private RectTransform nodeListContainer;
        private ScrollRect nodeListScrollRect;
        private Font uiFont;
        private WorldMapScreenController screenController;
        private Action<NodeId> onNodeEntryRequested;
        private PersistentGameState gameState;
        private PlayableCharacterSelectionService characterSelectionService;
        private PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;

        public void Show(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            Action<NodeId> nodeEntryRequested = null,
            SessionContextState sessionContext = null,
            PersistentGameState gameState = null)
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
            this.gameState = gameState;
            characterSelectionService = gameState == null ? null : new PlayableCharacterSelectionService();
            skillPackageAssignmentService = gameState == null
                ? null
                : new PlayableCharacterSkillPackageAssignmentService(characterSelectionService);
            gameObject.name = "WorldMapScreen";

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();
        }

        private void Refresh()
        {
            IReadOnlyList<WorldMapNodeOption> nodeOptions = screenController.BuildNodeOptions();
            titleText.text = "World Map";
            summaryText.text = WorldMapScreenTextBuilder.BuildSummaryText(
                nodeOptions,
                screenController.HasSelection,
                screenController.HasSelection ? screenController.SelectedNodeId : default,
                screenController.SessionContext,
                screenController.HasForwardRouteChoice,
                screenController.ForwardSelectableNodeCount);
            RefreshCharacterSelection();
            RefreshSkillPackageAssignment();
            RefreshEntryButton();

            ClearNodeButtons();
            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                CreateNodeButton(nodeOption);
            }

            RefreshLayout();
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

        private void HandleCharacterSelection(string characterId)
        {
            if (characterSelectionService == null || gameState == null)
            {
                return;
            }

            if (!characterSelectionService.TrySelectCharacter(gameState, characterId))
            {
                return;
            }

            Debug.Log($"World map character selected: {characterId}.");
            Refresh();
        }

        private void HandleSkillPackageAssignment(string skillPackageId)
        {
            if (skillPackageAssignmentService == null || gameState == null)
            {
                return;
            }

            if (!skillPackageAssignmentService.TryAssignSelectedCharacterSkillPackage(gameState, skillPackageId))
            {
                return;
            }

            Debug.Log($"World map skill package assigned: {skillPackageId}.");
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

            panelRectTransform = panelObject.GetComponent<RectTransform>();
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
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, SummaryPreferredHeight);

            characterSelectionText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "CharacterSelectionSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                characterSelectionText.gameObject,
                CharacterSelectionSummaryPreferredHeight);

            GameObject characterSelectionObject = new GameObject(
                "CharacterSelectionList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            characterSelectionObject.transform.SetParent(panelObject.transform, false);

            VerticalLayoutGroup characterSelectionLayout = characterSelectionObject.GetComponent<VerticalLayoutGroup>();
            characterSelectionLayout.spacing = 8f;
            characterSelectionLayout.childAlignment = TextAnchor.UpperLeft;
            characterSelectionLayout.childControlWidth = true;
            characterSelectionLayout.childControlHeight = true;
            characterSelectionLayout.childForceExpandWidth = true;
            characterSelectionLayout.childForceExpandHeight = false;

            ContentSizeFitter characterSelectionFitter = characterSelectionObject.GetComponent<ContentSizeFitter>();
            characterSelectionFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            characterSelectionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            characterSelectionContainer = characterSelectionObject.GetComponent<RectTransform>();
            characterSelectionContainer.anchorMin = new Vector2(0f, 1f);
            characterSelectionContainer.anchorMax = new Vector2(1f, 1f);
            characterSelectionContainer.pivot = new Vector2(0.5f, 1f);
            characterSelectionContainer.localScale = Vector3.one;

            skillPackageAssignmentText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "SkillPackageAssignmentSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                skillPackageAssignmentText.gameObject,
                SkillPackageAssignmentSummaryPreferredHeight);

            GameObject skillPackageAssignmentObject = new GameObject(
                "SkillPackageAssignmentList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            skillPackageAssignmentObject.transform.SetParent(panelObject.transform, false);

            VerticalLayoutGroup skillPackageAssignmentLayout = skillPackageAssignmentObject.GetComponent<VerticalLayoutGroup>();
            skillPackageAssignmentLayout.spacing = 8f;
            skillPackageAssignmentLayout.childAlignment = TextAnchor.UpperLeft;
            skillPackageAssignmentLayout.childControlWidth = true;
            skillPackageAssignmentLayout.childControlHeight = true;
            skillPackageAssignmentLayout.childForceExpandWidth = true;
            skillPackageAssignmentLayout.childForceExpandHeight = false;

            ContentSizeFitter skillPackageAssignmentFitter = skillPackageAssignmentObject.GetComponent<ContentSizeFitter>();
            skillPackageAssignmentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            skillPackageAssignmentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            skillPackageAssignmentContainer = skillPackageAssignmentObject.GetComponent<RectTransform>();
            skillPackageAssignmentContainer.anchorMin = new Vector2(0f, 1f);
            skillPackageAssignmentContainer.anchorMax = new Vector2(1f, 1f);
            skillPackageAssignmentContainer.pivot = new Vector2(0.5f, 1f);
            skillPackageAssignmentContainer.localScale = Vector3.one;

            enterSelectedNodeButton = CreateActionButton(
                panelObject.transform,
                "EnterSelectedNodeButton",
                "Select a reachable node to enter",
                out enterSelectedNodeButtonText);
            RuntimeUiSupport.AddLayoutElement(enterSelectedNodeButton.gameObject, 56f);
            enterSelectedNodeButton.onClick.AddListener(HandleNodeEntryRequest);

            CreateScrollableNodeList(panelObject.transform);
        }

        private void CreateScrollableNodeList(Transform parent)
        {
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
            scrollViewImage.color = new Color(0.04f, 0.05f, 0.08f, 0.30f);

            LayoutElement scrollViewLayoutElement = scrollViewObject.GetComponent<LayoutElement>();
            scrollViewLayoutElement.flexibleHeight = 1f;

            GameObject viewportObject = new GameObject(
                "NodeListViewport",
                typeof(RectTransform),
                typeof(RectMask2D));
            viewportObject.transform.SetParent(scrollViewObject.transform, false);

            nodeListViewport = viewportObject.GetComponent<RectTransform>();
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

            nodeListContainer = nodeListObject.GetComponent<RectTransform>();
            ConfigureNodeListContentRect();

            nodeListScrollRect = scrollViewObject.GetComponent<ScrollRect>();
            nodeListScrollRect.content = nodeListContainer;
            nodeListScrollRect.viewport = nodeListViewport;
            nodeListScrollRect.horizontal = false;
            nodeListScrollRect.vertical = true;
            nodeListScrollRect.movementType = ScrollRect.MovementType.Clamped;
            nodeListScrollRect.scrollSensitivity = 32f;
            nodeListScrollRect.verticalNormalizedPosition = 1f;
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

        private void ClearCharacterButtons()
        {
            for (int index = characterSelectionContainer.childCount - 1; index >= 0; index--)
            {
                if (Application.isPlaying)
                {
                    Destroy(characterSelectionContainer.GetChild(index).gameObject);
                    continue;
                }

                DestroyImmediate(characterSelectionContainer.GetChild(index).gameObject);
            }
        }

        private void ClearSkillPackageButtons()
        {
            for (int index = skillPackageAssignmentContainer.childCount - 1; index >= 0; index--)
            {
                if (Application.isPlaying)
                {
                    Destroy(skillPackageAssignmentContainer.GetChild(index).gameObject);
                    continue;
                }

                DestroyImmediate(skillPackageAssignmentContainer.GetChild(index).gameObject);
            }
        }

        private void RefreshCharacterSelection()
        {
            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions = BuildCharacterSelectionOptions();
            characterSelectionText.text = WorldMapScreenTextBuilder.BuildCharacterSelectionText(selectionOptions);

            ClearCharacterButtons();
            foreach (PlayableCharacterSelectionOption selectionOption in selectionOptions)
            {
                CreateCharacterButton(selectionOption);
            }
        }

        private void RefreshSkillPackageAssignment()
        {
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions = BuildSkillPackageOptions();
            skillPackageAssignmentText.text = WorldMapScreenTextBuilder.BuildSkillPackageAssignmentText(
                BuildSelectedCharacterDisplayName(),
                skillPackageOptions);

            ClearSkillPackageButtons();
            foreach (PlayableCharacterSkillPackageOption skillPackageOption in skillPackageOptions)
            {
                CreateSkillPackageButton(skillPackageOption);
            }
        }

        private IReadOnlyList<PlayableCharacterSelectionOption> BuildCharacterSelectionOptions()
        {
            if (characterSelectionService == null || gameState == null)
            {
                return Array.Empty<PlayableCharacterSelectionOption>();
            }

            return characterSelectionService.BuildSelectableOptions(gameState);
        }

        private IReadOnlyList<PlayableCharacterSkillPackageOption> BuildSkillPackageOptions()
        {
            if (skillPackageAssignmentService == null || gameState == null)
            {
                return Array.Empty<PlayableCharacterSkillPackageOption>();
            }

            return skillPackageAssignmentService.BuildOptionsForSelectedCharacter(gameState);
        }

        private string BuildSelectedCharacterDisplayName()
        {
            if (skillPackageAssignmentService == null || gameState == null)
            {
                return "none";
            }

            return skillPackageAssignmentService.ResolveSelectedCharacterDisplayName(gameState);
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
            buttonRectTransform.anchorMin = new Vector2(0f, 1f);
            buttonRectTransform.anchorMax = new Vector2(1f, 1f);
            buttonRectTransform.pivot = new Vector2(0.5f, 1f);

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 72f;
            layoutElement.preferredHeight = 72f;
            layoutElement.flexibleWidth = 1f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = WorldMapScreenStateResolver.ResolveNodeColor(nodeOption);

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
            buttonText.text = WorldMapScreenTextBuilder.BuildNodeLabel(nodeOption);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;
        }

        private void CreateCharacterButton(PlayableCharacterSelectionOption selectionOption)
        {
            GameObject buttonObject = new GameObject(
                $"{selectionOption.CharacterId}_CharacterButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(characterSelectionContainer, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = CharacterSelectionButtonPreferredHeight;
            layoutElement.preferredHeight = CharacterSelectionButtonPreferredHeight;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = selectionOption.IsSelected
                ? new Color(0.20f, 0.45f, 0.28f, 1f)
                : new Color(0.25f, 0.33f, 0.58f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => HandleCharacterSelection(selectionOption.CharacterId));

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = WorldMapScreenTextBuilder.BuildCharacterButtonLabel(selectionOption);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;
        }

        private void CreateSkillPackageButton(PlayableCharacterSkillPackageOption skillPackageOption)
        {
            GameObject buttonObject = new GameObject(
                $"{skillPackageOption.SkillPackageId}_SkillPackageButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(skillPackageAssignmentContainer, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = SkillPackageAssignmentButtonPreferredHeight;
            layoutElement.preferredHeight = SkillPackageAssignmentButtonPreferredHeight;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = skillPackageOption.IsAssigned
                ? new Color(0.38f, 0.32f, 0.16f, 1f)
                : new Color(0.23f, 0.28f, 0.48f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => HandleSkillPackageAssignment(skillPackageOption.SkillPackageId));

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = WorldMapScreenTextBuilder.BuildSkillPackageButtonLabel(skillPackageOption);

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
            bool hasSelectedNode = screenController.TryGetSelectedNodeId(out NodeId selectedNodeId);
            WorldMapScreenButtonState buttonState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                onNodeEntryRequested != null,
                hasSelectedNode,
                selectedNodeId);
            enterSelectedNodeButton.interactable = buttonState.IsInteractable;
            enterSelectedNodeButtonText.text = buttonState.Label;
        }

        private void RefreshLayout()
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(characterSelectionContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(skillPackageAssignmentContainer);
            ConfigureNodeListContentRect();
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeListContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeListViewport);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            Canvas.ForceUpdateCanvases();
        }

        private void ConfigureNodeListContentRect()
        {
            if (nodeListContainer == null)
            {
                return;
            }

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

            if (nodeListScrollRect != null)
            {
                nodeListScrollRect.horizontalNormalizedPosition = 0f;
            }
        }
    }
}

