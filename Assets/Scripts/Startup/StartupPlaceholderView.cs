using System;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Startup
{
    /// <summary>
    /// Renders the compact startup main menu and its shared compact settings surface.
    /// </summary>
    public sealed class StartupPlaceholderView : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Button continueButton;
        private GameObject menuButtonGroup;
        private GameObject settingsButtonGroup;
        private CompactSettingsView settingsView;
        private Font uiFont;
        private bool canContinue;
        private bool isShowingSettings;
        private UserSettingsState currentSettingsState;
        private Action onStartRequested;
        private Action onContinueRequested;
        private Action onQuitRequested;
        private Action<UserSettingsState> onSettingsChanged;

        public void ShowMainMenu(
            bool canContinue,
            Action onStartRequested,
            Action onContinueRequested,
            Action onQuitRequested,
            UserSettingsState settingsState = null,
            Action<UserSettingsState> settingsChanged = null)
        {
            this.canContinue = canContinue;
            this.onStartRequested = onStartRequested;
            this.onContinueRequested = onContinueRequested;
            this.onQuitRequested = onQuitRequested;
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged = settingsChanged;
            isShowingSettings = false;

            gameObject.name = StartupEntryTarget.MainMenuPlaceholder.ToString();
            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();

            Debug.Log("Startup main menu active.");
        }

        private void Refresh()
        {
            if (isShowingSettings)
            {
                titleText.text = "Settings";
                summaryText.text =
                    "Changes apply immediately and persist automatically.\n" +
                    "Adjust master, music, SFX, and the current desktop window mode.";
                menuButtonGroup.SetActive(false);
                settingsButtonGroup.SetActive(true);
                settingsView.gameObject.SetActive(true);
                return;
            }

            titleText.text = "Main Menu";
            summaryText.text = canContinue
                ? "Continue resumes the last safe world or service context.\nStart begins a fresh prototype session."
                : "Start begins a fresh prototype session.\nContinue becomes available after a safe world or service save.";
            menuButtonGroup.SetActive(true);
            settingsButtonGroup.SetActive(false);
            settingsView.gameObject.SetActive(false);
            continueButton.interactable = canContinue && onContinueRequested != null;
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
            panelRectTransform.anchorMin = new Vector2(0.30f, 0.18f);
            panelRectTransform.anchorMax = new Vector2(0.70f, 0.82f);
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
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 92f);

            menuButtonGroup = CreateButtonGroup(panelObject.transform, "MenuButtons");
            CreateActionButton(menuButtonGroup.transform, "StartButton", "Start", HandleStartRequested);
            continueButton = CreateActionButton(menuButtonGroup.transform, "ContinueButton", "Continue", HandleContinueRequested);
            CreateActionButton(menuButtonGroup.transform, "SettingsButton", "Settings", HandleSettingsRequested);
            CreateActionButton(menuButtonGroup.transform, "QuitButton", "Quit", HandleQuitRequested);

            settingsButtonGroup = CreateButtonGroup(panelObject.transform, "SettingsButtons");
            settingsView = settingsButtonGroup.AddComponent<CompactSettingsView>();
            settingsView.Hide();
        }

        private GameObject CreateButtonGroup(Transform parent, string objectName)
        {
            GameObject groupObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(VerticalLayoutGroup));
            groupObject.transform.SetParent(parent, false);

            VerticalLayoutGroup layoutGroup = groupObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 8f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            return groupObject;
        }

        private Button CreateActionButton(
            Transform parent,
            string objectName,
            string label,
            Action onClick)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.18f, 0.24f, 0.32f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.highlightedColor = new Color(0.25f, 0.32f, 0.42f, 1f);
            colors.pressedColor = new Color(0.14f, 0.19f, 0.26f, 1f);
            colors.disabledColor = new Color(0.12f, 0.14f, 0.17f, 0.85f);
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

            RectTransform labelRectTransform = buttonText.rectTransform;
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(12f, 8f);
            labelRectTransform.offsetMax = new Vector2(-12f, -8f);

            RuntimeUiSupport.AddLayoutElement(buttonObject, 48f);
            button.onClick.AddListener(() => onClick?.Invoke());
            return button;
        }

        private void HandleStartRequested()
        {
            onStartRequested?.Invoke();
        }

        private void HandleContinueRequested()
        {
            if (!canContinue)
            {
                return;
            }

            onContinueRequested?.Invoke();
        }

        private void HandleSettingsRequested()
        {
            isShowingSettings = true;
            settingsView.Show(
                currentSettingsState,
                HandleSettingsChanged,
                HandleSettingsBackRequested,
                "SettingsBackButton");
            Refresh();
        }

        private void HandleSettingsBackRequested()
        {
            isShowingSettings = false;
            settingsView.Hide();
            Refresh();
        }

        private void HandleQuitRequested()
        {
            onQuitRequested?.Invoke();
        }

        private void HandleSettingsChanged(UserSettingsState settingsState)
        {
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged?.Invoke(currentSettingsState.Clone());
        }
    }
}
