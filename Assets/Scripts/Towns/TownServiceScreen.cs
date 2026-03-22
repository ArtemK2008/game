using System;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Towns
{
    public sealed class TownServiceScreen : MonoBehaviour
    {
        private const float OverviewPreferredHeight = 120f;
        private const float ProgressionPreferredHeight = 168f;
        private const float BuildPreparationPreferredHeight = 132f;

        private Canvas canvas;
        private Text titleText;
        private Text overviewText;
        private Text progressionText;
        private Text buildPreparationText;
        private Button returnToWorldButton;
        private Text returnToWorldButtonText;
        private Button stopSessionButton;
        private Text stopSessionButtonText;
        private Font uiFont;
        private Action onReturnToWorldRequested;
        private Action onStopSessionRequested;

        public void Show(
            NodePlaceholderState placeholderState,
            PersistentGameState gameState,
            Action returnToWorldRequested,
            Action stopSessionRequested = null,
            TownServiceScreenStateResolver stateResolver = null)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (placeholderState.NodeType != NodeType.ServiceOrProgression)
            {
                throw new ArgumentException(
                    "Town service screen requires a service-or-progression node context.",
                    nameof(placeholderState));
            }

            if (placeholderState.TownServiceContext == null)
            {
                throw new ArgumentException(
                    "Town service screen requires an explicit town service context definition.",
                    nameof(placeholderState));
            }

            TownServiceScreenStateResolver effectiveStateResolver = stateResolver ?? new TownServiceScreenStateResolver();
            TownServiceScreenState screenState = effectiveStateResolver.Resolve(placeholderState, gameState);

            onReturnToWorldRequested = returnToWorldRequested ?? throw new ArgumentNullException(nameof(returnToWorldRequested));
            onStopSessionRequested = stopSessionRequested;
            gameObject.name = "TownServiceScreen";

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh(screenState);
        }

        private void Refresh(TownServiceScreenState screenState)
        {
            titleText.text = screenState.ServiceContext.DisplayName;
            overviewText.text = TownServiceScreenTextBuilder.BuildOverviewText(screenState);
            progressionText.text = TownServiceScreenTextBuilder.BuildProgressionText(screenState);
            buildPreparationText.text = TownServiceScreenTextBuilder.BuildBuildPreparationText(screenState);
            stopSessionButton.interactable = onStopSessionRequested != null;
            stopSessionButtonText.text = onStopSessionRequested == null
                ? "Stop Session Unavailable"
                : "Stop Session";
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(TownServiceScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 105;

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
            panelImage.color = new Color(0.10f, 0.12f, 0.16f, 0.96f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.14f, 0.10f);
            panelRectTransform.anchorMax = new Vector2(0.86f, 0.90f);
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

            titleText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Title",
                30,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, 44f);

            overviewText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Overview",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.92f, 0.97f, 1f));
            RuntimeUiSupport.AddLayoutElement(overviewText.gameObject, OverviewPreferredHeight);

            progressionText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "ProgressionSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.91f, 0.96f, 1f));
            RuntimeUiSupport.AddLayoutElement(progressionText.gameObject, ProgressionPreferredHeight);

            buildPreparationText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "BuildPreparationSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.91f, 0.96f, 1f));
            RuntimeUiSupport.AddLayoutElement(buildPreparationText.gameObject, BuildPreparationPreferredHeight);

            GameObject actionRowObject = new GameObject(
                "ActionRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            actionRowObject.transform.SetParent(panelObject.transform, false);

            HorizontalLayoutGroup actionRowLayout = actionRowObject.GetComponent<HorizontalLayoutGroup>();
            actionRowLayout.spacing = 12f;
            actionRowLayout.childAlignment = TextAnchor.MiddleCenter;
            actionRowLayout.childControlWidth = true;
            actionRowLayout.childControlHeight = true;
            actionRowLayout.childForceExpandWidth = true;
            actionRowLayout.childForceExpandHeight = false;

            RuntimeUiSupport.AddLayoutElement(actionRowObject, 56f);

            returnToWorldButton = CreateActionButton(
                actionRowObject.transform,
                "ReturnToWorldMapButton",
                "Return To World Map",
                new Color(0.22f, 0.39f, 0.58f, 1f),
                out returnToWorldButtonText);
            returnToWorldButton.onClick.AddListener(HandleReturnToWorldRequested);

            stopSessionButton = CreateActionButton(
                actionRowObject.transform,
                "StopSessionButton",
                "Stop Session",
                new Color(0.36f, 0.28f, 0.22f, 1f),
                out stopSessionButtonText);
            stopSessionButton.onClick.AddListener(HandleStopSessionRequested);
        }

        private Button CreateActionButton(
            Transform parent,
            string objectName,
            string label,
            Color buttonColor,
            out Text buttonText)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 56f;
            layoutElement.preferredHeight = 56f;
            layoutElement.flexibleWidth = 1f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = buttonColor;

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

        private void HandleReturnToWorldRequested()
        {
            onReturnToWorldRequested?.Invoke();
        }

        private void HandleStopSessionRequested()
        {
            onStopSessionRequested?.Invoke();
        }
    }
}
