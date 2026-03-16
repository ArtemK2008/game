using UnityEngine;
using UnityEngine.UI;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Startup
{
    public sealed class StartupPlaceholderView : MonoBehaviour
    {
        private StartupEntryTarget activeTarget;
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Font uiFont;

        public void Show(StartupEntryTarget target)
        {
            activeTarget = target;
            gameObject.name = target.ToString();
            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();

            Debug.Log($"Startup placeholder active: {activeTarget}.");
        }

        private void Refresh()
        {
            titleText.text = activeTarget == StartupEntryTarget.MainMenuPlaceholder
                ? "Main Menu Placeholder"
                : "Startup Placeholder";
            summaryText.text = BuildSummaryText();
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(StartupPlaceholderView));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 120;

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
            panelImage.color = new Color(0.08f, 0.09f, 0.12f, 0.95f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.22f, 0.28f);
            panelRectTransform.anchorMax = new Vector2(0.78f, 0.72f);
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
                28,
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
                new Color(0.90f, 0.92f, 0.96f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 120f);
        }

        private string BuildSummaryText()
        {
            switch (activeTarget)
            {
                case StartupEntryTarget.MainMenuPlaceholder:
                    return
                        "Session stopped at a safe post-run point.\n" +
                        "This placeholder stands in for the future main menu/system flow.\n" +
                        "You can close the session here or resume world flow later.";
                case StartupEntryTarget.WorldViewPlaceholder:
                    return
                        "World startup placeholder is active.\n" +
                        "The current project usually routes directly into the world map instead.";
                default:
                    return "Startup placeholder active.";
            }
        }
    }
}
