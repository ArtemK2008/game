using System;
using UnityEngine;
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

            RuntimeUiSupport.EnsureInputSystemEventSystem();
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

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(NodePlaceholderScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

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
                20,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 120f);

            statusText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Status",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.78f, 0.82f, 0.90f, 1f));
            RuntimeUiSupport.AddLayoutElement(statusText.gameObject, 78f);

            returnButton = CreateActionButton(
                panelObject.transform,
                "ReturnToWorldMapButton",
                "Resolve Placeholder Node and Return");
            RuntimeUiSupport.AddLayoutElement(returnButton.gameObject, 56f);
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

            Text buttonText = RuntimeUiSupport.CreateText(
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

    }
}
