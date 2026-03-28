using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Core
{
    /// <summary>
    /// Renders a compact in-game system menu overlay with the shared compact settings surface.
    /// </summary>
    public sealed class CompactSystemMenuView : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private Text titleText;
        private Text summaryText;
        private Button exitButton;
        private Text exitButtonText;
        private GameObject menuButtonGroup;
        private GameObject settingsButtonGroup;
        private CompactSettingsView settingsView;
        private Font uiFont;
        private bool canExit;
        private bool isShowingSettings;
        private UserSettingsState currentSettingsState;
        private Action onExitRequested;
        private Action onResumeRequested;
        private Action<UserSettingsState> onSettingsChanged;

        public bool IsVisible => gameObject.activeSelf;

        public void ShowMenu(
            bool canExit,
            Action onResumeRequested,
            Action onExitRequested = null,
            UserSettingsState settingsState = null,
            Action<UserSettingsState> settingsChanged = null)
        {
            this.canExit = canExit;
            this.onResumeRequested = onResumeRequested ?? throw new ArgumentNullException(nameof(onResumeRequested));
            this.onExitRequested = onExitRequested;
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged = settingsChanged;
            isShowingSettings = false;

            EnsureUi();
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            Refresh();
        }

        public void HideMenu()
        {
            isShowingSettings = false;
            settingsView?.Hide();
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
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

            titleText.text = "System Menu";
            summaryText.text = canExit && onExitRequested != null
                ? "Resume returns to the current context.\nExit safely returns to the main menu from this context."
                : "Resume returns to the current context.\nExit is unavailable until you reach a safe world, service, or resolved post-run context.";
            menuButtonGroup.SetActive(true);
            settingsButtonGroup.SetActive(false);
            settingsView.gameObject.SetActive(false);
            exitButton.interactable = canExit && onExitRequested != null;
            exitButtonText.text = exitButton.interactable ? "Exit" : "Exit Unavailable";
        }

        private void EnsureUi()
        {
            if (canvasGroup != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(CompactSystemMenuView));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvasGroup = RuntimeUiSupport.GetOrAddComponent<CanvasGroup>(gameObject);

            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.offsetMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;
            rootRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rootRectTransform.localScale = Vector3.one;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            GameObject backdropObject = new GameObject(
                "SystemMenuBackdrop",
                typeof(RectTransform),
                typeof(Image));
            backdropObject.transform.SetParent(transform, false);

            RectTransform backdropRectTransform = backdropObject.GetComponent<RectTransform>();
            backdropRectTransform.anchorMin = Vector2.zero;
            backdropRectTransform.anchorMax = Vector2.one;
            backdropRectTransform.offsetMin = Vector2.zero;
            backdropRectTransform.offsetMax = Vector2.zero;
            backdropRectTransform.localScale = Vector3.one;

            Image backdropImage = backdropObject.GetComponent<Image>();
            backdropImage.color = new Color(0f, 0f, 0f, 0.56f);

            GameObject panelObject = new GameObject(
                "SystemMenuPanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup));
            panelObject.transform.SetParent(transform, false);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.32f, 0.24f);
            panelRectTransform.anchorMax = new Vector2(0.68f, 0.76f);
            panelRectTransform.offsetMin = Vector2.zero;
            panelRectTransform.offsetMax = Vector2.zero;
            panelRectTransform.localScale = Vector3.one;

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.09f, 0.12f, 0.97f);

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
                "SystemMenuTitle",
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, 44f);

            summaryText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "SystemMenuSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.92f, 0.96f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 92f);

            menuButtonGroup = CreateButtonGroup(panelObject.transform, "SystemMenuButtons");
            CreateActionButton(menuButtonGroup.transform, "SystemMenuResumeButton", "Resume", HandleResumeRequested);
            CreateActionButton(menuButtonGroup.transform, "SystemMenuSettingsButton", "Settings", HandleSettingsRequested);
            exitButton = CreateActionButton(menuButtonGroup.transform, "SystemMenuExitButton", "Exit", HandleExitRequested);
            exitButtonText = exitButton.GetComponentInChildren<Text>(true);

            settingsButtonGroup = CreateButtonGroup(panelObject.transform, "SystemMenuSettingsButtons");
            settingsView = settingsButtonGroup.AddComponent<CompactSettingsView>();
            settingsView.Hide();
        }

        private static GameObject CreateButtonGroup(Transform parent, string objectName)
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

        private void HandleResumeRequested()
        {
            onResumeRequested?.Invoke();
        }

        private void HandleSettingsRequested()
        {
            isShowingSettings = true;
            settingsView.Show(
                currentSettingsState,
                HandleSettingsChanged,
                HandleSettingsBackRequested,
                "SystemMenuSettingsBackButton");
            Refresh();
        }

        private void HandleSettingsBackRequested()
        {
            isShowingSettings = false;
            settingsView.Hide();
            Refresh();
        }

        private void HandleExitRequested()
        {
            if (!canExit || onExitRequested == null)
            {
                return;
            }

            onExitRequested.Invoke();
        }

        private void HandleSettingsChanged(UserSettingsState settingsState)
        {
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged?.Invoke(currentSettingsState.Clone());
        }
    }
}
